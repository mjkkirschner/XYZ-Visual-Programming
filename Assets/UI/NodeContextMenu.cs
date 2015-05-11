using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nodeplay.UI
{
	public class NodeContextMenu : MonoBehaviour
	{
		private GameObject window;
		private BaseModel Model;
		private List<GameObject> buttons;
		
		// Use this for initialization
		void OnEnable ()
		{
			//Model = this.GetComponentInParent<BaseModel>();
			window = this.transform.Find("Window").gameObject;
			buttons = new List<GameObject>();
			window.transform.Find("Header/CloseButton").GetComponent<Button>().onClick.AddListener(()=>destroy());
		}
		
		public void init (List<Button> contextButtons)
		{
			foreach(var button in contextButtons)
			{
				button.transform.SetParent(window.transform,false);
				//add button to list of buttons
				buttons.Add(button.gameObject);
				button.onClick.AddListener(() => OnButtonPress());
			}
		}

		private void OnButtonPress()
		{
			Debug.Log("a button is being pressed");
			//so destroy the window
			GameObject.Destroy(this.gameObject);
			
		}
		
		public void destroy()
		{
			GameObject.Destroy(this.gameObject);
		}
		
	}
}
