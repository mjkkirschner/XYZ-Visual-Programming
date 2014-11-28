using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using Nodeplay.UI.Utils;
using Nodeplay.Nodes;

namespace Nodeplay.Engine
{
	class ExplicitGraphExecution:MonoBehaviour
	{
		public Queue<Task> Q;

		public List<NodeModel> FindNodesWithNoDependencies()
		{
			//list of everything that inherits from nodemodel in the scene
			var allnodes = GameObject.FindObjectsOfType<NodeModel>();
			//list of nodemodels where the input list is empty, so
			// this node has no input ports, or where all inputs are connected...think this does that :P
			var nodeps = allnodes.Where(x => x.ExecutionInputs.Count == 0).ToList();
			nodeps.ForEach(x=>Debug.Log(x.name));
			return nodeps;
			
		}

		public List<NodeModel> FindEntryPoints()
		{
			//list of everything that inherits from nodemodel in the scene
			var allnodes = GameObject.FindObjectsOfType<NodeModel>();
			//list of nodemodels where the input list is empty, so
			// this node has no input ports, or where all inputs are connected...think this does that :P
			var nodeps = allnodes.Where(x => x.ExecutionInputs.Count == 0 || x is StartExecution).ToList();
			nodeps.ForEach(x=>Debug.Log(x.name));
			return nodeps;
			
		}


		/// <summary>
		/// method that triggers evaluation on nodes and validates inputs before triggering 
		/// walks from entry points of computation graph to downstream nodes 
		/// </summary>
		public void EvaluateNodes()
		{
			StartCoroutine("ObservableEval");
		}
		/// <summary>
		/// method that checks if a node is ready for eval
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private bool ReadyForEval(NodeModel node)
		{
			foreach (var inputP in node.Inputs)
			{   //TODO add null check for connector
				Debug.Log("checking "+ inputP.NickName + "on" + node);
				Debug.Log(inputP.connectors.Count);
				if (inputP.connectors[0].PStart.Owner.StoredValueDict == null)
				{
					return false;
				}
				
				
			}
			return true;
		}
		
		
		/// <summary>
		/// generator that evals a new node each frame
		/// issue here is that we are actually slowing evaluation 
		/// instead of just slowing the updates to the nodes based on their position
		/// in the eval graph
		/// </summary>
		/// <returns></returns>
		IEnumerator ObservableEval()
		{
			var entrypoints = FindNodesWithNoDependencies();
			List<Task> actions = entrypoints.Select(x=> new Task(x,new System.Action (() => x.Evaluate()), new WaitForSeconds(1))).ToList();
			Q = new Queue<Task>(actions);
			
			
			while (Q.Count > 0)
			{
				//Debug.Log(S.ToJSONstring());
				Debug.Log("stack count is "+ Q.Count);
				Q.ToList().ForEach(x=>Debug.Log(x.Node.GetType().Name));
				var headOfQueue = Q.Peek();
				//TODO this convience method might live on the nodeModel
				//if (ReadyForEval(topofstack))
				//{
					Debug.Log(headOfQueue + " is ready for eval");
					// then evaluate that sucker
					//value will be cached on the node for now
					//TODO what if this node is a data node and requires no evaluation and has no evaluator?
					
					//pop the node we are about to evaluate, otherwise we'll never be able to 
					//
					var popped = Q.Dequeue();
					headOfQueue.MethodCall.Invoke();
					
					//we can now add the nodes that are attached to this nodes outputs
					//add Distinct to eliminate adding a node twice , for instance [] = [] (thats 2 connectors, not equals)
					
					//var childnodes = popped.Outputs.SelectMany(x => x.connectors.Select(y => y.PEnd.Owner)).Distinct().ToList();
					//childnodes = childnodes.Except(S).ToList();
					//childnodes.ForEach(x => S.Push(x));
			//	}

				if (headOfQueue.Yieldbehavior != null ){
					yield return headOfQueue.Yieldbehavior;
					}
				}

				
			}
			
		}
		
		
	}
	
	
	


