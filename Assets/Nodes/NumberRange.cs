using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;

namespace Nodeplay.Nodes
{
    public class NumberRange : NodeModel
    {


        protected override void Start()
        {
            base.Start();
			AddExecutionInputPort("generateRange");
			AddExecutionOutPutPort("generated");
            AddOutPutPort("range");
            AddInputPort("start");
            AddInputPort("end");
			Code = "range = range(start,end);generated();";
            Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
        }





    }
}
