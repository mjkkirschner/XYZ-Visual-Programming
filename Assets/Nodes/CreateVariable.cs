using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;
using System;
using Nodeplay.Core;
using System.ComponentModel;

namespace Nodeplay.Nodes
{
	public class CreateVariable : NodeModel,IContextable
	{
		private int lastUpdateFrame = 0;
		private GameObject vizroot;
		public object variable ="empty";
		private string variablename;
		public string VariableName
		{
			get
			{
				return this.variablename;
				
			}
			
			set
			{
				if (value != variablename)
				{
					this.variablename = value;
					NotifyPropertyChanged("VariableName");
				}
			}
		}

		protected override void Start()
		{
			base.Start();

			//explicitGraphExecution.Evaluating += updateVisualization;

			AddOutPutPort("OUTPUT");
			StoredValueDict = new Dictionary<string, object>();
			//create a handler that watches for name property changes on the 
			PropertyChanged += updatevar;
			NotifyPropertyChanged("VariableName");
			viewPrefabs = new List<string>(){"VariableNodeBaseView"};


		}

		private void updatevar(object sender, PropertyChangedEventArgs args){

			if (args.PropertyName ==  "VariableName")
				{
				Debug.Log("just created a variable named: " + VariableName); 
				StoredValueDict["OUTPUT"] = new  VariableReference(()=> variable, val => {variable = val;},VariableName );
				}
		}

		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("VariableName",VariableName);
			var UI = base.BuildSceneElements();
			UI.GetComponent<Renderer>().material.color = new Color(56.0f/256.0f,158.0f/256.0f,201.0f/256.0f);
			vizroot = new GameObject("vizroot");
			vizroot.transform.SetParent(UI.transform);
			vizroot.AddComponent<InspectorVisualization>();
			return UI;
		}


		private void updateVisualization(){
		//this is deleting everything everyframe
			//TODO MAKE IT FASTER!

		if(Time.frameCount > lastUpdateFrame){

		var childrenofVisualization = vizroot.GetComponent<InspectorVisualization>().transform.Cast<Transform>().ToList();
		if (childrenofVisualization.Count > 0){
				childrenofVisualization.Where(x=>x.CompareTag("visualization")).ToList().ForEach(x=>DestroyImmediate(x.gameObject));
		}
		transform.root.GetComponentInChildren<InspectorVisualization>().PopulateTopLevel(
				(StoredValueDict["OUTPUT"] as VariableReference).Get() );
				lastUpdateFrame = Time.frameCount;
			}
		}

		#region IContextable implementation
		
		public List<Button> RequestContextButtons ()
		{
			var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
			button.GetComponentInChildren<Text>().text = "Toggle Inspector";
			button.GetComponentInChildren<Button>().onClick.AddListener(() => {populateInspector(vizroot,new List<object>(){ (StoredValueDict["OUTPUT"] as VariableReference).Get()});} );
			return new List<Button>(){button.GetComponentInChildren<Button>()};
		}
		
		#endregion


		
	
	
	private void populateInspector(GameObject evalparent, List<object> vals){
		//if the rootwrapper exists
		if (evalparent.transform.childCount > 0)
		{
			//then delete it
				var inspector = evalparent.transform.Cast<Transform>().ToList();
			inspector.ForEach(x=>GameObject.Destroy(x.gameObject));
		}
		else
			{
				//else populate another one
		evalparent.GetComponent<InspectorVisualization>().PopulateTopLevel(vals,0);
			}
		}
	}
}
