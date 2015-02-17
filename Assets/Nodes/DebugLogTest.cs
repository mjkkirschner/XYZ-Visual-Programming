using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using System;

namespace Nodeplay.Nodes
{
	public class DebugLogTest : NodeModel
	{
		
		protected override void Start()  
		{
			base.Start();
			
			AddInputPort("message");
			AddExecutionInputPort("start");
			AddOutPutPort("OUTPUT");
			AddExecutionOutPutPort("done");

			
			Code = "OUTPUT = message ;print(message); done();";
			
			
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
		}
		

		
		
		
	}
}
