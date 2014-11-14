using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;

namespace Nodeplay.Nodes
{
		public class TestNodeType : NodeModel
		{
	
				
				 protected override void Start ()
				{
						base.Start ();
						AddOutPutPort ();
						AddInputPort ("input1");
                        AddInputPort("input2");
                        Code = "OUTPUT = range(input1,input2*2);print OUTPUT";
                        Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
				}
	
				
	
	
	
		}
}
