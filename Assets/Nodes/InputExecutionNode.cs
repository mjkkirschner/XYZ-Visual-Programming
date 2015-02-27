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
				var evaldata = gatherExecutionData();
				pointerToFirstNodeInGraph = evaldata.First().Second;
			}
		
		protected override void OnNodeModified ()
		{
			//if start hasnt run yet we wont have any outputs on this node yet,
			//so dont try to lookup evaldata... will throw
			if (ExecutionOutputs != null){
			base.OnNodeModified ();
			var evaldata = gatherExecutionData();
			pointerToFirstNodeInGraph = evaldata.First().Second;
			}
		}

		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("Symbol",Symbol);
			return base.BuildSceneElements();
		}

		}


}