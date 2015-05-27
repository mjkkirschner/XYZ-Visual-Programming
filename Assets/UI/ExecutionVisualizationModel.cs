using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
using System.Linq;

public class ExecutionVisualizationModel : BaseModel
{
	protected List<string> viewPrefabs = new List<string>();
	protected override void Start()
	{
		base.Start();
		var view = this.gameObject.AddComponent<VisualzationView>();
		//gather types from outputs of the node... unclear if we really want this...
		//they'll all just be the same
		foreach (var type in this.transform.root.GetComponent<NodeModel>().Outputs.Select(x => x.ObjectType))
		{
			viewPrefabs.Add(type.Name);
		}
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
	/*	GameObject UI = null;
		foreach (var viewPrefab in viewPrefabs)
		{
			if (Resources.Load(viewPrefab) == null)
			{
				//if the resource doesnt exist, bail
				continue;
			}
			//if this is first prefab
			if (viewPrefabs.IndexOf(viewPrefab) == 0)
			{
				UI = Instantiate(Resources.Load(viewPrefab)) as GameObject;
				UI.transform.localPosition = this.gameObject.transform.position;
				UI.transform.parent = this.gameObject.transform;
				UI.transform.localScale = new Vector3(.3f, .3f, .3f);
			}
			else
			{
				GameObject subui = Instantiate(Resources.Load(viewPrefab)) as GameObject;
				var offset = subui.transform.position - UI.transform.localPosition;
				subui.transform.localPosition = this.gameObject.transform.position;
				subui.transform.parent = (UI.transform);
				subui.transform.Translate(offset);
			}


		}
		return UI;
*/
	}
	
	
	
	
	
	
	
	
	
	
	
}