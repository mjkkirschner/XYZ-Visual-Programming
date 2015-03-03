using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;
using System;

namespace Nodeplay.UI
{


	public class PositionWindowAboveAllRenderers : PositionWindowUnderAllRenderers
	{

		public override void GenerateBounds(List<GameObject> toBound)
		{
			Vector3 campos = Camera.main.transform.position;
			Vector3 center = Vector3.zero;
			var allrenderers = toBound.SelectMany(x => x.GetComponentsInChildren<MeshRenderer>()).ToList();
			var totalBounds = allrenderers[0].bounds;
			foreach (Renderer ren in allrenderers)
			{
				center = center + ren.gameObject.transform.position;
				totalBounds.Encapsulate(ren.bounds);

			}
			center = center / (allrenderers.Count);
			var vectorTowardsCam = (campos - center ).normalized;
			var newPoint = center + (vectorTowardsCam * totalBounds.size.z);
			this.gameObject.transform.position = newPoint;
			
		}

		
	}
}

