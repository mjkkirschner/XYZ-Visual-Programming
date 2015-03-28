using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Nodeplay.Interfaces;
using System.ComponentModel;
using UnityEngine.EventSystems;
using Nodeplay.UI;



    class ExecutionPortView:PortView
    {


		 /// <summary>
   /// method that positions the executionport relative to the other ports
   /// </summary>
   /// <param name="port"></param>
   /// <returns></returns>
    public override GameObject PositionNewPort(GameObject port)
    {

      
        var view = this.Model.Owner.GetComponent<NodeView>();
        var boundingBox = view.UI.GetComponent<Renderer>().bounds;
        port.transform.parent = Model.Owner.transform;
        port.transform.position = Model.Owner.transform.position;
        // move the port from the center to back or front depending on port type
		// also rotate it by 90 degrees
        float direction;

        if (port.GetComponent<PortModel>().PortType == PortModel.porttype.input)
        {
            direction = -1* boundingBox.size.z/2;
        }
        else
        {
			direction = boundingBox.size.z/2;
        }

		
		port.transform.localScale = new Vector3(.33f, .33f, .33f);

        port.transform.Translate(0, 0, direction,Space.Self);
		if (direction<0)
		{
			port.transform.Rotate(new Vector3(0,180,0));
		}
        // now we need to move the port in relation up or down to all other execution ports,
        // and possibly adjust other ports as well

        List<ExecutionPortModel> ports;

        if (port.GetComponent<PortModel>().PortType == PortModel.porttype.input)
        {
            ports = Model.Owner.ExecutionInputs.ToList();
        }
        else
        {
			ports = Model.Owner.ExecutionOutputs.ToList();
        }


        foreach (var currentport in ports)
        {
            var index = ports.IndexOf(currentport);
			var portRenderer = port.gameObject.GetComponentInChildren<Renderer>();
			
			var stepsize = 1f;
           currentport.gameObject.transform.localPosition = new Vector3(currentport.gameObject.transform.localPosition.x,
				(portRenderer.bounds.size.y * stepsize*5) * ((float)index * -1) - (boundingBox.size.y / 2),
            currentport.gameObject.transform.localPosition.z);
			//TODO use the index to set something on the portview that stretches part of the compoenent...
			//---
			//-----
			//-------
        }

        return port;
    }

		


    }

