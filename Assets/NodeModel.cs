using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
using Nodeplay.Engine;
using System.Linq;
public class NodeModel : BaseModel
{
    //todo probably will need to readd location properties if I want to support the non-graph based workflows...$$$

		//possibly we store a list of connectors that we keep updated
		// from ports, will need to add events on ports
		public List<PortModel> Inputs { get; set; }
		public List<PortModel> Outputs { get; set; }
        private System.Object storedvalue;
        public System.Object StoredValue
        {
            get
            {
                return this.storedvalue;

            }

            set
            {
                if (value != storedvalue)
                {
                    this.storedvalue = value;
                    NotifyPropertyChanged("StoredValue");
                }
            }
        }
    //TODO rename rethink
        public string Code { get; set; }    
    
		public MonoBehaviour Evaluator;

		protected override void Start ()
		{
				
				Debug.Log ("just started NodeModel");
                this.gameObject.AddComponent<NodeView>();
                StoredValue = null;
				Inputs = new List<PortModel> ();
				Outputs = new List<PortModel> ();
               
		}
        

		public void AddInputPort (string name = null)
		{
				// for now lets just add a child sphere - 
				// add a portmodel component to that sphere
				var newport = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				
				newport.AddComponent<PortModel> ();
                // initialze the port
                newport.GetComponent<PortModel>().init(this, Outputs.Count, PortModel.porttype.input, name);

                //hookup the ports listener to the nodes propertychanged event, and hook
                // handlers on the node back from the ports connection events
                this.PropertyChanged += newport.GetComponent<PortModel>().NodePropertyChangeEventHandler;
                newport.GetComponent<PortModel>().PortConnected += PortConnected;
                newport.GetComponent<PortModel>().PortDisconnected += PortDisconnected;
                Inputs.Add(newport.GetComponent<PortModel>());    

                //PositionNewPort(newport);   
                //add the current port to the list of inputs on this node
				
				
                
				

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

    public override GameObject BuildSceneElements()
    {
        //unsure on this design, for now we just attached the loaded or new geometry as the child of the
        // root gameobject

        //TODO broken, this implementation does not work for two reasons
        //1. the nodeview/nodemodel are looking for mouse events to be forwarded only to
        //their gameobjects directly, not any children, so we wont get any events on the children nodes
        //possibly something to fix, we may want to forward all events down the chain of children until they get used
        //2. the prefab needs to be instantiated at the location of the root GO or moved to the creation point.
        
        // the base node implementation is to load the basenodeview prefab and set it as child of the root go
        
        GameObject UI = Instantiate(Resources.Load("NodeBaseView")) as GameObject;
        UI.transform.localPosition = this.gameObject.transform.position;
        UI.transform.parent = this.gameObject.transform;
        return UI;
     


    }
    /// <summary>
    /// method that gathers port names and evaluated values from connected nodes
    /// </summary>
    /// <returns></returns>
    private List<Tuple<string, System.Object>> gatherInputPortData()
    {
        var output = new List<Tuple<string, System.Object>>();
        foreach (var port in Inputs)
        {   Debug.Log("gathering input port data on node" + name);
            var portInputPackage =  Tuple.New(port.NickName, port.connectors[0].PStart.Owner.StoredValue);
            Debug.Log("created a port package"+portInputPackage.First + ":" + portInputPackage.Second.ToString());
            output.Add(portInputPackage);
        }
        return output;

    }

    //this points to evaluation engine or some delegate
    internal void Evaluate()
    {
      //build packages for all data
        
        var inputdata = gatherInputPortData();
        var outvar = ((PythonEvaluator)Evaluator).Evaluate(Code,inputdata.Select(x => x.First).ToList(), inputdata.Select(x => x.Second).ToList());
        this.StoredValue = outvar;

    }

    
}
