using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nodeplay.UI
{
		public class PortSelectionManager : MonoBehaviour
		{
		private GameObject window;
		public PortModel StartPort {get;private set;}
		public List<PortModel> ApplicablePorts {get;private set;}
		private List<GameObject> buttons;

				// Use this for initialization
				void OnEnable ()
				{
					window = this.transform.Find("Window").gameObject;
					buttons = new List<GameObject>();
				}
	
		public void init (PointerEventData originalPressEvent,PortModel startport, List<PortModel> applicableports)
				{
				//TODO do null checks
				StartPort = startport;
				ApplicablePorts = applicableports;

				//now generate buttons for each port in the applicable ports
				foreach(var port in ApplicablePorts)
					{
					var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
					button.transform.SetParent(window.transform,false);
					//add button to list of buttons
					buttons.Add(button);
					//capture the port
					var portCopy = port;
					button.GetComponentInChildren<Text>().text = port.NickName;
				button.GetComponent<Button>().onClick.AddListener(() => OnButtonPress(originalPressEvent,portCopy));

					}
				}
		//handler that is raised when some button is in the list of ports is clicked
		//we need to create a connection from the start port to the port represented by the button...
			private void OnButtonPress(PointerEventData originalEvent,PortModel port)
		{
			Debug.Log("sending an event to a port");
			//trigger an event on the port we would like to connect to
			port.gameObject.GetComponent<PortView>().OnDrop(originalEvent);
			//then destroy the pointerSelectionWindow!
			GameObject.Destroy(this.gameObject);

		}

		public void destory()
		{
			GameObject.Destroy(this.gameObject);
		}

	}
}

