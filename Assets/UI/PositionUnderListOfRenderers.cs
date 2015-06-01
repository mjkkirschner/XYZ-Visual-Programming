using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{
	
	
	public class PositionUnderListOfRenderers : MonoBehaviour
	{
		
		public List<GameObject> PostitionUnderThese = new List<GameObject>();
		public GameObject Model_GO;
		public Vector3 PostPositionTranslation = Vector3.zero;

		protected  void OnEnable()
		{

		}

		void Start()
		{
			var model = this.GetComponentInParent<NodeModel>();
			//subscribe to the model changes
			if (model == null)
			{
				return;
			}
			Model_GO = model.gameObject;
			Model_GO.GetComponent<NodeModel>().PropertyChanged += NodePropertyChangeEventHandler;

			//force a call to properychangehandelr
			NodePropertyChangeEventHandler(null,new PropertyChangedEventArgs("Location"));

		}
		
		/// this handler is used to respond to changes on the node
		// when the node is modified in some way we update the windows position
		public virtual void NodePropertyChangeEventHandler(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName != "Location")
			{
				return;
			}
			if (PostitionUnderThese.Count < 1)
			{
				return;
			}
			GenerateBounds(PostitionUnderThese);
			//this.gameObject.transform.Translate(PostPositionTranslation);

			
		}
		
		public virtual void GenerateBounds(List<GameObject> toBound)
		{
			Vector3 center = Vector3.zero;
			var allrenderers = toBound.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToList();
			if (allrenderers.Count <1)
			{
				return;
			}
			var totalBounds = allrenderers[0].bounds;
			foreach (Renderer ren in allrenderers)
			{
				center = center + ren.gameObject.transform.position;
				totalBounds.Encapsulate(ren.bounds);
			}

			//calculate the bounds of all the objects in the thing we're trying to move
			//so we can make sure they dont overlap

			Bounds totalBoundstoMove; 
			var toMoverenderers = this.GetComponentsInChildren<MeshRenderer>().ToList();
			if (toMoverenderers.Count >=1)
			{
				totalBoundstoMove = toMoverenderers[0].bounds;
				
				foreach(Renderer ren in this.GetComponentsInChildren<MeshRenderer>().ToList())
				{
					totalBoundstoMove.Encapsulate(ren.bounds);
				}

			}

			//try for a rect transform
			else
			{
				if (this.GetComponent<RectTransform>() != null)
				    {
					totalBoundstoMove = new Bounds(this.GetComponent<RectTransform>().transform.position
					                               ,new Vector3(0,this.GetComponent<RectTransform>().sizeDelta.y * this.transform.localScale.y,0));
					}
				else
				{
					//if nothing is here return
				return;
				}
			}
		
			center = center / (allrenderers.Count);


			var newPoint = center - new Vector3(0, totalBounds.size.y*.5f, 0);
			newPoint = newPoint - new Vector3(0, totalBoundstoMove.size.y,0);

			this.gameObject.transform.position = newPoint;
			
		}
		
		
		protected virtual void OnDestroy()
		{
			if (Model_GO == null)
			{
				return;
			}
			//Debug.Log("unsubscribing from nodemodel property changes");
			Model_GO.GetComponent<NodeModel>().PropertyChanged -= NodePropertyChangeEventHandler;

		}
		
	}
}