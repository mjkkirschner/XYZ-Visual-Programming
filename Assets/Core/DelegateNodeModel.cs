using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
using Nodeplay.Engine;
using System.Linq;
using UnityEngine.UI;
using Nodeplay.UI;
using System.Xml;

public  class DelegateNodeModel : NodeModel
{

	public Func<Dictionary<string,object>,Dictionary<string,object>,Dictionary<string,object>> CodePointer {get;set;}

	protected virtual Dictionary<string,object> CompiledNodeEval(Dictionary<string,object> inputstate,Dictionary<string,object> intermediateOutVals)
	{
		return intermediateOutVals;
	}

	internal override void Evaluate()
	{
		OnEvaluation();
		//build packages for all data 
		var inputdata = gatherInputPortData();
		if (CodePointer == null){
			Debug.Break();
		}

		//i.e. For i in range(10):
		//triggers["iteration"]()
		//triggers["donewithiteration"]()
		var executiondata = gatherExecutionData();
		//here we use the codepointer instead of the code string
		var evalpackage = new EvaluationPackage(CodePointer,
		                                        inputdata.Select(x => x.First).ToList(),
		                                        inputdata.Select(x => x.Second).ToList(),
		                                        Outputs.Select(x=>x.NickName).ToList(),
		                                        executiondata);
		var outvar = Evaluator.Evaluate(evalpackage);
		this.StoredValueDict = outvar;
		OnEvaluated();
		
	}
		
}

