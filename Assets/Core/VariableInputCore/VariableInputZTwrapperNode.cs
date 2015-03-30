using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;
using System;
using System.Xml;

namespace Nodeplay.Nodes
{
	
	public class VariableInputZTwrapperNode : VariableInputDelegateNodeModel
	{
		protected FunctionDescription funcdef;
		//TODO might just be a string...

		protected override string GetPortName ()
		{
			return funcdef.Parameters.Last().Name + Inputs.Count.ToString();
		}

		protected override void Start()
		{
			base.Start();
			
			//on start go grab the correct information from the appmodel's loadedFunctions dict
			// use the name of this type as the key into the dictionary
			funcdef = GameObject.FindObjectOfType<AppModel>().LoadedFunctions[this.GetType().FullName];
			
			//on start add a correctly named input port for each 
			//parameter in methodinfo that we've loaded
			
			//foreach (var param in funcdef.Parameters)
			//{
			//	AddInputPort(param.Name);
			//}
			//add 1 output, 1 start, and end trigger
			AddExecutionInputPort("start");
			//TODO support multiout attribute
			AddOutPutPort("OUTPUT");
			AddExecutionOutPutPort("done");
			
			
			CodePointer = CompiledNodeEval;
			Evaluator = this.gameObject.AddComponent<CsharpEvaluator>();
		}
		
		protected override Dictionary<string, object> CompiledNodeEval(Dictionary<string, object> inputstate, Dictionary<string, object> intermediateOutVals)
		{
			var output = intermediateOutVals;
			if (funcdef.MethodPointer == null)
			{
				throw new ArgumentException("method pointer cannot be null for loaded zt node");
			}
			if (funcdef.LoadedTypePointer == null)
			{
				throw new ArgumentException("type pointer cannot be null for loaded zt node");
			}
			
			//we just supply an array of the inputs, which are the variable added inputs
			//since this node was defined with a params attribute

			var keystoselect = Inputs.Select(x=>x.NickName).ToList();
			var inputportvals = keystoselect.Where(inputstate.ContainsKey)
				.Select(x => inputstate[x])
					.ToList();

			output["OUTPUT"] = call(funcdef,inputportvals.ToArray());
			(inputstate["done"] as Action).Invoke();
			return output;
			
		}

	//http://stackoverflow.com/questions/6484651/calling-a-function-using-reflection-that-has-a-params-parameter-methodbase
			private object call(FunctionDescription funcdef ,params object[] input)
		{
			var parameters = funcdef.Parameters;
				int lastParamPosition = parameters.Count - 1;
				
				object[] realParams = new object[parameters.Count];
				for (int i = 0; i < lastParamPosition; i++)
					realParams[i] = input[i];
				
				Type paramsType = parameters[lastParamPosition].ParameterType.GetElementType();
				Array extra = Array.CreateInstance(paramsType, input.Length - lastParamPosition);
				for (int i = 0; i < extra.Length; i++)
					extra.SetValue(input[i + lastParamPosition], i);
				
				realParams[lastParamPosition] = extra;
				
				input = realParams;
			
			
			return funcdef.MethodPointer.Invoke(null, input);
		}
		
	}
}
			
