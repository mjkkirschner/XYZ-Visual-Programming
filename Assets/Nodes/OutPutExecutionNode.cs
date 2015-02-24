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
	/// OutPut execution node, this class stores a pointer to a trigger port
	/// bridging the wrapper of a customnode and the graph definition,
	/// this node upon execution needs to call the trigger delegate on the external wrapper
	/// </summary>
	public class OutPutExecutionNode : DelegateNodeModel
	{
		public Delegate PointerToAnOutputOnWrapper{get;set;}
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
			AddExecutionInputPort ("start");
			AddExecutionOutPutPort ("done");
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator> ();
			var evaldata = gatherExecutionData();
			//TODO what happens when two outputs have same name, what happens when name changes... does pointer update?
			PointerToAnOutputOnWrapper = evaldata.Where(x=>x.First==Symbol).Select(y=>y.Second).First();
		}

		protected override Dictionary<string, object> CompiledNodeEval (Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			PointerToAnOutputOnWrapper.DynamicInvoke ();
			return output;
			
		}

		protected override void OnNodeModified ()
		{
			if (ExecutionOutputs.Count>0 && ExecutionInputs.Count>0){
			base.OnNodeModified ();
			var evaldata = gatherExecutionData();
			PointerToAnOutputOnWrapper = evaldata.Where(x=>x.First==Symbol).Select(y=>y.Second).First();
			}
		}
		
		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("Symbol",Symbol);
			return base.BuildSceneElements();
		}
		
	}
	
	
}