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
		

			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
		}
		
		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			tempUI.GetComponent<Renderer>().material.color = Color.cyan;
			tempUI.AddComponent<BoundingRenderer>();
			tempUI.GetComponent<BoundingRenderer> ().initialze (new List<int> (){0,1}, 
			new List<Color>() {Color.cyan,new Color(.7f,.4f,.1f)});
			return tempUI;
			
		}
		
		
		
	}
}