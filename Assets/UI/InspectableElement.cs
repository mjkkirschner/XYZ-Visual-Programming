using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nodeplay.UI
{
	/// <summary>
	/// TODO may design this class as generic so we can keep track of the type
	/// and have it work with a monobehavior based class that points here
	/// </summary>
	[RequireComponent(typeof(LayoutElement))]
	public class InspectableElement: UIBehaviour ,IPointerClickHandler
	{
		public NodeModel Model;
		public GameObject tempgeo;
		public Type ElementType;
		public object reference;
		// Use this for initialization
		protected override void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			throw new NotImplementedException();
		}
	}
}
