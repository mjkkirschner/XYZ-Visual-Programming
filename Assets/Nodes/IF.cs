using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;
using Nodeplay.UI;

namespace Nodeplay.Nodes
{
	public class IF : NodeModel
	{
		
		
		protected override void Start()
		{
			base.Start();

			AddInputPort("test");
			
			AddExecutionInputPort("startTest");
			
			AddExecutionOutPutPort("TrueBranch");
			AddExecutionOutPutPort("ElseBranch");
			AddExecutionOutPutPort("done");
			
			Code = 
@"if test:
	TrueBranch();
else:
	ElseBranch();
done()";
		
			
			//Code = "for i in range(input1*2):" +Environment.NewLine +
			//		"\t"+"onIteration()" + Environment.NewLine+
			
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
		}
		
		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			tempUI.GetComponent<Renderer>().material.color = Color.cyan;
			tempUI.AddComponent<BoundingRenderer>();
			return tempUI;
			
		}
		
		
		
	}
}