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
	public class Comparison : NodeModel
	{
		
		protected override void Start()
		{
			base.Start();

			AddInputPort("x");
			AddInputPort("y");

			AddOutPutPort("boolean");
			AddExecutionInputPort("compare x to y");
			AddExecutionOutPutPort("compared");
			
			Code = "boolean = x>y;compared()";
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
			
		}
		
		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI ("Code",Code);
			
			return base.BuildSceneElements();
			
			
		}
		
		
		
	}
}