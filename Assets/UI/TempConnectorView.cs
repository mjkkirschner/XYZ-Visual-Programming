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
			
            dist_to_camera = Vector3.Distance(this.gameObject.transform.position, Camera.main.transform.position);
            NodeManager = GameObject.FindObjectOfType<NodeManager>();

			NormalScale = new Vector3(.2f, .2f, .2f);
			HoverScale = new Vector3(.2f, .2f, .2f);

            Debug.Log("just started TempViewConnector");
            started = true;
        }
    }
}
