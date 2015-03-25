using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Nodeplay.UI
{
    class TempConnectorView:ConnectorView
    {
        protected override void Start()
        {
			geometryToRepeat = Resources.Load<GameObject>("connector_sub");
            dist_to_camera = Vector3.Distance(this.gameObject.transform.position, Camera.main.transform.position);
      
			NormalScale = new Vector3(.2f, .2f, .2f);
			HoverScale = new Vector3(.2f, .2f, .2f);

            Debug.Log("just started TempViewConnector");
            started = true;
        }

		public void init (Vector3 startpoint, Vector3 endpoint, GameObject geoToRepeat = null)
		{
			if (!started)
			{
				Start();
			}

			if (geoToRepeat != null){
				geometryToRepeat = geoToRepeat;
			}
			redraw (startpoint, endpoint,geometryToRepeat);
			TemporaryGeometry.Select(x => x.transform.parent = this.gameObject.transform);
			
		}
    }
}
