using Nodeplay.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Nodeplay.UI{
	public class CameraMoveToSelection:MonoBehaviour
	{

	


	public void MoveToSelection(List<GameObject> selection)
	
	{
			var zoompos = calculateCentroid(selection.Select(x=>x.transform.localPosition).ToList());
			var offsettpos = zoompos + (this.gameObject.transform.right * 20f);
			
			//now calculate where the camera is currently looking
			var cameraviewpoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 30f));
			
			StartCoroutine(slowmove(this.transform.position,offsettpos,cameraviewpoint,zoompos,1f,this.gameObject));

	}

		//TODO this does not belong here
		private IEnumerator slowmove(Vector3 frompos,Vector3 topos,Vector3 lookFrom,Vector3 lookat, float duration,GameObject goToMove)
		{
			Debug.Log("moving camera");
			
			for (float f = 0; f <= duration; f = f + Time.deltaTime)
			{
				//move the camera towards the new node
				goToMove.transform.position = Vector3.Lerp(frompos, topos, f);
				
				goToMove.transform.LookAt(Vector3.Lerp(lookFrom,lookat,f));
				
				yield return null;
				
			}

	}

		private Vector3 calculateCentroid (List<Vector3>points)
		{
			Vector3 center = Vector3.zero;
			foreach (var point in points)
			{
				center = center + point;
			}
			
			if (points.Count<1)
			{
				Debug.Log("can't find centroid, no points");
				return Vector3.zero;
			}
			center = center / (points.Count);
			return center;
		}
	}
}
