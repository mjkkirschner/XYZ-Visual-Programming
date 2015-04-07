using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;
using System;

namespace Nodeplay.Nodes
{
	public class PythonNode : VariableInputOutputNodeModel
	{
		protected override string GetPortName ()
		{
			return "input" + Inputs.Count.ToString();
		}
		
		protected override void Start()
		{
			base.Start();
			
			AddOutPutPort("OUTPUT0");
			
			AddExecutionInputPort("start");
			AddExecutionOutPutPort("run");
			
			Code = "OUTPUT0 = range(1);run()";
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
			
		}
		
		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI ("Code",Code);
			
			return base.BuildSceneElements();
			
			
		}

	}
}