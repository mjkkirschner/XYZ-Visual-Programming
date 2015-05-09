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
		protected FunctionDescription funcdef;
		//TODO might just be a string...
		protected override void Start()
		{
			base.Start();
			
			//on start go grab the correct information from the appmodel's loadedFunctions dict
			// use the name of this type as the key into the dictionary
			funcdef = GameObject.FindObjectOfType<AppModel>().LoadedFunctions[this.GetType().FullName];

			//on start add a correctly named input port for each 
			//parameter in methodinfo that we've loaded

			foreach (var param in funcdef.Parameters)
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
			viewPrefabs.Add(funcdef.LoadedTypePointer.Name);
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

			//Debug.Log("about to call method:" + funcdef.MethodPointer.Name + "on original type:" + funcdef.LoadedTypePointer); 

			var keystoselect = funcdef.Parameters.Select(x=>x.Name).ToList();
	          var inputportvals = keystoselect.Where(inputstate.ContainsKey)
	          .Select(x => inputstate[x])
	          .ToList();
			 
			//TODO throwing errors :(
			output["OUTPUT"] = funcdef.MethodPointer.Invoke(null,inputportvals.ToArray());
			(inputstate["done"] as Action).Invoke();
			return output;

		}

	}
}
