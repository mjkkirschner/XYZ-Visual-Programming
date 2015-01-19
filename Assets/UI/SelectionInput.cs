using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Nodeplay.UI
{
	class SelectionInput:MonoBehaviour
	{
		public void Update()
		{
			if (Input.GetKey(KeyCode.Delete))
			{
				var currently_selected = GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject;
				var appmodel = GameObject.FindObjectOfType<AppModel>();
				var currentGraph = appmodel.WorkModels.Where(x => x.Current == true).First();
				//send a delete element command to the current graph model
				//it's possible currently selected go is not in the current graph model
				currentGraph.DeleteNode(currently_selected);

			}
		}
	}
}
