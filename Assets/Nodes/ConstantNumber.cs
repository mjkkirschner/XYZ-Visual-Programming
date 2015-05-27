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
	public class Constant : NodeModel
	{


		protected override void Start()
		{
			base.Start();
			AddOutPutPort("OUTPUT");
			Evaluator = this.gameObject.AddComponent<PythonEvaluator>();
			StoredValueDict = new Dictionary<string, object>();
			//create a handler that watches for name property changes on the 
			PropertyChanged += updatevar;
			NotifyPropertyChanged("Code");
			viewPrefabs = new List<string>(){"VariableNodeBaseView"};

		}
		
		private void updatevar(object sender, PropertyChangedEventArgs args){

			//if the code is updated, then reval and store the result here
			if (args.PropertyName ==  "Code")
			{
				Debug.Log("updating constant code val"); 
				Evaluate();
			}
		}
		
		public override GameObject BuildSceneElements()
		{
			ExposeVariableInNodeUI("Code",Code);
			var UI = base.BuildSceneElements();
			UI.GetComponent<Renderer>().material.color = new Color(56.0f/256.0f,158.0f/256.0f,201.0f/256.0f);
			return UI;
		}

	}
}