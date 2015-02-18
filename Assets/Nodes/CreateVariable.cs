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
	public class CreateVariable : NodeModel
	{
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

			AddOutPutPort("OUTPUT");
			StoredValueDict = new Dictionary<string, object>();
			//create a handler that watches for name property changes on the 
			PropertyChanged += updatevar;
			NotifyPropertyChanged("VariableName");
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
			return base.BuildSceneElements();


		}



	}
}
