using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System.Collections;
using UnityEngine.UI;
using Nodeplay.UI.Utils;
using UnityEngine.EventSystems;

/// <summary>
/// class that is responsible for defining how a node looks
/// this class instantiates gameobjects, geometry, and UI elements into the scene
/// it's possible that an entire node will be serialzed as a prefab and will not require
/// running the nodeview to be instantiated
/// </summary>
using Nodeplay.UI;


public class NodeView : BaseView<NodeModel>{
	    
    protected override void Start()
    {
        base.Start();
        Debug.Log("just started NodeView");
        
    }

    public void OnEvaluated(object sender, EventArgs e)
    {

        StartCoroutine(Blunk(Color.red,.1f));
        //subclass this component so we can just look for the output box
        //need to marshal or implement to_string per output type somehow
       // UI.GetComponentInChildren<Text>().text = Model.StoredValueDict.ToJSONstring();
    }

    public void OnEvaluation(object sender, EventArgs e)
    {
        StartCoroutine(Blink(Color.red,.1f));
    }

    public override void OnPointerUp(PointerEventData pointerdata)
    {
        // if we're connecting to this node, then add this node
        // to the target list of each of the nodes in the selection.

        Debug.Log("Mouse up event handler called");
       
    }
  
	//handler for ondrop events for nodes, when this occurs it might be because
	//we have dragged a portview and dropped on the node, we should create a list
	//of applicable ports that the user might have meant to drop onto and bring this
	//list of buttons up for a user to select from
	public override void OnDrop(PointerEventData pointerdata)
	{
		if(pointerdata.button == PointerEventData.InputButton.Left){
			//if we mouseUp over a port we need to check if we were connecting/dragging,
			// and then we'll instantiate a new connectorModel, the model will create it's own view
			// and the view will listen to its ports for property changes
			
			Debug.Log("Mouse just dropped on node");
			
			Debug.Log("I" + Model.name + " was just dropped on");
			
			var startport = pointerdata.pointerPress.GetComponent<PortModel>();
			if (startport != null)
			{
				List<PortModel>applicablePorts = new List<PortModel>();
				//if the starting port was a data input, then we'll need to get all the data outputs
				//on this node
				if (startport.PortType == PortModel.porttype.input && startport.GetType() == typeof(PortModel))
				{
					applicablePorts = Model.Outputs;
				}
				//if the starting port was a data output, then we'll need to get all the data inputs
				//on this node
				else if (startport.PortType == PortModel.porttype.output && startport.GetType() == typeof(PortModel))
				{
					applicablePorts = Model.Inputs;
				}
				//if the starting port was a execinput, then we'll need to get all the execoutputs
				//on this node
				else if (startport.PortType == PortModel.porttype.input && startport.GetType() == typeof(ExecutionPortModel))
				{
					applicablePorts = Model.ExecutionOutputs.Cast<PortModel>().ToList();
				}
				//if the starting port was a execOutput, then we'll need to get all the execinputs
				//on this node
				else if (startport.PortType == PortModel.porttype.output && startport.GetType() == typeof(ExecutionPortModel))
				{
					applicablePorts = Model.ExecutionInputs.Cast<PortModel>().ToList();
				}

				//now we have the list of applicable nodes, create a selection window
				//with buttons foreach port in the applicableports lists
				var copiedPointerData = new PointerEventData(EventSystem.current);
				copiedPointerData.pointerPress = pointerdata.pointerPress;
				copiedPointerData.pointerDrag = pointerdata.pointerDrag;
				createPortSelectionWindow(copiedPointerData,startport,applicablePorts);
				
				//Model.Owner.GraphOwner.AddConnection(pointerdata.pointerDrag.GetComponent<PortModel>(),this.Model);
			}
		}
	}
    
	private void createPortSelectionWindow(PointerEventData orginalpress,PortModel startport, List<PortModel> applicableports)
	{
		//load a vertical layout window that holds buttons for selecting ports
		//these buttons will just call a connect function on the portselection script which
		//sits on the portselection window, this object is setup with a list of ports
		//and the original port and takes care of the rest, nodeview doesnt know anything else...
		var prefab = Resources.Load<GameObject>("PortSelectionWindow");
		var portWindow = GameObject.Instantiate(prefab) as GameObject;
		portWindow.transform.localPosition = Vector3.zero;
		portWindow.transform.SetParent(this.transform);
		portWindow.GetComponent<PortSelectionManager>().init(orginalpress,startport,applicableports);
	}

}
