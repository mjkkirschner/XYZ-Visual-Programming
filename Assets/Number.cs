using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
namespace Nodeplay.Nodes
{
    public class Number : NodeModel
    {


        protected override void Start()
        {
            base.Start();
            AddOutPutPort();
            Code = "OUTPUT = 5";
            StoredValue = 5;
           Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
        }





    }
}
