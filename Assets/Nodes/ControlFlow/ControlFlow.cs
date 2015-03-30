using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;

namespace Nodeplay.Nodes
{
	public abstract class ControlFlowNodeModel : NodeModel
	{

		protected override void Start()
		{
			base.Start();
			viewPrefabs = new List<string>(){"ControlFlowNodeBaseView"};
		}


		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			tempUI.GetComponent<Renderer>().material.color = Color.cyan;
			return tempUI;
			
		}
		
		
		
	}
}

