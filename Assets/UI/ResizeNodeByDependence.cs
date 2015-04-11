using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Nodeplay.UI
{
	
	public class ResizeNodeByDependence : MonoBehaviour
	{
		
		public NodeModel Model;
		public NodeView View;
		private bool initialzed = false;
		// Use this for initialization
		void Start()
		{
			View = this.GetComponentInParent<NodeView>();
			Model = this.GetComponentInParent<NodeModel>();
			Model.PropertyChanged += resize;
		}

		void resize(object sender, PropertyChangedEventArgs args)
			{

			int counter= 1;
			if (args.PropertyName == "Ports")
			{
				//count each connected output, and then count how many connectors are on each port
				// sum all of these and scale the UI by this number...
				foreach (var output in Model.Outputs)
				{
					foreach (var connector in output.connectors)
					{
						counter += 1;
					}
				}
			
				float veccomponent = counter/2;
				veccomponent = Mathf.Clamp(veccomponent,1.0f,5.0f);

				View.ModifySelectable(Vector3.one  * veccomponent ,Vector3.one * veccomponent);
			
			}

			}


	}
	
}