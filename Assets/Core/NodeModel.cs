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
using UnityEngine.UI;
using Nodeplay.UI;

public class NodeModel : BaseModel
{
    //todo probably will need to readd location properties if I want to support the non-graph based workflows...$$$

    //possibly we store a list of connectors that we keep updated
    // from ports, will need to add events on ports
    public List<PortModel> Inputs { get; set; }
    public List<PortModel> Outputs { get; set; }
    private Dictionary<string,System.Object> storedvaluedict;
    public Dictionary<string,System.Object> StoredValueDict
    {
        get
        {
            return this.storedvaluedict;

        }

        set
        {
            if (value != storedvaluedict)
            {
                this.storedvaluedict = value;
                NotifyPropertyChanged("StoredValue");
            }
        }
    }
    //events for callbacks to view during and after nodemodel evaluation
    public delegate void EvaluationHandler(object sender, EventArgs e);
    public delegate void EvaluatedHandler(object sender, EventArgs e);
    public event EvaluationHandler Evaluation;
    public event EvaluatedHandler Evaluated;


    //TODO rename rethink
    public string Code { get; set; }

    public MonoBehaviour Evaluator;

    protected override void Start()
    {

        Debug.Log("just started NodeModel");
        var view = this.gameObject.AddComponent<NodeView>();
        Evaluated += view.OnEvaluated;
        Evaluation += view.OnEvaluation;
        StoredValueDict = null;
        Inputs = new List<PortModel>();
        Outputs = new List<PortModel>();

    }


    public void AddInputPort(string name = null)
    {
       //TODO this should create an empty gameobject and port view should create its own UIelements
        var newport = new GameObject();
        newport.AddComponent<PortModel>();
        // initialze the port
        newport.GetComponent<PortModel>().init(this, Inputs.Count, PortModel.porttype.input, name);

        //hookup the ports listener to the nodes propertychanged event, and hook
        // handlers on the node back from the ports connection events
        this.PropertyChanged += newport.GetComponent<PortModel>().NodePropertyChangeEventHandler;
        newport.GetComponent<PortModel>().PortConnected += PortConnected;
        newport.GetComponent<PortModel>().PortDisconnected += PortDisconnected;
        Inputs.Add(newport.GetComponent<PortModel>());


    }
    /// <summary>
    /// Adds an output portmodel and geometry to the node.
    /// also updates the outputs array
    /// </summary>
    public void AddOutPutPort(string portName)
    {

        var newport = new GameObject();
        newport.AddComponent<PortModel>();
        // initialze the port
        newport.GetComponent<PortModel>().init(this, Outputs.Count, PortModel.porttype.output, portName);
        var currentPort = newport.GetComponent<PortModel>();
        // registers a listener on the port so it gets updates about the nodes property changes
        // we use this to let the port notify it's attached connectors that they need to update
        this.PropertyChanged += currentPort.NodePropertyChangeEventHandler;
        newport.GetComponent<PortModel>().PortConnected += PortConnected;
        newport.GetComponent<PortModel>().PortDisconnected += PortDisconnected;
        Outputs.Add(currentPort);
    }

    public void PortConnected(object sender, EventArgs e)
    {
        Debug.Log("I just got a port connected event");

    }

    public void PortDisconnected(object sender, EventArgs e)
    {
        Debug.Log("I just got a port disconnected event");
    }

    public override GameObject BuildSceneElements()
    {
        //unsure on this design, for now we just attached the loaded or new geometry as the child of the
        // root gameobject
        // the base node implementation is to load the basenodeview prefab and set it as child of the root go

        GameObject UI = Instantiate(Resources.Load("NodeBaseView")) as GameObject;
        UI.transform.localPosition = this.gameObject.transform.position;
        UI.transform.parent = this.gameObject.transform;

        // now load the outputview
        var output = Instantiate(Resources.Load("OutputWindowText")) as GameObject;
        output.transform.localPosition = this.gameObject.transform.position;
        output.transform.parent = UI.transform.parent;
        this.PropertyChanged+= output.AddComponent<OutDisplay>().HandleModelChanges;
        
        //iterate all graphics casters and turn blocking on for 3d objects
		var allcasters = UI.GetComponentsInChildren<GraphicRaycaster>().ToList();
		allcasters.ForEach(x=>x.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD);
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
        {
            Debug.Log("gathering input port data on node" + name);
            //TODO instead of looking for the owners stored value we either need to look at the stored value of
            // at the port, or storedValue will be a dictionray of output port values where we can index in using 
            // some index, not sure what index we'll have... we need to support multiple outs from one port
            // going to multiple inputs on another port, so it needs to be the index of the output port
            // on the parent node
            
            //TODO add a check for the storedvaluedict returning key exception 
            var outputConnectedToThisInput = port.connectors[0].PStart;
            var portInputPackage = Tuple.New(port.NickName, outputConnectedToThisInput.Owner.StoredValueDict[outputConnectedToThisInput.NickName]);
            Debug.Log("created a port package" + portInputPackage.First + ":" + portInputPackage.Second.ToString());
            output.Add(portInputPackage);
        }
        return output;

    }

    protected virtual void OnEvaluation()
    {
        Debug.Log("sending a evaluation state change");
        if (Evaluation != null)
        {
            Evaluation(this, EventArgs.Empty);
        }
    }

    protected virtual void OnEvaluated()
    {
        Debug.Log("sending a evaluation state change");
        if (Evaluated != null)
        {
            Evaluated(this, EventArgs.Empty);
        }
    }


    //this points to evaluation engine or some delegate
    internal void Evaluate()
    {
        OnEvaluation();
        //build packages for all data
        
        var inputdata = gatherInputPortData();
        //TODO this does not currently let us supply more than one output per node...
        // since the entire stored value is extracted from the node and transfered
        var outvar = ((PythonEvaluator)Evaluator).Evaluate(Code, inputdata.Select(x => x.First).ToList(), inputdata.Select(x => x.Second).ToList(), Outputs.Select(x=>x.NickName).ToList());
        this.StoredValueDict = outvar;
        OnEvaluated();
    }


}
