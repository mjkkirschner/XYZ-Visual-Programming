using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
public class NodeModel : BaseModel
{
    //todo probably will need to readd location properties if I want to support the non-graph based workflows...$$$

		//possibly we store a list of connectors that we keep updated
		// from ports, will need to add events on ports
		public List<PortModel> Inputs { get; set; }
		public List<PortModel> Outputs { get; set; }
		
		

		protected override void Start ()
		{
				
				Debug.Log ("just started NodeModel");

				Inputs = new List<PortModel> ();
				Outputs = new List<PortModel> ();

		}

		// node should not know about targets or connecting etc, this is for ports to
		// implement

		/*
		public ReadOnlyCollection<NodeSimple> Targets {
				get {
						return targets.AsReadOnly ();
				}
		}



		public void removeTarget (NodeSimple target)
		{
				if (targets.Contains (target)) {
						targets.Remove (target);
				}

				return;
		}

		public void ConnectTo (NodeSimple target)
		{		
				if (targets.Contains (target) || target == this) {
						return;
				}

				targets.Add (target);
		}
*/
		


		public void AddInputPort ()
		{
				// for now lets just add a child sphere - 
				// add a portmodel component to that sphere
				var newport = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				//need a position method that arranges the port depending on how many exist in the outs
				newport.transform.position = this.gameObject.transform.position;
				newport.transform.parent = this.gameObject.transform;
				newport.transform.Translate (0.0f, 0.0f, -1.2f);
				newport.transform.localScale = new Vector3 (.33f, .33f, .33f);
				newport.AddComponent<PortModel> ();
		
				var currentPort = newport.GetComponent<PortModel> ();
				Inputs.Add (currentPort);
				currentPort.init (this, Outputs.Count, PortModel.porttype.input);

				
				this.PropertyChanged += currentPort.NodePropertyChangeEventHandler;	
				newport.GetComponent<PortModel>().PortConnected += PortConnected;
				newport.GetComponent<PortModel>().PortDisconnected += PortDisconnected;

		}
		/// <summary>
		/// Adds an output portmodel and geometry to the node.
		/// also updates the outputs array
		/// </summary>
		public void AddOutPutPort ()
		{
				// for now lets just add a child sphere - 
				// add a portmodel component
				var newport = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				//need a position method that arranges the port depending on how many exist in the outs
				newport.transform.position = this.gameObject.transform.position;
				newport.transform.parent = this.gameObject.transform;
				newport.transform.Translate (0.0f, 0.0f, 1.2f);
				newport.transform.localScale = new Vector3 (.33f, .33f, .33f);
				newport.AddComponent<PortModel> ();

				var currentPort = newport.GetComponent<PortModel> ();
				Outputs.Add (currentPort);
				currentPort.init (this, Outputs.Count, PortModel.porttype.output);


				// registers a listener on the port so it gets updates about the nodes property changes
				// we use this to let the port notify it's attached connectors that they need to update
				this.PropertyChanged += currentPort.NodePropertyChangeEventHandler;
				newport.GetComponent<PortModel>().PortConnected += PortConnected;
				newport.GetComponent<PortModel>().PortDisconnected += PortDisconnected;
		}
		
		public void PortConnected(object sender, EventArgs e){
		Debug.Log("I just got a port connected event");

	}

	public void PortDisconnected(object sender, EventArgs e){
		Debug.Log("I just got a port disconnected event");
	}


		

}
