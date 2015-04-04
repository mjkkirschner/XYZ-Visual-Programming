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
using System;

namespace Nodeplay.Engine
{
	class ExplicitGraphExecution:MonoBehaviour
	{
		public List<Task> TaskSchedule;
		public Task CurrentTask;
		public float ExecutionsPerFrame {get;set;}
		public Boolean  ButtonPressed {get;set;}
		public UnityEngine.UI.Toggle DebugModeToggle;

		protected virtual void Start()
		{
			ExecutionsPerFrame =1;
			DebugModeToggle = GameObject.Find("DebugToggle").GetComponent<UnityEngine.UI.Toggle>();
		}

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
		{	//TODO anything where we using find objects of type on nodemodels or graph elements is going to break with custom nodes....
			//need to only search in specific graph model... or use a flag...
			//list of everything that inherits from nodemodel in the scene
			var allnodes = GameObject.FindObjectsOfType<NodeModel>();
			//list of nodemodels where the input list is empty, so
			// this node has no input ports, or where all inputs are connected...think this does that :P
			var nodeps = allnodes.Where(x => x.ExecutionInputs.Count == 0 && !(x is CreateVariable ||x is InputExecutionNode) || x is StartExecution).ToList();
			//nodeps.ForEach(x=>Debug.Log(x.name));
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
		
		IEnumerator WaitForButtonPress(UnityEngine.UI.Button button)
		{
			ButtonPressed = false;
			Debug.Log("setting button pressed to false");
			while(ButtonPressed != true)
				{
				//don't do anything...
				yield return null;
				}
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
			var entrypoints = FindEntryPoints();
			List<Task> actions = entrypoints.Select(x=> new Task(null,x,0,new System.Action (() => x.Evaluate()), new WaitForSeconds(1))).ToList();
			TaskSchedule = new List<Task>(actions);
			
			Task headOfQueue = null;
			while (TaskSchedule.Count > 0)
			{
				//Debug.Log(S.ToJSONstring());
				//Debug.Log("stack count is "+ TaskSchedule.Count);
				//TaskSchedule.ToList().ForEach(x=>Debug.Log(x.NodeRunningOn.GetType().Name));
				foreach (var i in Enumerable.Range(0,(int)ExecutionsPerFrame).ToList())
				{
					headOfQueue = TaskSchedule.First();
					
					
					//pop the node we are about to evaluate, otherwise we'll never be able to 
					
					
					CurrentTask = headOfQueue;
					headOfQueue.MethodCall.Invoke();
					TaskSchedule.RemoveAt(0);

					//now check state of debug mode, if it's enabled move the camera to the location of the executing node
					//and simply poll the state of the continue button, do not continue until button pressed.
					if (DebugModeToggle.isOn)
					{
						var nodepos = headOfQueue.NodeRunningOn.transform.position;
						var offsettpos = nodepos + (headOfQueue.NodeRunningOn.transform.right * 20f);
						//var offsettopos = Camera.main.transform.position + (vectorFromCamToNode * .5f);

						StartCoroutine(slowmove(Camera.main.transform.position,offsettpos,1f,Camera.main.gameObject,nodepos));
						//will need to subclass button so that it holds state or use a toggle...
						//for now just hold state on this execution class, //TODO fix this
						yield return StartCoroutine(WaitForButtonPress(null));
					}


				}
				if (headOfQueue.Yieldbehavior != null ){
					yield return headOfQueue.Yieldbehavior;
				}
			}
			
			
		}
		//TODO this does not belong here
		private IEnumerator slowmove(Vector3 frompos,Vector3 topos, float duration,GameObject goToMove,Vector3 lookat)
		{
			Debug.Log("moving camera");

			for (float f = 0; f <= duration; f = f + Time.deltaTime)
			{

				goToMove.transform.position = Vector3.Lerp(frompos, topos, f);
				goToMove.transform.LookAt(lookat);
				yield return null;
				
			}
			//goToMove.transform.position = topos;
		}

		
	}
	
	
}





