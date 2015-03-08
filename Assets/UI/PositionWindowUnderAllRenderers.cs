using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{


	public class PositionWindowUnderAllRenderers : MonoBehaviour
	{

		public GameObject Model_GO;

		void Start()
		{
			Model_GO = this.GetComponentInParent<NodeModel>().gameObject;
			//subscribe to the model changes
			Model_GO.GetComponent<NodeModel>().PropertyChanged += NodePropertyChangeEventHandler;
			//force a call to properychangehandelr
			NodePropertyChangeEventHandler(null,new PropertyChangedEventArgs("null"));
		}

		/// this handler is used to respond to changes on the node
		// when the node is modified in some way we update the windows position
		public virtual void NodePropertyChangeEventHandler(object sender, EventArgs args)
		{
			GenerateBounds(Model_GO.transform.Cast<Transform>().Select(t => t.gameObject).ToList());

		}

		public virtual void GenerateBounds(List<GameObject> toBound)
		{
			Vector3 center = Vector3.zero;
			var allrenderers = toBound.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToList();
			var totalBounds = allrenderers[0].bounds;
			foreach (Renderer ren in allrenderers)
			{
				center = center + ren.gameObject.transform.position;
				totalBounds.Encapsulate(ren.bounds);

			}
			center = center / (allrenderers.Count);
			var newPoint = center - new Vector3(0, totalBounds.size.y*.5f, 0);
			newPoint = newPoint + new Vector3(0,-3,0);
			this.gameObject.transform.position = newPoint;
			
		}
		

		protected virtual void OnDestroy()
		{
			//Debug.Log("unsubscribing from nodemodel property changes");
			Model_GO.GetComponent<NodeModel>().PropertyChanged -= NodePropertyChangeEventHandler;
		}
		
	}
}

