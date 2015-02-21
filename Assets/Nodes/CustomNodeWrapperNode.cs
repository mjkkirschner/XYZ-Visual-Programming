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
		//this class describes a custom node as a nodemodel
		//it will be created at runtime from some customnodefunction description and points to some customnode graphmodel
		public class CustomNodeWrapper : DelegateNodeModel
		{
				//the custom node model class holds these values even outside of it's own evaluation scope
				//since we're not sure when the custom node definitio will be done executing and may require access to these values
				public var inputdata = new List<Tuple<string,object>> ();
				public var executiondata = new List<Tuple<string,Action>> ();

	
		public string Description {get;set;}

		public string Category {get;set;}

				public CustomNodeFunctionDescription funcdef;
				public event Action Disposed;

				protected override void Start ()
				{
						base.Start ();
			
						//on start go grab the correct information from the appmodel's customNodeManager dict of loadead custom nodes
						// use the name of this type as the key into the dictionary, or alteratively, the GUID of the function, which
						// we could set on load, this shuld be a guid of the definition not of the node model, but we do not know the
						// guid of the function definition at this point...will have to rectify this later, when we create the type,
						// we may set this function guid explicitly... possibly even assigning the function description instead of getting it.
						funcdef = GameObject.FindObjectOfType<AppModel> ().CustomNodeManager.loadedCustomNodes [this.GetType ().FullName];
						//funcdef = GameObject.FindObjectOfType<AppModel>().CustomNodeManager.loadedCustomNodes[];

						ValidateDefinition (funcdef);

			
						//on start add a correctly named input port for each 
						//parameter in methodinfo that we've loaded
			
						foreach (var param in funcdef.Parameters) {
								AddInputPort (param.Name);
						}
						//add an output port for each outputnode in the customnode graph
						foreach (var dataOutput in funcdef.OutputNodes) {
								AddOutPutPort (dataOutput.Name);
						}

						//add an execution trigger for each inputexec node in the customnode graph
						if (funcdef.InputExecutionNodes.count > 1) {
								Debug.LogException (new ArgumentException ("customnodes cannot yet have more than 1 input trigger"));
				          
						}

						foreach (var inTrigger in funcdef.InputExecutionNodes) {
								AddExecutionInputPort (inTrigger.Name);
						}
						//add an execution trigger out for each outputexec node in the customnode graph
						foreach (var outTrigger in funcdef.OutputExecutionNodes) {
								AddExecutionInputPort (outTrigger.Name);
						}
			



						CodePointer = CustomNodeEval;
						//we do not need an evaluator, there is nothing to evaluate...
						//instead we'll directly compose our scope for evaluation on this node
						//gathering all inputs and storing them on the nodemodel (customnode)
						//then triggering our pointer into the customnode graph
						//the inputNodes in the graph will search the customnodeModel's inputdata
						//I can't think of another way to provide the inputs to the nodes in a clean way

						//Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();

				}

		protected virtual void OnDestroy()
		{
			Disposed();
		}
		
		internal override void Evaluate ()
				{
						OnEvaluation ();
						//build packages for all data 
						inputdata = gatherInputPortData ();
						if (CodePointer == null) {
								Debug.Break ();
						}
						//grab the pointers for downstream execution after the custom node is done
						executiondata = gatherExecutionData ();
						//here we use the codepointer instead of the code string
						//and we'll just call it directly...instead of using an evaluator
						CustomNodeEval ();

						//instead of returning something from the evaluation of this node
						//we rely on the output nodes inside the customnode graph to write their output vals
						//back to this storedvalueDict....ugly I know...an event based flow might be more reliable
						//this.StoredValueDict = outvar;
						OnEvaluated ();
		
				}

				protected override void CustomNodeEval ()
				{
						//for now only allow 1 execution input to simplify things
						Debug.Log ("about to call method:" + funcdef.InputExecutionNodes [0].Name + "on original type:" + this.GetType ().FullName); 
						//now call the method on the "start" execution trigger
						funcdef.InputExecutionNodes [0].DynamicInvoke ();
				}				

		#region customnode controller merged some methods into the wrapper
				public override void SyncNodeWithDefinition(NodeModel model)
				{
					if (IsInSyncWithNode(model)) 
						return;
					
					SyncNodeWithDefinition(model);
					
					model.OnNodeModified();
				}
				//do we need this?
				public override void SerializeCore(XmlElement nodeElement, SaveContext saveContext)
				{
					//Debug.WriteLine(pd.Object.GetType().ToString());
					XmlElement outEl = nodeElement.OwnerDocument.CreateElement("ID");
					
					outEl.SetAttribute("value", Definition.FunctionId.ToString());
					nodeElement.AppendChild(outEl);
					nodeElement.SetAttribute("nickname", NickName);
				}
				
				/// <summary>
				///   Return if the custom node instance is in sync with its definition.
				///   It may be out of sync if .dyf file is opened and updated and then
				///   .dyn file is opened. 
				/// </summary>
				public bool IsInSyncWithNode(NodeModel model)
				{
					if (funcdef == null)
						return true;
					
					if (funcdef.Parameters != null)
					{
						var defParamNames = funcdef.input.Select(p => p.Name);
						var paramNames = model.Inputs.Select(p => p.NickName);
						if (!defParamNames.SequenceEqual(paramNames))
							return false;
						
						var defParamTypes = Definition.Parameters.Select(p => p.Type.ToShortString());
						var paramTypes = model.InPortData.Select(p => p.ToolTipString);
						if (!defParamTypes.SequenceEqual(paramTypes))
							return false;
					}
					
					if (Definition.ReturnKeys != null)
					{
						var returnKeys = model.OutPortData.Select(p => p.NickName);
						if (!Definition.ReturnKeys.SequenceEqual(returnKeys))
							return false;
					}
					
					return true;
				}
		#endregion	

		#region Serialization/Deserialization methods
		
		public override void Save (XmlDocument doc, XmlElement element)
		{
			base.Save (doc, element); //Base implementation must be called
			
			Controller.SerializeCore (element, context);
			
			var xmlDoc = element.OwnerDocument;
			
			var outEl = xmlDoc.CreateElement ("Name");
			outEl.SetAttribute ("value", NickName);
			element.AppendChild (outEl);
			
			outEl = xmlDoc.CreateElement ("Description");
			outEl.SetAttribute ("value", Description);
			element.AppendChild (outEl);
			
			outEl = xmlDoc.CreateElement ("Inputs");
			foreach (string input in InPortData.Select(x => x.NickName)) {
				XmlElement inputEl = xmlDoc.CreateElement ("Input");
				inputEl.SetAttribute ("value", input);
				outEl.AppendChild (inputEl);
			}
			element.AppendChild (outEl);
			
			outEl = xmlDoc.CreateElement ("Outputs");
			foreach (string output in OutPortData.Select(x => x.NickName)) {
								XmlElement outputEl = xmlDoc.CreateElement ("Output");
								outputEl.SetAttribute ("value", output);
								outEl.AppendChild (outputEl);
						}
						element.AppendChild (outEl);
				}
			
				protected override void Load (XmlElement nodeElement, SaveContext context)
				{
						base.DeserializeCore (nodeElement, context); //Base implementation must be called
				
						List<XmlNode> childNodes = nodeElement.ChildNodes.Cast<XmlNode> ().ToList ();
				
						XmlNode nameNode = childNodes.LastOrDefault (subNode => subNode.Name.Equals ("Name"));
						if (nameNode != null && nameNode.Attributes != null)
								NickName = nameNode.Attributes ["value"].Value;
				
						XmlNode descNode = childNodes.LastOrDefault (subNode => subNode.Name.Equals ("Description"));
						if (descNode != null && descNode.Attributes != null)
								Description = descNode.Attributes ["value"].Value;
				
						if (!Controller.IsInSyncWithNode (this)) {
								Controller.SyncNodeWithDefinition (this);
								OnNodeModified ();
						} else {
								foreach (XmlNode subNode in childNodes) {
										if (subNode.Name.Equals ("Outputs")) {
												var data =
								subNode.ChildNodes.Cast<XmlNode> ()
									.Select (
										(outputNode, i) =>
										new
										{
										data = new PortData (outputNode.Attributes [0].Value, Properties.Resources.ToolTipOutput + (i + 1)),
										idx = i
									});
							
												foreach (var dataAndIdx in data) {
														if (OutPortData.Count > dataAndIdx.idx)
																OutPortData [dataAndIdx.idx] = dataAndIdx.data;
														else
																OutPortData.Add (dataAndIdx.data);
												}
										} else if (subNode.Name.Equals ("Inputs")) {
												var data =
								subNode.ChildNodes.Cast<XmlNode> ()
									.Select (
										(inputNode, i) =>
										new
										{
										data = new PortData (inputNode.Attributes [0].Value, Properties.Resources.ToolTipInput + (i + 1)),
										idx = i
									});
							
												foreach (var dataAndIdx in data) {
														if (InPortData.Count > dataAndIdx.idx)
																InPortData [dataAndIdx.idx] = dataAndIdx.data;
														else
																InPortData.Add (dataAndIdx.data);
												}
										}
						
						#region Legacy output support
						
						else if (subNode.Name.Equals ("Output")) {
												var data = new PortData (subNode.Attributes [0].Value, Properties.Resources.ToolTipFunctionOutput);
							
												if (OutPortData.Any ())
														OutPortData [0] = data;
												else
														OutPortData.Add (data);
										}
						
										#endregion
								}
					
								RegisterAllPorts ();
						}
				}
			
			#endregion
			
				private void ValidateDefinition (CustomNodeFunctionDescription def)
				{
						if (def == null) {
								Debug.LogException (new ArgumentNullException ("functiondef is null"));
						}
				
						if (def.IsProxy) {
								this.Error (Properties.Resources.CustomNodeNotLoaded);
						} else {
								this.ClearRuntimeError ();
						}
				}
			
				public void ResyncWithDefinition (CustomNodeFunctionDescription def)
				{
						ValidateDefinition (def);
						Controller.Definition = def;
						Controller.SyncNodeWithDefinition (this);
				}
		}
		
		
		public class Symbol : NodeModel
		{
				private string inputSymbol = String.Empty;
				private string nickName = String.Empty;
			
				public Symbol ()
				{
						OutPortData.Add (new PortData ("", Properties.Resources.ToolTipSymbol));
				
						RegisterAllPorts ();
				
						ArgumentLacing = LacingStrategy.Disabled;
				
						InputSymbol = String.Empty;
				}
			
				public string InputSymbol {
						get { return inputSymbol; }
						set {
								inputSymbol = value;
					
								ClearRuntimeError ();
								var substrings = inputSymbol.Split (':');
					
								nickName = substrings [0].Trim ();
								var type = TypeSystem.BuildPrimitiveTypeObject (PrimitiveType.kTypeVar);
								object defaultValue = null;
					
								if (substrings.Count () > 2) {
										this.Warning (Properties.Resources.WarningInvalidInput);
								} else if (!string.IsNullOrEmpty (nickName) &&
										(substrings.Count () == 2 || InputSymbol.Contains ("="))) {
										// three cases:
										//    x = default_value
										//    x : type
										//    x : type = default_value
										IdentifierNode identifierNode;
										AssociativeNode defaultValueNode;
						
										if (!TryParseInputSymbol (inputSymbol, out identifierNode, out defaultValueNode)) {
												this.Warning (Properties.Resources.WarningInvalidInput);
										} else {
												if (identifierNode.datatype.UID == Constants.kInvalidIndex) {
														string warningMessage = String.Format (
									Properties.Resources.WarningCannotFindType, 
									identifierNode.datatype.Name);
														this.Warning (warningMessage);
												} else {
														nickName = identifierNode.Value;
														type = identifierNode.datatype;
												}
							
												if (defaultValueNode != null) {
														TypeSwitch.Do (
									defaultValueNode,
									TypeSwitch.Case<IntNode> (n => defaultValue = n.Value),
									TypeSwitch.Case<DoubleNode> (n => defaultValue = n.Value),
									TypeSwitch.Case<BooleanNode> (n => defaultValue = n.Value),
									TypeSwitch.Case<StringNode> (n => defaultValue = n.value),
									TypeSwitch.Default (() => defaultValue = null));
												}
										}
								}
					
								Parameter = new TypedParameter (nickName, type, defaultValue);
					
								OnNodeModified ();
								RaisePropertyChanged ("InputSymbol");
						}
				}
			
				public Tuple<object,ParameterInfo> Parameter {
						get;
						private set;
				}
			
				public override IdentifierNode GetAstIdentifierForOutputIndex (int outputIndex)
				{
						return
					AstFactory.BuildIdentifier (
						string.IsNullOrEmpty (nickName) ? AstIdentifierBase : nickName + "__" + AstIdentifierBase);
				}
			
				protected override void SerializeCore (XmlElement nodeElement, SaveContext context)
				{
						base.SerializeCore (nodeElement, context);
						//Debug.WriteLine(pd.Object.GetType().ToString());
						XmlElement outEl = nodeElement.OwnerDocument.CreateElement ("Symbol");
						outEl.SetAttribute ("value", InputSymbol);
						nodeElement.AppendChild (outEl);
				}
			
				protected override void DeserializeCore (XmlElement nodeElement, SaveContext context)
				{
						base.DeserializeCore (nodeElement, context);
						foreach (var subNode in
				         nodeElement.ChildNodes.Cast<XmlNode>()
				         .Where(subNode => subNode.Name == "Symbol")) {
								InputSymbol = subNode.Attributes [0].Value;
						}
				
						ArgumentLacing = LacingStrategy.Disabled;
				}
			
				private bool TryParseInputSymbol (string inputSymbol, 
			                                 out IdentifierNode identifier, 
			                                 out AssociativeNode defaultValue)
				{
						identifier = null;
						defaultValue = null;
				
						// workaround: there is an issue in parsing "x:int" format unless 
						// we create the other parser specially for it. We change it to 
						// "x:int = dummy;" for parsing. 
						var parseString = InputSymbol;
				
						// if it has default value, then append ';'
						if (InputSymbol.Contains ("=")) {
								parseString += ";";
						} else {
								String dummyExpression = "{0}=dummy;";
								parseString = string.Format (dummyExpression, parseString);
						}
				
						ParseParam parseParam = new ParseParam (this.GUID, parseString);
				
						if (EngineController.CompilationServices.PreCompileCodeBlock (ref parseParam) &&
								parseParam.ParsedNodes != null &&
								parseParam.ParsedNodes.Any ()) {
								var node = parseParam.ParsedNodes.First () as BinaryExpressionNode;
								Validity.Assert (node != null);
					
								if (node != null) {
										identifier = node.LeftNode as IdentifierNode;
										if (inputSymbol.Contains ('='))
												defaultValue = node.RightNode;
						
										return identifier != null;
								}
						}
				
						return false;
				}
			
				protected override bool UpdateValueCore (UpdateValueParams updateValueParams)
				{
						string name = updateValueParams.PropertyName;
						string value = updateValueParams.PropertyValue;
				
						if (name == "InputSymbol") {
								InputSymbol = value;
								return true; // UpdateValueCore handled.
						}
				
						return base.UpdateValueCore (updateValueParams);
				}
		}
		
		[NodeName("Output")]
		[NodeCategory(BuiltinNodeCategories.CORE_INPUT)]
		[NodeDescription("OutputNodeDescription",typeof(Dynamo.Properties.Resources))]
		[IsInteractive(false)]
		[NotSearchableInHomeWorkspace]
		[IsDesignScriptCompatible]
		public class Output : NodeModel
		{
				private string symbol = "";
			
				public Output ()
				{
						InPortData.Add (new PortData ("", ""));
				
						RegisterAllPorts ();
				
						ArgumentLacing = LacingStrategy.Disabled;
				}
			
				public string Symbol {
						get { return symbol; }
						set {
								symbol = value;
								OnNodeModified ();
								RaisePropertyChanged ("Symbol");
						}
				}
			
				public override IdentifierNode GetAstIdentifierForOutputIndex (int outputIndex)
				{
						if (outputIndex < 0 || outputIndex > OutPortData.Count)
								throw new ArgumentOutOfRangeException ("outputIndex", @"Index must correspond to an OutPortData index.");
				
						return AstIdentifierForPreview;
				}
			
				internal override IEnumerable<AssociativeNode> BuildAst (List<AssociativeNode> inputAstNodes)
				{
						AssociativeNode assignment;
						if (null == inputAstNodes || inputAstNodes.Count == 0)
								assignment = AstFactory.BuildAssignment (AstIdentifierForPreview, AstFactory.BuildNullNode ());
						else
								assignment = AstFactory.BuildAssignment (AstIdentifierForPreview, inputAstNodes [0]);
				
						return new[] { assignment };
				}
			
				protected override void SerializeCore (XmlElement nodeElement, SaveContext context)
				{
						base.SerializeCore (nodeElement, context);
						//Debug.WriteLine(pd.Object.GetType().ToString());
						XmlElement outEl = nodeElement.OwnerDocument.CreateElement ("Symbol");
						outEl.SetAttribute ("value", Symbol);
						nodeElement.AppendChild (outEl);
				}
			
				protected override void DeserializeCore (XmlElement nodeElement, SaveContext context)
				{
						base.DeserializeCore (nodeElement, context);
						foreach (var subNode in 
				         nodeElement.ChildNodes.Cast<XmlNode>()
				         .Where(subNode => subNode.Name == "Symbol")) {
								Symbol = subNode.Attributes [0].Value;
						}
				
						ArgumentLacing = LacingStrategy.Disabled;
				}
			
				protected override bool UpdateValueCore (UpdateValueParams updateValueParams)
				{
						string name = updateValueParams.PropertyName;
						string value = updateValueParams.PropertyValue;
				
						if (name == "Symbol") {
								Symbol = value;
								return true; // UpdateValueCore handled.
						}
				
						return base.UpdateValueCore (updateValueParams);
				}
		}

}

