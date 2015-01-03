using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;

namespace Nodeplay.Nodes
{
	public class InstantiateCube : NodeModel
	{
		
		protected override void Start()
		{
			base.Start();

			AddOutPutPort("OUTPUT");
			AddInputPort("input1");
			AddInputPort("input2");
			AddExecutionInputPort("start");
			AddExecutionOutPutPort("VariableCreated");

			Code = "OUTPUT = unity.GameObject.CreatePrimitive(unity.PrimitiveType.Cube);" +
				"OUTPUT.transform.Translate(input1,input2,0);"
				+"VariableCreated()";
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
		}
		
		public override GameObject BuildSceneElements()
		{
			UIInputValueDict = new Dictionary<string, object>();
			UIInputValueDict.Add("Code",Code);
			return base.BuildSceneElements();

			
			
			
		}
		
		
		
	}
}
