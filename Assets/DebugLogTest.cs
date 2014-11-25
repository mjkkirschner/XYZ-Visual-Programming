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
			//AddOutPutPort("OUTPUT");
			//AddInputPort("input1");
			AddExecutionInputPort("start");
			AddOutPutPort("OUTPUT");
			//AddExecutionOutPutPort("done");

			
			Code = "OUTPUT = 'we were iterated';print('blahblahblah')";
			
			
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
		}
		
		
		
		
		
	}
}
