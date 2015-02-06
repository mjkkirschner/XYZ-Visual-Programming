using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;

namespace Nodeplay.Nodes
{
    public class Number : NodeModel
    {
		
        protected override void Start()
        {
            base.Start();

            AddOutPutPort("OUTPUT");

			AddExecutionInputPort("start");
			AddExecutionOutPutPort("VariableCreated");

			Code = "OUTPUT = 5;VariableCreated()";
            Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
        }

		public override GameObject BuildSceneElements()
		{
			if (UIInputValueDict == null)
			{
				UIInputValueDict = new Dictionary<string, object>();
				UIInputValueDict.Add("Code", Code);
			}
			return base.BuildSceneElements();

			
		}



    }
}
