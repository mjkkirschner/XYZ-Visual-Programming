using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEditor;
using System.ComponentModel;
using Nodeplay.Engine;
using Nodeplay.Core;

namespace Nodeplay.Engine {

public class AppModel : MonoBehaviour
{
	public Dictionary<string, FunctionDescription> LoadedFunctions { get; set; }
	public List<GraphModel> WorkModels {get;set;}
	private List<Type> _loadedNodeModels;
	public CustomNodeManager CollapsedCustomGraphNodeManager;
	//TODO implement observable collection list
	public List<Type> LoadedNodeModels
		
	{
		get
		{
			return this._loadedNodeModels;
			
		}
		
		set
		{
			if (value != this._loadedNodeModels)
			{
				this._loadedNodeModels = value;
				NotifyPropertyChanged("LoadadNodes");
			}
		}
	}
	public Library NodeLibrary {get;set;}


	public event PropertyChangedEventHandler PropertyChanged;
	protected virtual void NotifyPropertyChanged(String info)
	{
		Debug.Log("sending " + info + " change notification");
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
	}

	 
	/// <summary>
	/// method that polls the app for the current graph model, also ensures there is only one current
	/// </summary>
	/// <returns>The graph model.</returns>
	public GraphModel GetCurrentGraphModel()
	{ 
		var currentGraphs = WorkModels.Where(x=>x.Current == true).ToList();
				if (currentGraphs.Count>1)
					{
				Debug.Log("Somehow there is more than one current graph...this should not be possible");
				Debug.Break();
			return null;
					}
				else
					{
			return currentGraphs.First();
					}
		}
	
	public void SaveGraph(){
		// call save on the current graphmodel
		var current = WorkModels.Where(x=>x.Current == true).First();

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
		var temp = new GraphModel("tempload",this);
		temp.LoadGraphModel(path);
		WorkModels.Add(temp);
		var ls = GameObject.Find("LoadScreen");
		ls.SetActive(false);
		temp.Current = true;

	}

	public void NewGraph(){

		var model = new GraphModel("untitled"+WorkModels.Count.ToString(),this);
		//TODO remove this next line just for testing, this bool might be set when this model
		//is the assigned model of the canvas, or something like this, 
		//the graphmodel needs to set current when its loaded and displaying its nodes,
		// will most likely create a canvasview that sits on the camera and has an assigned graphmodel
		// which it will call instantiate on, and possibly other commands, current will be set from the canvas etc...

		model.Current = true;
		WorkModels.Add(model);
		//hide the loadscreen
		var ls = GameObject.Find("LoadScreen");
		ls.SetActive(false);

		//model.InstantiateNode<ForLoopTest> (new Vector3 (1, 1, 1));
		//model.InstantiateNode<DebugLogTest> (new Vector3 (2, 2, 2));
		model.InstantiateNode<StartExecution> (new Vector3 (0, 0, 0));
		model.InstantiateNode<InstantiateCube> (new Vector3 (0, 0, 0));
		model.InstantiateNode<Number> (new Vector3 (3, 0, 0));
		//model.InstantiateNode<Number> (new Vector3 (3, 0, 0));
		//model.InstantiateNode<csharpsum> (new Vector3 (3, 0, 0));

	}

		// Use this for initialization
		void Start ()
		{
			WorkModels = new List<GraphModel>();
			LoadedNodeModels = new List<Type>();
			LoadedFunctions = new Dictionary<string, ZeroTouchFunctionDescription>();
			//create a nodeModelloader for this instance of appmodel
			var nodeloaderinst = new NodeModelLoader();
			var ZTnodeloaderinst = new ZTsubsetLoader();
			// on program start, we load a home screen into the main canvas 
			//var maincanvas = GameObject.Find("Canvas");
			//the home screen comtains, run, save(possibly), and the library component
			
			//TODO for now find the object, but we should load it here instead
			var homescreen = GameObject.Find("HomeCanvas");
		 	NodeLibrary = homescreen.GetComponentInChildren<Library>();
			this.PropertyChanged += NodeLibrary.HandleAppModelChanges;

			LoadedNodeModels = nodeloaderinst.LoadNodeModels("Nodes",true);
			Debug.Log("loaded "+ LoadedNodeModels.Count.ToString() + " nodes");

			LoadedNodeModels = LoadedNodeModels.Concat(ZTnodeloaderinst.LoadNodeModels("ZTNodes",false)).ToList();
			Debug.Log("loaded " + LoadedNodeModels.Count.ToString() + " nodes");

			LoadedFunctions = LoadedFunctions.Concat(ZTnodeloaderinst.functions).ToDictionary(x => x.Key, x => x.Value);
			Debug.Log("loaded " + LoadedFunctions.Keys.Count.ToString() + " function pointers");
			//the load screen will have some callbacks here that create graphmodels
			// either by loading them and passing the string to parse back, or by creating a new one
			
		}



		void Update ()
		{

			
			
		}
		

	

	}
}
