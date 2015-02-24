using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using System.Reflection;
using System.Xml;

namespace Nodeplay.Nodes
{
		//this class describes some data output from a cusotm node graph
	//when this node is executed it pushes the data on it's inputport to the stored value dict
	//of the calling node, the wrapper in the outer graph that represents it.
		public class Output : DelegateNodeModel
		{
		private CustomNodeWrapper caller;
				private string symbol = "some output";
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
			AddExecutionInputPort ("SetOutputValueOnWrapper");
			AddInputPort ("output_value");
			AddExecutionOutPutPort ("done");
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator> ();
		}

		protected override Dictionary<string, object> CompiledNodeEval (Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			//when this node is being executed the caller must have been passed, so we can search the graph
			//this node belongs to and find the caller in the execution input...
			
			var inputTriggerNode = GraphOwner.Nodes.OfType<InputExecutionNode>().First();
			caller = inputTriggerNode.CustomNodeWrapperCaller;
			
			//when this node is executed we need to push the input value to the stored value dict on the 
			//calling node
			if (caller == null) {
				Debug.LogException (new Exception("the caller has not been set, " +
					"so this input symbol doesn't know what node to search for input values"));
			}

			caller.StoredValueDict[Symbol] = output ["output_value"];

			(inputstate ["done"] as Delegate).DynamicInvoke ();
			return output;
			
		}

		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("Symbol",Symbol);
			return base.BuildSceneElements();
		}

		public override void Save (XmlDocument doc, XmlElement element)
				{
						base.Save (doc, element);
						
						XmlElement outEl = element.OwnerDocument.CreateElement ("Symbol");
						outEl.SetAttribute ("value", Symbol);
						element.AppendChild (outEl);
				}
			
		public override void Load ( XmlNode node)
				{
						base.Load (node);
						foreach (var subNode in 
				         node.ChildNodes.Cast<XmlNode>()
				         .Where(subNode => subNode.Name == "Symbol")) {
								Symbol = subNode.Attributes [0].Value;
						}

				}
			
		}

}
