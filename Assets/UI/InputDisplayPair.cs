using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.EventSystems;
using Nodeplay.UI.Utils;


namespace Nodeplay.UI
{
	
	class InputDisplayPair : UIBehaviour
	{
		private InputDisplay inputowner;
		public String InputName { get; set; }
		private String inputString;
		private Text nameControl;
		private InputField valueControl;
		bool started = false;
		
		protected override void Start()
		{
			//find the text controls by name
			inputowner = this.gameObject.GetComponentInParent<InputDisplay>();
			nameControl = transform.FindChild("input name").GetComponent<Text>();
			valueControl = transform.FindChild("InputField").GetComponent<InputField>();
			//register a listener on this inputpair so we can react to changes in the inputfield
			//TODO figure out how to generalize this for any ui element with onValueChange callbacks
			valueControl.gameObject.GetComponent<InputField>().onValueChange.AddListener(this.InputChangeHandler);
			started = true;

		}

		public void InputChangeHandler ( string text){
			//TODO really need to use implementation of observable dictionary...
			var dictcopy =  new Dictionary<string,object>(inputowner.model.UIInputValueDict);
			dictcopy[InputName] = text;
			inputowner.model.UIInputValueDict = dictcopy;
			Debug.Log("modifying inputdict on model");
			Debug.Log(InputName +" : " + text);
		}



		
		public void UpdateLabels(string name, object value)
		{
			if (!started){
				Start();
			}
			Debug.Log(name);
			Debug.Log(value);
			nameControl.text = name;
			inputString = (value is string) ? (string)value: value.ToJSONstring();
			valueControl.text = inputString;
		}
		
		
		
	}
}
