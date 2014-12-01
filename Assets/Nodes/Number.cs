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
			InputValueDict = new Dictionary<string, object>();
			InputValueDict.Add("Code",Code);
			return base.BuildSceneElements();


			
		}



    }
}
