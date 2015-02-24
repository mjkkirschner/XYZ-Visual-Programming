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
		//thi class describes an explict input to a custom node graph that bridges between custom node graph and 
		//wrapper
		public class Symbol : DelegateNodeModel
		{
				private string inputSymbol = String.Empty;

		private CustomNodeWrapper caller;

				public string InputSymbol {
						get { return inputSymbol; }
			set {if (value != inputSymbol)
				{
								inputSymbol = value;
				
								//todo go lookup type from an exosed dropdown of types in current assembly
				var param = new Tuple<string,System.Type>(InputSymbol,typeof(System.Object));

				Parameter = new Tuple<object, Tuple<string,System.Type>> (null, param);
								NotifyPropertyChanged ("InputSymbol");
				}
						}
				}
		
				public Tuple<object,Tuple<string,System.Type>> Parameter { get; private set; }

				protected override void Start ()
				{
						base.Start ();
						AddExecutionInputPort ("GetInputFromOuterGraph");
						AddOutPutPort ("variable_value");
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

						//when this node is executed we need to grab the correct values off the calling wrapper node
						//and then pass them as outputs
						if (caller == null) {
								Debug.LogException (new Exception("the caller has not been set, so this input symbol doesn't know what node to search for input values"));
						}
			//TODO dont know if this is correct using nickname
						output ["variable_value"] = caller.StoredValueDict [InputSymbol];
						(inputstate ["done"] as Delegate).DynamicInvoke ();
						return output;
			
				}

				public override GameObject BuildSceneElements()
				{
					ExposeVariableInNodeUI("InputSymbol",InputSymbol);
					return base.BuildSceneElements();
				}

				public override void Save (XmlDocument doc, XmlElement element)
				{
						base.Save (doc, element);
						XmlElement outEl = element.OwnerDocument.CreateElement ("Symbol");
						outEl.SetAttribute ("value", InputSymbol);
						element.AppendChild (outEl);
				}
			
				public override void Load ( XmlNode node)
				{
						base.Load (node);
						foreach (var subNode in
				         node.ChildNodes.Cast<XmlNode>()
				         .Where(subNode => subNode.Name == "Symbol")) {
								InputSymbol = subNode.Attributes [0].Value;
						}
				
				}
			
		}

}
