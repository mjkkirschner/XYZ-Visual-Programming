using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditor;

public class ProgramEntry : MonoBehaviour
{

		
	public List<GraphModel> workmodels = new List<GraphModel>();


	public void SaveGraph(){
		// call save on the current graphmodel
		var current = workmodels.Where(x=>x.Current == true).First();

		var path = EditorUtility.SaveFilePanel(
					"Save Graph As xml File",
					"",
					current.Name + ".xml",
					"xml");

		current.SaveGraphModel(path);
	}

	public void LoadGraph(){
		var path = EditorUtility.OpenFilePanel("Choose A Graph To Open","","xml");
		//create a new blank graphmodel
		//then call load on it with path, which will deserialze an xml file into that model
		var temp = new GraphModel("tempload");
		temp.LoadGraphModel(path);
		workmodels.Add(temp);
		var ls = GameObject.Find("LoadScreen");
		ls.SetActive(false);
	}

	public void NewGraph(){

		var model = new GraphModel("untitled"+workmodels.Count.ToString());
		//TODO remove this next line just for testing, this bool might be set when this model
		//is the assigned model of the canvas, or something like this, 
		//the graphmodel needs to set current when its loaded and displaying its nodes,
		// will most likely create a canvasview that sits on the camera and has an assigned graphmodel
		// which it will call instantiate on, and possibly other commands, current will be set from the canvas etc...

		model.Current = true;
		workmodels.Add(model);
		//hide the loadscreen
		var ls = GameObject.Find("LoadScreen");
		ls.SetActive(false);

		model.InstantiateNode<ForLoopTest> (new Vector3 (1, 1, 1));
		model.InstantiateNode<ForLoopTest>(new Vector3(2,2,1));
		model.InstantiateNode<DebugLogTest> (new Vector3 (2, 2, 2));
		model.InstantiateNode<DebugLogTest> (new Vector3 (3, 3, 3));
		model.InstantiateNode<StartExecution> (new Vector3 (0, 0, 0));
		model.InstantiateNode<InstantiateCube> (new Vector3 (0, 0, 0));


	}

		// Use this for initialization
		void Start ()
		{
			// on program start, we load a home screen into the main canvas 
			//var maincanvas = GameObject.Find("Canvas");
			
			//the home screen will have some callbacks here that create graphmodels
			// either by loading them and passing the string to parse back, or by creating a new one
			
		}



		void Update ()
		{

			
			
		}
		

	

		
}
