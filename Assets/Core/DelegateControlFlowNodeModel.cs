using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;

public abstract class ControlFlowDelegateNodeModel : DelegateNodeModel
{

	protected override void Start()
	{
		base.Start();
		viewPrefab = "ControlFlowNodeBaseView";
	}

	public override GameObject BuildSceneElements()
	{
		var tempUI = base.BuildSceneElements();
		tempUI.GetComponent<Renderer>().material.color = Color.cyan;
		return tempUI;
		
	}

	
}