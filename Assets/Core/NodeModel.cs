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
using System.Xml;
using UnityEngine.EventSystems;
using Nodeplay.Nodes;

public class NodeModel : BaseModel
{
	protected List<string> viewPrefabs =  new List<string>(){"NodeBaseView"};
	protected override void NotifyPropertyChanged (string info)
	{
		base.NotifyPropertyChanged (info);
		
		//also update the model if this property is in the dict
		//for now just force a rebuild...
		//TODO we may also want to update the dictionary if the property has changed on the model...
		//
		UpdateModelProperties(UIInputValueDict);
	}

	//add a indexer to nodemodels, this allows getting property by name, so we can lookup
	//propertie from the input dict if its modififed, and then change properties on the model
	// might be more useful to look into creating bindings with proper c# classes
	public object this[string propertyName]
	{
		get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
		set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
	}
    //possibly we store a list of connectors that we keep updated
    // from ports, will need to add events on ports
    public List<PortModel> Inputs { get; set; }
    public List<PortModel> Outputs { get; set; }
	public List<ExecutionPortModel> ExecutionInputs {get;set;}
	public List<ExecutionPortModel> ExecutionOutputs {get;set;}
	//adding a string key to the inputvaluedict
	// will search the node model for that property and expose it in the UI
	private Dictionary<string,System.Object> uiinputvalueDict;
	public Dictionary<string,System.Object> UIInputValueDict
	
	{
		get
		{
			return this.uiinputvalueDict;
			
		}
		
		set
		{
			if (value != uiinputvalueDict)
			{
				this.uiinputvalueDict = value;
				//update all properties in dict
				UpdateModelProperties(uiinputvalueDict);
				NotifyPropertyChanged("InputValues");
			}
		}
	}

	//stored value dictionary is the output value dictionary of finished computation
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
	public GraphModel GraphOwner {get;internal set;}
    //events for callbacks to view during and after nodemodel evaluation
    public delegate void EvaluationHandler(object sender, EventArgs e);
    public delegate void EvaluatedHandler(object sender, EventArgs e);
    public event EvaluationHandler Evaluation;
    public event EvaluatedHandler Evaluated;

	private string code;
    public string Code
	{
		get
		{
			return this.code;
			
		}
		
		set
		{
			if (value != code)
			{
				this.code = value;
				NotifyPropertyChanged("code");
			}
		}
	}


    public Evaluator Evaluator;

	protected void UpdateModelProperties(Dictionary<string,object> inputdict){

		foreach(var entry in inputdict){
			this[entry.Key] = entry.Value;
		}
	}

	protected ExplicitGraphExecution explicitGraphExecution {
		get;
		set;
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		Inputs = new List<PortModel>();
		Outputs = new List<PortModel>();
		ExecutionInputs = new List<ExecutionPortModel>();
		ExecutionOutputs = new List<ExecutionPortModel>();
	}

    protected override void Start()
    {
		base.Start();
        Debug.Log("just started starting! NodeModel");
        var view = this.gameObject.AddComponent<NodeView>();
        Evaluated += view.OnEvaluated;
        Evaluation += view.OnEvaluation;
        StoredValueDict = null;
        
		explicitGraphExecution = GameObject.FindObjectOfType<ExplicitGraphExecution> ();
		//this dictionary might be loaded when the node is laoded 
		//and we dont want to erase it
		if (UIInputValueDict == null){
			Debug.Log("the UIinputdict has not been loadead, creating an empty one");
			UIInputValueDict = new Dictionary<string, object>();
		}
		Debug.Log("just finished starting NodeModel");
    }

	public void AddExecutionInputPort(string name = null)
	{
		//TODO this should create an empty gameobject and port view should create its own UIelements
		var newport = new GameObject();
		newport.AddComponent<ExecutionPortModel>();
		// initialze the port
		newport.GetComponent<ExecutionPortModel>().init(this, ExecutionInputs.Count, PortModel.porttype.input, name);
		
		//hookup the ports listener to the nodes propertychanged event, and hook
		// handlers on the node back from the ports connection events
		this.PropertyChanged += newport.GetComponent<ExecutionPortModel>().NodePropertyChangeEventHandler;
		newport.GetComponent<ExecutionPortModel>().PortConnected += PortConnected;
		newport.GetComponent<ExecutionPortModel>().PortDisconnected += PortDisconnected;
		ExecutionInputs.Add(newport.GetComponent<ExecutionPortModel>());
		
		
	}

	public void AddExecutionOutPutPort(string portName)
	{
		
		var newport = new GameObject();
		newport.AddComponent<ExecutionPortModel>();
		// initialze the port
		newport.GetComponent<ExecutionPortModel>().init(this, ExecutionOutputs.Count, PortModel.porttype.output, portName);
		var currentPort = newport.GetComponent<ExecutionPortModel>();
		// registers a listener on the port so it gets updates about the nodes property changes
		// we use this to let the port notify it's attached connectors that they need to update
		this.PropertyChanged += currentPort.NodePropertyChangeEventHandler;
		newport.GetComponent<ExecutionPortModel>().PortConnected += PortConnected;
		newport.GetComponent<ExecutionPortModel>().PortDisconnected += PortDisconnected;
		ExecutionOutputs.Add(currentPort);
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
	/// Removes the input port named name.
	/// </summary>
	/// <param name="name">Name.</param>
	protected virtual void RemoveInputPort (string name)
	{
		if (Inputs.Where(x=>x.NickName == name).ToList().Count>1)
		{
			throw new Exception("more than two input ports with name we want to remove");
		}
		var toRemove = new List<PortModel>();
		foreach(var port in Inputs)
		{
			if (port.NickName == name)
			{
				if (port.IsConnected)
				{
					port.Owner.GraphOwner.RemoveConnection(port);
				}
				toRemove.Add(port);
				Destroy(port.gameObject);
			}
		}
		toRemove.ForEach(x=>Inputs.Remove(x));
		//actually need to update the gameobjects representing these or send events that take care of this
	}

	/// <summary>
	/// Removes the output port named name.
	/// </summary>
	/// <param name="name">Name.</param>
	protected virtual void RemoveOutputPort (string name)
	{
		if (Outputs.Where(x=>x.NickName == name).ToList().Count>1)
		{
			throw new Exception("more than two outputs ports with name we want to remove");
		}
		var toRemove = new List<PortModel>();
		foreach(var port in Outputs)
		{
			if (port.NickName == name)
			{
				if (port.IsConnected)
				{
					port.Owner.GraphOwner.RemoveConnection(port);
				}
				toRemove.Add(port);
				Destroy(port.gameObject);
			}
		}
		toRemove.ForEach(x=>Inputs.Remove(x));
		//actually need to update the gameobjects representing these or send events that take care of this
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

    public void PortConnected(object sender, ConnectorModel e)
    {
        Debug.Log("I " + this.GetType().Name+  " just got a port connected event on " + (sender as PortModel).NickName );

    }

    public void PortDisconnected(object sender, ConnectorModel e)
    {
		Debug.Log("I " + this.GetType().Name+  " just got a port DISconnected event on " + (sender as PortModel).NickName);
    }


	/// <summary>
	/// If node is connected to some other node(other than Output) then it is not a 'top' node
	/// </summary>
	public bool IsTopMostDataNode
	{
		get
		{
			if (Outputs.Count < 1)
				return false;
			
			foreach (var port in Outputs.Where(port => port.connectors.Count != 0))
			{
				return port.connectors.Any(connector => connector.PEnd.Owner is Output);
			}
			
			return true;
		}
	}

    public override GameObject BuildSceneElements()
    {
        //unsure on this design, for now we just attached the loaded or new geometry as the child of the
        // root gameobject
        // the base node implementation is to load the basenodeview prefab and set it as child of the root go
		this.gameObject.AddComponent<PositionNodeRelativeToParents>();

		GameObject UI = null;
		foreach (var viewPrefab in viewPrefabs){

			//if this is first prefab
			if (viewPrefabs.IndexOf(viewPrefab) == 0)
			{	UI = Instantiate(Resources.Load(viewPrefab)) as GameObject;
				UI.transform.localPosition = this.gameObject.transform.position;
				UI.transform.parent = this.gameObject.transform;
			}
			else
			{
				GameObject subui = Instantiate(Resources.Load(viewPrefab)) as GameObject;
				var offset = subui.transform.position - UI.transform.localPosition;
				subui.transform.localPosition = this.gameObject.transform.position;
				subui.transform.parent = (UI.transform);
				subui.transform.Translate(offset);
			}
        		
      
		}

		//add a new toggleDisplay that will contain both the output and input panel
		// now load the outputview
		var nodedisplay = new GameObject();
		nodedisplay.transform.localPosition = this.gameObject.transform.position;
		nodedisplay.transform.parent = UI.transform.parent;

		nodedisplay.AddComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
		nodedisplay.AddComponent<HorizontalLayoutGroup>();
		nodedisplay.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.UpperCenter;
		nodedisplay.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;

		//load the togglepanel that will point to the outputpanel and toggle it
		var togglepanel = Instantiate(Resources.Load("ToggleLabel")) as GameObject;
		togglepanel.transform.localPosition = this.gameObject.transform.position;
		//translate position down to the bottom of the bounds of nodeview
		//togglepanel.transform.Translate(0,-3* UI.renderer.bounds.size.y, 0);
		togglepanel.transform.SetParent(this.transform,false);
		togglepanel.transform.localPosition = new Vector3(
			togglepanel.transform.localPosition.x,
			//TODO something strange here
			 UI.GetComponent<Renderer>().bounds.min.y*10,
			togglepanel.transform.localPosition.z);

		togglepanel.AddComponent<TogglePanelButton>();
		var tpb = togglepanel.GetComponentInChildren<TogglePanelButton>();
		togglepanel.GetComponentInChildren<Toggle>().onValueChanged.AddListener(delegate { tpb.ToggleContentFitMethod(nodedisplay); });
		togglepanel.AddComponent<UILabel>();
		//togglepanel.AddComponent<EventConsumer>();

		//set the correct settings for this canvas renderer
		togglepanel.GetComponent<Canvas>().worldCamera = Camera.main;


		nodedisplay.transform.SetParent(togglepanel.transform.FindChild("Window").transform,false);

        // now load the outputview
        var output = Instantiate(Resources.Load("OutputWindow")) as GameObject;
		output.transform.localScale = Vector3.one;
		output.AddComponent<LayoutElement>().minWidth = 100;
		//output.GetComponent<UIWindowBase>().enabled = false;
        output.transform.localPosition = this.gameObject.transform.position;
		output.transform.SetParent(nodedisplay.transform, false);
		output.transform.localPosition = Vector3.zero;
        this.PropertyChanged+= output.AddComponent<OutDisplay>().HandleModelChanges;
        
		//add input window for this node...may not add one... this maybe should be per node override
		var input = Instantiate(Resources.Load("InputWindow")) as GameObject;
		input.transform.localScale = Vector3.one;
		input.AddComponent<LayoutElement>().minWidth = 100;
		//input.GetComponent<UIWindowBase>().enabled = false;
		input.transform.localPosition = this.gameObject.transform.position;
		input.transform.SetParent(nodedisplay.transform, false);
		input.transform.localPosition = Vector3.zero;
		this.PropertyChanged+= input.AddComponent<InputDisplay>().HandleModelChanges;                

		//add a coordinate system object
		var CSgumball = Resources.Load("CoordinateSystem") as GameObject;
		var csgo = GameObject.Instantiate(CSgumball);
		csgo.transform.SetParent(this.transform,false);
		csgo.transform.Translate(0,2,0);


        //iterate all graphics casters and turn blocking on for 3d objects
		var allcasters = this.GetComponentsInChildren<GraphicRaycaster>().ToList();
		allcasters.ForEach(x=>x.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD);

		//UI.AddComponent<Light>().type = LightType.Point;
		//UI.GetComponent<Light>().range = 40;
		//UI.GetComponent<Light>().intensity = .4f;
		//UI.GetComponent<Light>().color = Color.white;
		return UI;



    }

	protected void ExposeVariableInNodeUI (string variable,object value)
	{
		if (UIInputValueDict != null) {
			// The Add method throws an exception if the new key is  
			// already in the dictionary. 
			try {
				UIInputValueDict.Add (variable, value);
			}
			catch (ArgumentException) {
				Debug.Log ("An element with this key, " +variable +  " already exists on the node" + ", " +
				           "most likely, it was loaded from a saved file");
			}
		}
	}

    /// <summary>
    /// method that gathers port names and evaluated values from connected nodes
    /// </summary>
    /// <returns></returns>
    protected List<Tuple<string, System.Object>> gatherInputPortData()
    {
        var inputdata = new List<Tuple<string, System.Object>>();
        foreach (var port in Inputs)
        {
            //Debug.Log("gathering input port data on node" + name);
            //TODO instead of looking for the owners stored value we either need to look at the stored value of
            // at the port, or storedValue will be a dictionray of output port values where we can index in using 
            // some index, not sure what index we'll have... we need to support multiple outs from one port
            // going to multiple inputs on another port, so it needs to be the index of the output port
            // on the parent node
            
            //TODO add a check for the storedvaluedict returning key exception 
            var outputConnectedToThisInput = port.connectors[0].PStart;
            var portInputPackage = Tuple.New(port.NickName, outputConnectedToThisInput.Owner.StoredValueDict[outputConnectedToThisInput.NickName]);
            //Debug.Log("created a port package " + portInputPackage.First + " : " + portInputPackage.Second.ToString());
            inputdata.Add(portInputPackage);
        }
        return inputdata;

    }
	/// <summary>
	///method gathers delegates from the executionoutput ports, these can be called
	/// by any evaluator to trigger these outputs at the correct time during eval.
	/// </summary>
	/// <returns>The execution data.</returns>
	protected List<Tuple<string,Action>> gatherExecutionData ()
	{
		var outputTriggers = new List<Tuple<string,System.Action>>();
		foreach (var trigger in ExecutionOutputs){
		
			//TODO somehow when creating this execution package we'll look at the connector at the index,
			// this connector type will contain the type of yeildinstruction to use for the task
			int indexCopy = trigger.Index;

			var currentTask = explicitGraphExecution.CurrentTask;
			var currentNode = this;

			Action storeMethodOnStack = () => {
				//Debug.Log("we're currently executing a closure which points to some task insertion");
				//Debug.Log("index copy is" + indexCopy.ToString()); 
				//Debug.Log("currentTask is" + currentTask.ID); 
				//Debug.Log("currentNode is" + currentNode.name); 
                 var currentVariablesOnModel = Evaluator.PollScopeForOutputs(Outputs.Select(x => x.NickName).ToList());
                 explicitGraphExecution.TaskSchedule.InsertTask(
                 new Task(currentTask, currentNode, indexCopy, new Action(() => CallOutPut(indexCopy, currentVariablesOnModel)), new WaitForEndOfFrame()));
            };
			var outputPackage = Tuple.New<string,System.Action>(trigger.NickName,storeMethodOnStack);
			outputTriggers.Add(outputPackage);
			//Debug.Log("gathering trigger delegate on node " + name +", this will call method named " +trigger.NickName+ "at: " + trigger.Index);

		}
		return outputTriggers;
	}



	public void CallOutPut(int index, Dictionary<string,object> intermediateVariableModelValues)
	{

		//Debug.Log("trying to get the output on " + this + "at index " + index);
		var trigger = this.ExecutionOutputs[index];
		//Debug.Log(ExecutionOutputs[index].NickName +  " is the output we found at that index");
		if (trigger.IsConnected){
		//Debug.Log("this trigger was connected");
       // Debug.Log("about to trigger execution on downstream node, gathering existing output variables from the saved scope");
        this.StoredValueDict = intermediateVariableModelValues;

		var nextNode = trigger.connectors[0].PEnd.Owner;
		//Debug.Log("about to evaluate " + nextNode);

			nextNode.Evaluate();

		}
	}

	/*public void CallOutPut(string outputName){
	foreach(var outtrigger in ExecutionOutputs){
		if (outtrigger.NickName == outputName){
				outtrigger.connectors.First().PEnd.Owner.Evaluate();
				break;
			}
		}
	}
*/
    protected virtual void OnEvaluation()
    {
        //Debug.Log("sending a evaluation state change");
        if (Evaluation != null)
        {
            Evaluation(this, EventArgs.Empty);
        }
    }

    protected virtual void OnEvaluated()
    {
        //Debug.Log("sending a evaluation state change");
        if (Evaluated != null)
        {
            Evaluated(this, EventArgs.Empty);
        }
    }

	protected virtual void OnNodeModified(){
	}

    //this points to evaluation engine or some delegate
	internal virtual void Evaluate()
	{
		OnEvaluation();
        //build packages for all data 
        var inputdata = gatherInputPortData();
		if (Code == null){
			Debug.Break();
		}

       //build packages for output execution triggers, these
		// are tuples that connect an execution output string to a delegate
		// which calls the eval method on the next node
		// the idea is to call these outputs appropriately when needed from the code
		// or script defind by the node

		//i.e. For i in range(10):
					//triggers["iteration"]()
				//triggers["donewithiteration"]()
		var executiondata = gatherExecutionData();
		var evalpackage = new EvaluationPackage(Code,
		                                        inputdata.Select(x => x.First).ToList(),
		                                        inputdata.Select(x => x.Second).ToList(),
		                                        Outputs.Select(x=>x.NickName).ToList(),
		                                        executiondata);
        var outvar = Evaluator.Evaluate(evalpackage);
		this.StoredValueDict = outvar;
        OnEvaluated();

    }
	//base save implementation iterates all members of the inputdict and saves their element 
	public virtual void Save(XmlDocument doc, XmlElement element)
	
	{
		var nodeData = doc.CreateElement("nodeinputdict");
		element.AppendChild(nodeData);
		if (UIInputValueDict != null)
		{
			foreach (var pair in UIInputValueDict)
			{
				nodeData.SetAttribute(pair.Key, pair.Value.ToString());
			}
		}


	}

	public virtual void Load(XmlNode node)
	{
		XmlNode inputdata = null ;
		
		foreach(XmlNode subnode in node.ChildNodes)
		{
			if (subnode.Name == "nodeinputdict")
			{
				inputdata = subnode;

			}
			
		}

		if (inputdata != null)
		{
			UIInputValueDict = new Dictionary<string, object>();
			foreach (XmlAttribute attribute in inputdata.Attributes)
			{
				Debug.Log("loading " + attribute.Name +" : "+ attribute.Value.ToString());
				
				UIInputValueDict.Add(attribute.Name,attribute.Value);
			}
			//force the notify property change to fire
			//TODO observable dictionary please
			var dictcopy = new Dictionary<string, object>(UIInputValueDict);
			UIInputValueDict = dictcopy;
		}
		
		
	}
}
