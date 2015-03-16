using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Nodeplay.Interfaces;
using System.ComponentModel;
using UnityEngine.EventSystems;
using Nodeplay.UI;
using System.Collections;
using UnityEngine.UI;

class PortView : BaseView<PortModel>
{
	
    protected override void Start()
    {
        base.Start();
		this.gameObject.AddComponent<EventConsumer>();
        PositionNewPort(this.gameObject);
		
    }

    /// <summary>
    /// a tempconnector that is drawn while dragging.
    /// </summary>
    private GameObject tempconnector;

   /// <summary>
   /// method that positions the port relative to the other ports
   /// </summary>
   /// <param name="port"></param>
   /// <returns></returns>
    public virtual GameObject PositionNewPort(GameObject port)
    {

        //bb of go

        //might need to be owner model - this might work for now as NodeView and Model should be the common base classes
        var view = this.Model.Owner.GetComponent<NodeView>();
        var boundingBox = view.UI.GetComponent<Renderer>().bounds;
        port.transform.parent = Model.Owner.transform;
        port.transform.position = Model.Owner.transform.position;
        // move the port from the center to back or front depending on port type

        float direction;

        if (port.GetComponent<PortModel>().PortType == PortModel.porttype.input)
        {
            direction = -4f;
        }
        else
        {
            direction = 4f;
        }

		

        port.transform.Translate(0, 0, boundingBox.size.z * direction);
        port.transform.localScale = new Vector3(.33f, .33f, .33f);

        // now we need to move the port in relation up or down to all other ports,
        // and possibly adjust other ports as well

        List<PortModel> ports;

        if (port.GetComponent<PortModel>().PortType == PortModel.porttype.input)
        {
			ports = Model.Owner.Inputs;
        }
        else
        {
			ports = Model.Owner.Outputs;
        }


        foreach (var currentport in ports)
        {
            var index = ports.IndexOf(currentport);
			var portRenderer = port.gameObject.GetComponentInChildren<Renderer>();
			//alternative algorithm, everytime we add a new port, we'll just iterate
			//each one by the renderer size of the port * c, stepping down, then we'll just rescale the y axis
			//of the nodeview UI so that it bounds all ports in the y axis...
			var stepsize = 6f;
           currentport.gameObject.transform.localPosition = new Vector3(currentport.gameObject.transform.localPosition.x,
                (portRenderer.bounds.size.y * stepsize) * ((float)index*-1) +(boundingBox.size.y/2) ,
            currentport.gameObject.transform.localPosition.z);
        }

        return port;
    }
    
	/// <summary>
	/// method that creates connections when dropping from a port to a port
	/// </summary>
	/// <param name="pointerdata"></param>
    public override void OnDrop(PointerEventData pointerdata)
    {
		if(pointerdata.button == PointerEventData.InputButton.Left){
		//if we mouseUp over a port we need to check if we were connecting/dragging,
		// and then we'll instantiate a new connectorModel, the model will create it's own view
		// and the view will listen to its ports for property changes
		
		Debug.Log("Mouse up event handler called");
		
		Debug.Log("I" + Model.NickName + " was just dropped on");
		
		var startport = pointerdata.pointerPress.GetComponent<PortModel>();
        if (startport != null)
        {
            if (startport.PortType == Model.PortType)
            {
                Debug.Log("breaking out, you cant attached two same direction connectors");
                return;
            }

			if (startport.GetType() != Model.GetType())
			{
				Debug.Log("can't connect execution connectors and data connectors");
				return;
			}

			//if the starting port was an output, then we'll need to reverse the connetion direction
			if (startport.PortType == PortModel.porttype.input)
			{
				Debug.Log("creating a reversed connection");
				Model.Owner.GraphOwner.AddConnection(this.Model, pointerdata.pointerDrag.GetComponent<PortModel>());
				return;
			}
            //TODO we must also look if we're about to create a cyclic dependencey, we should return a blank state

            // if port is already connected then disconnect old port before creating new connector
            if (Model.IsConnected)
            {	
					Model.Owner.GraphOwner.RemoveConnection(this.Model);
            }
			//if the port is a execution connector then we should remove the previous connection
			// because execution flow can't branch
			if (Model.GetType() == typeof(ExecutionPortModel) && startport.IsConnected)
			{
				startport.Owner.GraphOwner.RemoveConnection(startport.connectors[0].PEnd);
			}

				Model.Owner.GraphOwner.AddConnection(pointerdata.pointerDrag.GetComponent<PortModel>(),this.Model);
        }
   	 }
	}
    
	/// <summary>
	/// on pointerup clean the temp connector geometry
	/// </summary>
	/// <param name="pointerdata"></param>
	public override void OnPointerUp(PointerEventData pointerdata)
    {


        //handle this here for now:
        //destruction of temp connector
        if (tempconnector != null)
        {
            DestroyImmediate(tempconnector);
            tempconnector = null;
        }

        

    }

    //handler for dragging node event//
    //TODO there is a bug when dragging from an input to an output which destroys the other connectors from that output
    //fixes might include restricting connecting creation only if the we drag on outputs, and mouseup on inputs
    // or investingating why this is not working further, connector model is being destroyed but still refereneced
    public override void OnDrag(PointerEventData pointerdata)
    {
		if (pointerdata.button == PointerEventData.InputButton.Left){
            Vector3 to_point = ProjectCurrentDrag(dist_to_camera);

            if (tempconnector != null)
            {
                DestroyImmediate(tempconnector);
                tempconnector = null;
            }
            // since this is a port, we need to instantiate a new 
            //ConnectorView ( this is a temporary connector that we drag around in the UI)

            tempconnector = new GameObject("TempConnectorView");
            tempconnector.AddComponent<TempConnectorView>();
            tempconnector.GetComponent<TempConnectorView>().init(this.gameObject.transform.position, to_point);

        }

	}

}

