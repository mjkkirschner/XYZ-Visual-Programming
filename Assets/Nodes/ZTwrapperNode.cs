using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using System.Reflection;

namespace Nodeplay.Nodes
{
	public class ZTwrapperNode : DelegateNodeModel
	{
		//TODO might just be a string...
		protected List<ParameterInfo> parameters;
		protected MethodInfo methodPointer;
		protected Type loadedTypePointer;
		protected override void Start()
		{
			base.Start();
			
			//on start add a correctly named input port for each 
			//parameter in methodinfo that we've loaded

			foreach (var param in parameters)
			{
				AddInputPort(param.Name);
			}
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
			if (methodPointer == null)
			{
				throw new ArgumentException("method pointer cannot be null for loaded zt node");
			}
			if (loadedTypePointer == null)
			{
				throw new ArgumentException("type pointer cannot be null for loaded zt node");
			}

			output["OUTPUT"] = loadedTypePointer.InvokeMember(methodPointer.Name,BindingFlags.InvokeMethod, null, null, inputstate.Select(x => x.Value).ToArray(), null, null, inputstate.Select(y => y.Key).ToArray());
			(inputstate["done"] as Delegate).DynamicInvoke();
			return output;

		}

	}
}
