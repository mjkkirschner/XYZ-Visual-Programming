using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;

public class ExecutionVisualizationModel : BaseModel
{

	protected override void Start()
	{
		base.Start();
		var view = this.gameObject.AddComponent<VisualzationView>();
	}
	

	/// <summary>
	/// this code will either be a method or expression for generating UI elements
	/// it may also point to UI prefab data to load
	/// the viewmodel will execute this code, it is stored here to avoid needing custom views
	/// for each element type with similar interaction logic
	/// </summary>
	public override GameObject BuildSceneElements()
	{

		var x = GameObject.CreatePrimitive(PrimitiveType.Cube);
		this.gameObject.AddComponent<MeshFilter>().mesh = x.GetComponent<MeshFilter>().mesh;
		this.gameObject.AddComponent<MeshRenderer>();
		this.gameObject.GetComponent<Renderer>().material.color  = Color.red;
		this.gameObject.AddComponent<BoxCollider>();
		GameObject.Destroy(x);
		return this.gameObject;

	}
	
	
	
	
	
	
	
	
	
	
	
}