using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Nodeplay.Nodes;
using System.Collections;
using Nodeplay.Engine;


//originally forked from:https://github.com/DynamoDS/Dynamo/blob/master/src/DynamoCore/Core/CustomNodeDefinition.cs

namespace Nodeplay.Nodes
{
		/// <summary>
		/// Input execution node, this class defines stores a pointer to a trigger port
		/// bridging the wrapper of a customnode and the graph definition, this node really only stores a pointer
		/// and will never be executed directly
		/// </summary>
		public class InputExecutionNode : DelegateNodeModel
		{
		public Delegate pointerToFirstNodeInGraph{get;set;}
		public CustomNodeWrapper CustomNodeWrapperCaller {get;set;}
		public List<Tuple<string,Action>> Executiondata{ get; set; } 

		/// <summary>
		/// this method is used to force a regathering of execution data for immediate execution
		/// of a node, use case is a custom node wrapper about to execute a trigger on a node
		/// like this inputExecNode, we must gather the correct state of the node triggers
		/// and the currently executing tasks in the schedule, to correctly schedule any tasks
		/// the execution of this node generates... this is ugly and will be refactored later TODO
		/// </summary>
		public void ForceGatherExecutionData()
		{
			Executiondata = gatherExecutionData();
		}

		private string symbol = "";
		public string Symbol {
			get { return symbol; }
			set {
				if (value != symbol)
				{
				symbol = value;
				
				OnNodeModified ();
				NotifyPropertyChanged ("Symbol");
				}
			}
		}
			protected override void Start ()
			{
				base.Start ();
				AddExecutionOutPutPort ("done");
				
				CodePointer = CompiledNodeEval;
				Evaluator = this.gameObject.AddComponent<CsharpEvaluator> ();
				Executiondata = gatherExecutionData();
				pointerToFirstNodeInGraph = Executiondata.First().Second;
			}
		
		protected override void OnNodeModified ()
		{
			//if start hasnt run yet we wont have any outputs on this node yet,
			//so dont try to lookup evaldata... will throw
			if (ExecutionOutputs != null && ExecutionOutputs.Count>1 ){
			base.OnNodeModified ();
			Executiondata = gatherExecutionData();
				pointerToFirstNodeInGraph = Executiondata.First().Second;
			}
		}

		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("Symbol",Symbol);
			return base.BuildSceneElements();
		}

		}


}