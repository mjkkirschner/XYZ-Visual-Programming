using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{
	
	
	public class PositionWindowUnderPorts : MonoBehaviour
	{
		
		public GameObject Model_GO;
		public List<GameObject> allchildren = new List<GameObject>();

		void Start()
		{
			Model_GO = this.GetComponentInParent<BaseModel>().gameObject;
			//subscribe to the model changes
			Model_GO.GetComponent<BaseModel>().PropertyChanged += NodePropertyChangeEventHandler;
			//force a call to properychangehandelr
			NodePropertyChangeEventHandler(null,new PropertyChangedEventArgs("null"));
		}
		
		/// this handler is used to respond to changes on the node
		// when the node is modified in some way we update the windows position
		public void NodePropertyChangeEventHandler(object sender, PropertyChangedEventArgs args)
		{
			 allchildren = Model_GO.transform.Cast<Transform>().Select(t => t.gameObject).ToList();

			//check what kind of children these are, we want to find the node object or it's ports
			//not visualizations
			var baseModels = allchildren.Select(x => x.GetComponent<BaseModel>()).ToList();
			//we have a bunch of basemodels, we can remove the first
			if (baseModels.Count >1){

				//allchildren.Remove(Model_GO.transform.GetChild(0).gameObject);
			}
			else{
				allchildren.Add(Model_GO);
			}

			allchildren.Remove(this.gameObject);
			GenerateBounds(allchildren);
		}

		
		public void GenerateBounds(List<GameObject> toBound)
		{
			Vector3 center = Vector3.zero;
			var allrenderers = toBound.Select(x => {
				if( x.GetComponent<MeshRenderer>() !=null)
				{
					return x.GetComponent<MeshRenderer>();
				}
				return null;
			}).ToList();

			var totalBounds = allrenderers[0].bounds;
			allrenderers.RemoveAll(item => item == null);
			foreach (Renderer ren in allrenderers)
			{ 
				center = center + ren.gameObject.transform.position;
				totalBounds.Encapsulate(ren.bounds);
				
			}
			center = center / (allrenderers.Count);
			var newPoint = center - new Vector3(0, totalBounds.size.y*1.5f, 0);
			this.gameObject.transform.position = newPoint;
			
		}

		void OnDestroy()
		{	if (Model_GO){
			// if the gameobject hosting this window is destroyed we need to unsubscribe to this event
			Model_GO.GetComponent<BaseModel>().PropertyChanged -= NodePropertyChangeEventHandler;
			}
		}
		
	}
}