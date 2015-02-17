using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;
using System;
using Nodeplay.Core;

namespace Nodeplay.Nodes
{
	public class CreateVariable : NodeModel
	{
		public string variableName = "getsomenamefromUI";
		public object variable ="empty";
		protected override void Start()
		{
			base.Start();

			AddOutPutPort("OUTPUT");
			StoredValueDict = new Dictionary<string, object>();
			StoredValueDict["OUTPUT"] = new  VariableReference(()=> variable, val => {variable = val;});

		}

		public override GameObject BuildSceneElements()
		{
			return base.BuildSceneElements();


		}



	}
}
