using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;

namespace Nodeplay.Nodes
{
	public class StartExecution : NodeModel
	{
		
		
		protected override void Start()
		{
			base.Start();
			Code = "Start()";
			AddExecutionOutPutPort("Start");
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();

		}
		
		
		
		
		
	}
}

