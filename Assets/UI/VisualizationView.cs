using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Nodeplay.UI;

public class VisualzationView :BaseView<BaseModel>
{
	protected override void Start()
	{   //TODO contract for hierarchy
		// we always search the root gameobject of this view for the model,
		// need to enforce this contract somehow, I think can use requires component.
		Model = this.gameObject.GetComponent<BaseModel>();
		
		dist_to_camera = Vector3.Distance(this.gameObject.transform.position, Camera.main.transform.position);
		// nodemanager manages nodes - like a workspacemodel
		
		UI = Model.BuildSceneElements();
		var renderers = UI.GetComponentsInChildren<Renderer>();
		
		originalcolors = renderers.ToDictionary(x=>x.gameObject,x=>x.material.color);
		

		//Debug.Log("just started viz view");
		started = true;
		
	}

}