using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine.EventSystems;
using System.Linq;
using Nodeplay.Nodes;
using System.Xml;
using System.Reflection;
using System.Collections;

/// <summary>
/// initially modified from Dynamo's source version .74
/// https://github.com/DynamoDS/Dynamo/blob/master/src/DynamoCore/Models/WorkspaceModel.cs
/// </summary>


public class GraphModel : INotifyPropertyChanged, IPointerClickHandler
{
	private AppModel _appmodel;
	public List<NodeModel> Nodes { get; set; }
	public List<ConnectorModel> Connectors { get; set; }
	//TODO confusion with name, filename, etc
	public string Name { get; set; }
	public float X { get; set; }
	public float Y { get; set; }
	public float Z { get; set; }
	public bool HasUnsavedChanges { get; set; }
	public DateTime LastSaved { get; set; }
	public bool Current { get; set; }
	public string FileName { get; set; }


	public static GraphModel ByXmlFile(string filepath,AppModel appmodel)
	{
		//TODO parse the filepath, load the file here, and then call graphmodel constructor
		// which will deserialze
		return new GraphModel("loaded",appmodel);
	}

	public GraphModel(String name,AppModel appmodel)
	{

		Name = name;

		Nodes = new List<NodeModel>();
		Connectors = new List<ConnectorModel>();

		X = Camera.main.transform.position.x;
		Y = Camera.main.transform.position.y;
		Z = Camera.main.transform.position.z;

		HasUnsavedChanges = false;
		LastSaved = DateTime.Now;
		_appmodel = appmodel;

	}

	private GraphModel(String name, IEnumerable<NodeModel> nodes,
						 IEnumerable<ConnectorModel> connectors, float x, float y, float z, AppModel appmodel)
	{

		Name = name;

		Nodes = new List<NodeModel>(nodes);
		Connectors = new List<ConnectorModel>(connectors);

		X = x;
		Y = y;
		Z = z;

		HasUnsavedChanges = false;
		LastSaved = DateTime.Now;
		_appmodel = appmodel;

		//WorkspaceSaved += OnWorkspaceSaved;
		//WorkspaceVersion = AssemblyHelper.GetDynamoVersion();
		//undoRecorder = new UndoRedoRecorder(this);
	}

	
	/// <summary>
	/// this method instantiates a node of any type that inherits from node model
	/// it creates a gameobject to host the NodeModel by parsing the type parameters
	/// </summary>
	/// <typeparam name="NT"></typeparam>
	/// <returns></returns>

	public GameObject InstantiateNode<NT>(Vector3 point, Guid ID = new Guid()) where NT : NodeModel
	{
		var newnode = new GameObject();
		newnode.transform.position = point;
		newnode.AddComponent<NT>();
		// if not using default parameter then set the GUID explicitly
		if (ID != Guid.Empty)
		{
			newnode.GetComponent<NT>().SetGuidOnLoad(ID);
		}
		newnode.GetComponent<NT>().name = "node " + typeof(NT).Name + newnode.GetComponent<NT>().GUID.ToString();
		newnode.GetComponent<NT>().GraphOwner = this;
		Nodes.Add(newnode.GetComponent<NodeModel>());
		return newnode;
	}

	/// <summary>
	/// iterate all ports on node and have graph model remove connections
	/// this should call disconnect on other nodes correctly
	/// </summary>
	/// <param name="node"></param>
	public void DeleteNode(GameObject node)
	{
		Nodes.Remove(node.GetComponent<NodeModel>());
		//TODO need to check if the connections exist before trying to remove them
		node.GetComponent<NodeModel>().Inputs.ForEach(x => RemoveConnection(x));
		node.GetComponent<NodeModel>().Outputs.ForEach(x => RemoveConnection(x));
		node.GetComponent<NodeModel>().ExecutionInputs.ForEach(x => RemoveConnection(x));
		node.GetComponent<NodeModel>().ExecutionOutputs.ForEach(x => RemoveConnection(x));

		//then destory gameobject
		GameObject.Destroy(node);

	}

	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void NotifyPropertyChanged(String info)
	{
		Debug.Log("sending " + info + " change notification");
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(info));
		}
	}

	public void RemoveConnection(PortModel connectionEnd)
	{
		//do a null check to make sure the port actually has a connector  here
		//before attemtping to remove it
		if (connectionEnd.connectors.Count > 0 && connectionEnd.connectors[0]!=null)
		{
			//TODO THIS ONLY MAKES SENSE FOR INPUT NODES... and data connectors, not execution connectors
			Connectors.Remove(connectionEnd.connectors[0]);
			connectionEnd.Disconnect(connectionEnd.connectors[0]);
		}
	}

	public void AddConnection(PortModel start, PortModel end)
	{

		var realConnector = new GameObject("Connector");
		if (end.GetType() == typeof(ExecutionPortModel))
		{
			realConnector.AddComponent<ExecutionConnectorModel>();
		}
		else
		{
			realConnector.AddComponent<ConnectorModel>();
		}

		//TODO fix logic so that we reoder the inputs correctly... outputs should go first, then inputs are the end, can just check types
		realConnector.GetComponent<ConnectorModel>().init(start, end);
		end.Connect(realConnector.GetComponent<ConnectorModel>());
		Connectors.Add(realConnector.GetComponent<ConnectorModel>());

	}

	public void SaveGraphModel(string targetpath)
	{
		if (targetpath != null)
		{
			serializeGraph(targetpath);
		}

	}

	public void LoadGraphModel(string xmlpath)
	{
		if (xmlpath != null)
		{
			deserialzeGraph(xmlpath);
		}

	}

	/// <summary>
	///sub routine for deserialzation that needs to be run 
	///only after nodes are done being created
	/// </summary>
	private IEnumerator loadConnectors(XmlNode cNodesList, XmlNode ceNodesList)
	{
		yield return null;
		foreach (XmlNode connector in cNodesList.ChildNodes)
		{
			XmlAttribute guidStartAttrib = connector.Attributes[0];
			XmlAttribute intStartAttrib = connector.Attributes[1];
			XmlAttribute guidEndAttrib = connector.Attributes[2];
			XmlAttribute intEndAttrib = connector.Attributes[3];
			XmlAttribute portTypeAttrib = connector.Attributes[4];
			
			var guidStart = new Guid(guidStartAttrib.Value);
			var guidEnd = new Guid(guidEndAttrib.Value);
			int startIndex = Convert.ToInt16(intStartAttrib.Value);
			int endIndex = Convert.ToInt16(intEndAttrib.Value);
			PortModel.porttype portType = ((PortModel.porttype)Convert.ToInt16(portTypeAttrib.Value));
			
			//find the elements to connect
			NodeModel start = null;
			NodeModel end = null;
			
			foreach (NodeModel e in Nodes)
			{
				if (e.GUID == guidStart)
				{
					start = e;
				}
				else if (e.GUID == guidEnd)
				{
					end = e;
				}
				if (start != null && end != null)
				{
					break;
				}
			}
			//TODO should return connector?
			this.AddConnection(start.Outputs[startIndex], end.Inputs[endIndex]);
			Debug.Log("<color=orange>file load:</color>" + " done loading data connectors");	
			
			//OnConnectorAdded(newConnector);
		}
		
		
		foreach (XmlNode connector in ceNodesList.ChildNodes)
		{
			XmlAttribute guidStartAttrib = connector.Attributes[0];
			XmlAttribute intStartAttrib = connector.Attributes[1];
			XmlAttribute guidEndAttrib = connector.Attributes[2];
			XmlAttribute intEndAttrib = connector.Attributes[3];
			XmlAttribute portTypeAttrib = connector.Attributes[4];
			
			var guidStart = new Guid(guidStartAttrib.Value);
			var guidEnd = new Guid(guidEndAttrib.Value);
			int startIndex = Convert.ToInt32(intStartAttrib.Value);
			int endIndex = Convert.ToInt32(intEndAttrib.Value);
			PortModel.porttype portType = ((PortModel.porttype)Convert.ToInt32(portTypeAttrib.Value));
			
			//find the elements to connect
			NodeModel start = null;
			NodeModel end = null;
			
			foreach (NodeModel e in Nodes)
			{
				if (e.GUID == guidStart)
				{
					start = e;
				}
				else if (e.GUID == guidEnd)
				{
					end = e;
				}
				if (start != null && end != null)
				{
					break;
				}
			}
			if (start == null)
			{
				Debug.LogException(new Exception("could not find start node"));
			}
			if (end == null)
			{
				Debug.LogException(new Exception("could not find end node"));
			}
			
			Debug.Log("<color=orange>start node:</color> " + start.name );
			Debug.Log("<color=orange>end node:</color> " + end.name );
			
			//TODO should return connector?
			// it's possible the outputs have not been added yet, we may need to wait until the 
			// the next frame for the start method to finish completing on nodemodels.
			// if this is the case, we can just check if the node is started, if not, start it here.
			
			this.AddConnection(start.ExecutionOutputs[startIndex], end.ExecutionInputs[endIndex]);
			Debug.Log("<color=orange>file load:</color>" + " done loading execution connectors");	
			
			//OnConnectorAdded(newConnector);
		}
	}


	public bool deserialzeGraph(string xmlpath)
	{
		Debug.Log("opening a graph " + xmlpath);

		var xmlDoc = new XmlDocument();
		xmlDoc.Load(xmlpath);

		float cx = 0;
		float cy = 0;
		float cz = 0;

		XmlNodeList workspaceNodes = xmlDoc.GetElementsByTagName("Graph");

		foreach (XmlNode node in workspaceNodes)
		{
			foreach (XmlAttribute att in node.Attributes)
			{
				if (att.Name.Equals("X"))
				{
					cx = float.Parse(att.Value);
				}
				else if (att.Name.Equals("Y"))
				{
					cy = float.Parse(att.Value);
				}
				else if (att.Name.Equals("Z"))
				{
					cz = float.Parse(att.Value);
				}
			}
		}
			this.X = cx;
			this.Y = cy;
			this.Z = cz;

			XmlNodeList elNodes = xmlDoc.GetElementsByTagName("Elements");
			XmlNodeList cNodes = xmlDoc.GetElementsByTagName("Connectors");
			XmlNodeList ceNodes = xmlDoc.GetElementsByTagName("ExecutionConnectors");
			
			XmlNode elNodesList = elNodes[0];
			XmlNode cNodesList = cNodes[0];
			XmlNode ceNodesList = ceNodes[0];
			
			foreach (XmlNode elNode in elNodesList.ChildNodes)
			{
				XmlAttribute typeAttrib = elNode.Attributes["type"];
				XmlAttribute guidAttrib = elNode.Attributes["guid"];
				XmlAttribute nicknameAttrib = elNode.Attributes["nickname"];
				XmlAttribute xAttrib = elNode.Attributes["x"];
				XmlAttribute yAttrib = elNode.Attributes["y"];
				XmlAttribute zAttrib = elNode.Attributes["z"];
				

				string typeName = typeAttrib.Value;

				//test the GUID to confirm that it is non-zero
				//if it is zero, then we have to fix it
				//this will break the connectors, but it won't keep
				//propagating bad GUIDs
				var guid = new Guid(guidAttrib.Value);
				if (guid == Guid.Empty)
				{
					guid = Guid.NewGuid();
					Debug.LogException(new Exception("bad guid loaded"));
				}

				string nickname = nicknameAttrib.Value;

				float x = float.Parse(xAttrib.Value);
				float y = float.Parse(yAttrib.Value);
				float z = float.Parse(zAttrib.Value);

				
				// Retrieve optional 'function' attribute (only for DSFunction).
				//XmlAttribute signatureAttrib = elNode.Attributes["function"];
				//var signature = signatureAttrib == null ? null : signatureAttrib.Value;

				NodeModel el = null;
				XmlElement dummyElement = null;

				try
				{
					// The attempt to create node instance may fail due to "type" being
					// something else other than "NodeModel" derived object type.

					/*typeName = Utils.PreprocessTypeName(typeName);
					Type type = Utils.ResolveType(this, typeName);
					if (type != null)
					{
						var tld = Utils.GetDataForType(this, type);
						el = CurrentWorkspace.NodeFactory.CreateNodeInstance(
							tld, nickname, signature, guid);
					}
					*/
					//TODO need to add some methods for type resolve before attempting to 
					// instantiate node..
					Type nodetype = Type.GetType(typeName);
					
					MethodInfo method = this.GetType().GetMethod("InstantiateNode");
					MethodInfo generic = method.MakeGenericMethod(nodetype);
					//generic is a delegate pointing towards instantiate node, we're passing the nodetype to instantiate
					//this is a fully qualified type extracted from the xml file
					el = (generic.Invoke(this, new object[]{new Vector3(x,y,z),guid}) as GameObject).GetComponent<NodeModel>();
					//TODO not using nickname should I?
					
					if (el != null)
					{
						el.Load(elNode);
					}
					//else
				//	{
						//var e = elNode as XmlElement;
						//dummyElement = MigrationManager.CreateMissingNode(e, 1, 1);
						
					//}
				}
				catch (Exception e)
				{
					// If a given function is not found during file load, then convert the 
					// function node into a dummy node (instead of crashing the workflow).
					// 
					//var e = elNode as XmlElement;
					//dummyElement = MigrationManager.CreateUnresolvedFunctionNode(e);
					Debug.LogException(new Exception("problem loading... " + typeName));
				}

			/*	// If a custom node fails to load its definition, convert it into a dummy node.
				var function = el as Function;
				if ((function != null) && (function.Definition == null))
				{
					var e = elNode as XmlElement;
					dummyElement = MigrationManager.CreateMissingNode(
						e, el.InPortData.Count, el.OutPortData.Count);
				}

				if (dummyElement != null) // If a dummy node placement is desired.
				{
					// The new type representing the dummy node.
					typeName = dummyElement.GetAttribute("type");
					var type = Utils.ResolveType(this, typeName);
					var tld = Utils.GetDataForType(this, type);

					el = CurrentWorkspace.NodeFactory.CreateNodeInstance(tld, nickname, String.Empty, guid);
					el.Load(dummyElement);
				}
				*/
				
				//TODO add event
				//OnNodeAdded(el);

			
			}

			Debug.Log("<color=orange>file load:</color>" + " done loading nodes");

		_appmodel.StartCoroutine(loadConnectors(cNodesList,ceNodesList));
			
			this.FileName = xmlpath;
			this.HasUnsavedChanges = false;

		return true;
	}


	
			

	public bool serializeGraph(string targetpath)
	{
		var doc = new XmlDocument();
		doc.CreateXmlDeclaration("1.0", null, null);
		doc.AppendChild(doc.CreateElement("Graph"));
		doc.DocumentElement.SetAttribute("FilePath", targetpath);

		if (!PopulateXmlDocument(doc))
		{
			return false;
		}
		try
		{
			doc.DocumentElement.SetAttribute("FilePath", string.Empty);
			doc.Save(targetpath);
		}
		catch (System.IO.IOException ex)
		{
			Debug.Log(ex.Message);
			return false;
		}

		FileName = targetpath;
		return true;
	}
	//slightly modified from Dynamo.74 src
	protected bool PopulateXmlDocument(XmlDocument xmlDoc)
	{
		try
		{
			var root = xmlDoc.DocumentElement;

			root.SetAttribute("X", X.ToString());
			root.SetAttribute("Y", Y.ToString());
			root.SetAttribute("Z", Z.ToString());
			//root.SetAttribute("Category", Category);
			root.SetAttribute("Name", Name);

			var elementList = xmlDoc.CreateElement("Elements");
			//write the root element
			root.AppendChild(elementList);

			foreach (var node in Nodes)
			{
				var typeName = node.GetType().ToString();

				var nodeElement = xmlDoc.CreateElement(typeName);
				elementList.AppendChild(nodeElement);

				//set the type attribute
				nodeElement.SetAttribute("type", node.GetType().ToString());
				nodeElement.SetAttribute("guid", node.GUID.ToString());
				nodeElement.SetAttribute("nickname", node.name);
				nodeElement.SetAttribute("x", node.gameObject.transform.position.x.ToString());
				nodeElement.SetAttribute("y", node.gameObject.transform.position.y.ToString());
				nodeElement.SetAttribute("z", node.gameObject.transform.position.z.ToString());

				node.Save(xmlDoc, nodeElement);
			}

			//write only the output connectors
			var connectorList = xmlDoc.CreateElement("Connectors");
			//write the root element
			root.AppendChild(connectorList);

			foreach (var node in Nodes)
			{
				var ports = node.Outputs;
				foreach (var port in ports)
				{
					foreach (
	var c in
	port.connectors.Where(c => c.PStart != null && c.PEnd != null))
					{
						var connector = xmlDoc.CreateElement(c.GetType().ToString());
						connectorList.AppendChild(connector);
						connector.SetAttribute("start", c.PStart.Owner.GUID.ToString());
						connector.SetAttribute("start_index", c.PStart.Index.ToString());
						connector.SetAttribute("end", c.PEnd.Owner.GUID.ToString());
						connector.SetAttribute("end_index", c.PEnd.Index.ToString());

						if (c.PEnd.PortType == PortModel.porttype.input)
							connector.SetAttribute("portType", "0");
					}
				}
			}

			var ExecconnectorList = xmlDoc.CreateElement("ExecutionConnectors");
			//write the root element
			root.AppendChild(ExecconnectorList);

			foreach (var node in Nodes)
			{
				var ports = node.ExecutionOutputs;
				foreach (var port in ports)
				{
					foreach (
	var c in
	port.connectors.Where(c => c.PStart != null && c.PEnd != null))
					{
						var connector = xmlDoc.CreateElement(c.GetType().ToString());
						ExecconnectorList.AppendChild(connector);
						connector.SetAttribute("start", c.PStart.Owner.GUID.ToString());
						connector.SetAttribute("start_index", c.PStart.Index.ToString());
						connector.SetAttribute("end", c.PEnd.Owner.GUID.ToString());
						connector.SetAttribute("end_index", c.PEnd.Index.ToString());

						if (c.PEnd.PortType == PortModel.porttype.input)
							connector.SetAttribute("portType", "0");
					}
				}
			}



			return true;
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message + " : " + ex.StackTrace);
			return false;
		}
	}
	//TODO doesnt really belong here
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickCount != 2)
		{
			return;
		}
		var creationPoint = Vector3.zero;
		var mousePos = eventData.pressPosition;
		if (Nodes.Count > 0)
		{

			// this is basically reduce with a conditional either passing min or next, to find the min closest node
			// could replace with for loop...
			var closestNode = Nodes.Aggregate((min, next) => Vector3.Distance(min.transform.position, mousePos) < Vector3.Distance(next.transform.position, mousePos) ? min : next);
			// get distance to closest node
			var distToClosest = Vector3.Distance(Camera.main.transform.position, closestNode.transform.position);
			creationPoint = BaseView<NodeModel>.ProjectCurrentDrag(distToClosest);
		}

		//todo creation of a new node or element needs to be redesigned - 
		// process will be in general - 
		// create an empty gameobject
		// add a Model to it
		// Model will create a view which will also be added to the root GO
		// the view will call a method on the model to construct UI elements
		// which will be added to the scene and form some tree structure under the root

		var newnode = InstantiateNode<TestNodeType>(creationPoint);

	}



}

