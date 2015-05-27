using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{
	
	/// <summary>
	/// this class positions a group of mesh renderers under the bounds of rect in worldspace
	/// </summary>
	public class PositionWindowUnderRect : MonoBehaviour
	{
		
		public RectTransform PostitionUnderThese;
		public GameObject Model_GO;
		public Vector3 PostPositionTranslation = Vector3.zero;
		

		void Start()
		{
			var model = this.transform.root.GetComponent<NodeModel>();
			//subscribe to the model changes
			if (model == null)
			{
				return;
			}
			Model_GO = model.gameObject;
			Model_GO.GetComponent<NodeModel>().PropertyChanged += NodePropertyChangeEventHandler;
			
			//force a call to properychangehandelr
			NodePropertyChangeEventHandler(null,new PropertyChangedEventArgs("null"));
			
		}
		
		/// this handler is used to respond to changes on the node
		// when the node is modified in some way we update the renderers position
		public virtual void NodePropertyChangeEventHandler(object sender, EventArgs args)
		{
			if (PostitionUnderThese == null){
				throw new Exception("rect to position under is null");
			}
			GenerateBounds(PostitionUnderThese);
			this.gameObject.transform.Translate(PostPositionTranslation);
			
			
		}
		
		public virtual void GenerateBounds(RectTransform positionUnder)
		{
			var rectSize = positionUnder.sizeDelta;
			var rectCenter = positionUnder.transform.parent.position;
			//calculate the bounds of all the objects in the thing we're trying to move
			//so we can make sure they dont overlap
			var toMoverenderers = this.GetComponentsInChildren<MeshRenderer>().ToList();
			if (toMoverenderers.Count <1)
			{
				return;
			}
			var totalBoundstoMove = toMoverenderers[0].bounds;
			
			foreach(Renderer ren in this.GetComponentsInChildren<MeshRenderer>().ToList())
			{
				totalBoundstoMove.Encapsulate(ren.bounds);
			}

			
			var newPoint = rectCenter - new Vector3(0, rectSize.y*.5f * positionUnder.parent.transform.localScale.y , 0);
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