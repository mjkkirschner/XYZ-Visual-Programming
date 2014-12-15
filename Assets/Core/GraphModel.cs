using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine.EventSystems;
using System.Linq;
using Nodeplay.Nodes;
using System.Xml;

/// <summary>
/// initially modified from Dynamo's source version .74
/// https://github.com/DynamoDS/Dynamo/blob/master/src/DynamoCore/Models/WorkspaceModel.cs
/// </summary>


public class GraphModel:INotifyPropertyChanged, IPointerClickHandler
{
	
		public List<NodeModel> Nodes { get; set; }

		public List<ConnectorModel> Connectors { get; set; }

		public string Name{ get; set; }

		public float X{ get; set; }

		public float Y{ get; set; }

		public float Z{ get; set; }

		public bool HasUnsavedChanges { get; set; }

		public DateTime LastSaved { get; set; }

		public bool Current{ get; set; }

		public string FileName { get; set; }

		public static GraphModel ByXmlFile (string filepath)
		{
				//TODO parse the filepath, load the file here, and then call graphmodel constructor
				// which will deserialze
				return new GraphModel ("loaded");
		}

		public GraphModel (String name)
		{

				Name = name;
		
				Nodes = new List<NodeModel> ();
				Connectors = new List<ConnectorModel> ();
		
				X = Camera.main.transform.position.x;
				Y = Camera.main.transform.position.y;
				Z = Camera.main.transform.position.z;
		
				HasUnsavedChanges = false;
				LastSaved = DateTime.Now;

		}

		private GraphModel (String name, IEnumerable<NodeModel> nodes,
	                         IEnumerable<ConnectorModel> connectors, float x, float y, float z)
		{

				Name = name;
		
				Nodes = new List<NodeModel> (nodes);
				Connectors = new List<ConnectorModel> (connectors);

				X = x;
				Y = y;
				Z = z;
		
				HasUnsavedChanges = false;
				LastSaved = DateTime.Now;
		
				//WorkspaceSaved += OnWorkspaceSaved;
				//WorkspaceVersion = AssemblyHelper.GetDynamoVersion();
				//undoRecorder = new UndoRedoRecorder(this);
		}

		//todo will need to block out/implment functions to create nodes,connector etc with no GUI interaction
		// for loading
		/// <summary>
		/// this method instantiates a node of any type that inherits from node model
		/// it creates a gameobject to host the NodeModel by parsing the type parameters
		/// </summary>
		/// <typeparam name="NT"></typeparam>
		/// <returns></returns>
	
		public GameObject InstantiateNode<NT> (Vector3 point) where NT: NodeModel
		{
				var newnode = new GameObject ();
				newnode.transform.position = point;
				newnode.AddComponent<NT> ();
				newnode.GetComponent<NT> ().name = "node " + typeof(NT).Name + newnode.GetComponent<NT> ().GUID.ToString ();
				newnode.GetComponent<NT> ().GraphOwner = this;
				Nodes.Add (newnode.GetComponent<NodeModel> ());
				return newnode;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged (String info)
		{
				Debug.Log ("sending " + info + " change notification");
				if (PropertyChanged != null) {
						PropertyChanged (this, new PropertyChangedEventArgs (info));
				}
		}
		
		public void RemoveConnection (PortModel connectionEnd)
		{
				//TODO THIS ONLY MAKES SENSE FOR INPUT NODES... and data connectors, not execution connectors
				Connectors.Remove (connectionEnd.connectors [0]);
				connectionEnd.Disconnect (connectionEnd.connectors [0]);

		}

		public void AddConnection (PortModel start, PortModel end)
		{

				var realConnector = new GameObject ("Connector");
				if (end.GetType () == typeof(ExecutionPortView)) {
						realConnector.AddComponent<ExecutionConnectorModel> ();
				} else {
						realConnector.AddComponent<ConnectorModel> ();
				}

				//TODO fix logic so that we reoder the inputs correctly... outputs should go first, then inputs are the end, can just check types
				realConnector.GetComponent<ConnectorModel> ().init (start, end);
				end.Connect (realConnector.GetComponent<ConnectorModel> ());
				Connectors.Add (realConnector.GetComponent<ConnectorModel> ());

		}

		public void SaveGraphModel (string targetpath)
		{
				if (targetpath != null) {
						serializeGraph (targetpath);
				}

		}

		public bool serializeGraph (string targetpath)
		{
				var doc = new XmlDocument ();
				doc.CreateXmlDeclaration ("1.0", null, null);
				doc.AppendChild (doc.CreateElement ("Graph"));
				doc.DocumentElement.SetAttribute ("FilePath", targetpath);
		
				if (!PopulateXmlDocument (doc)) {
						return false;
				}
				try {
						doc.DocumentElement.SetAttribute ("FilePath", string.Empty);
						doc.Save (targetpath);
				} catch (System.IO.IOException ex) {
						Debug.Log (ex.Message);
						return false;
				}

				FileName = targetpath;
				return true;
		}
		//slightly modified from Dynamo.74 src
		protected bool PopulateXmlDocument (XmlDocument xmlDoc)
		{
				try {
						var root = xmlDoc.DocumentElement;
			
						root.SetAttribute ("X", X.ToString ());
						root.SetAttribute ("Y", Y.ToString ());
						root.SetAttribute ("Z", Z.ToString ());
						//root.SetAttribute("Category", Category);
						root.SetAttribute ("Name", Name);
			
						var elementList = xmlDoc.CreateElement ("Elements");
						//write the root element
						root.AppendChild (elementList);
			
						foreach (var node in Nodes) {
								var typeName = node.GetType ().ToString ();
				
								var nodeElement = xmlDoc.CreateElement (typeName);
								elementList.AppendChild (nodeElement);
				
								//set the type attribute
								nodeElement.SetAttribute ("type", node.GetType ().ToString ());
								nodeElement.SetAttribute ("guid", node.GUID.ToString ());
								nodeElement.SetAttribute ("nickname", node.name);
								nodeElement.SetAttribute ("x", node.gameObject.transform.position.x.ToString ());
								nodeElement.SetAttribute ("y", node.gameObject.transform.position.y.ToString ());
								nodeElement.SetAttribute ("z", node.gameObject.transform.position.z.ToString ());
				
								node.Save (xmlDoc, nodeElement);
						}
			
						//write only the output connectors
						var connectorList = xmlDoc.CreateElement ("Connectors");
						//write the root element
						root.AppendChild (connectorList);
			
						foreach (var node in Nodes) {
								var ports = node.Outputs;
								foreach (var port in ports) {
										foreach (
						var c in
						port.connectors.Where(c => c.PStart != null && c.PEnd != null)) {
												var connector = xmlDoc.CreateElement (c.GetType ().ToString ());
												connectorList.AppendChild (connector);
												connector.SetAttribute ("start", c.PStart.Owner.GUID.ToString ());
												connector.SetAttribute ("start_index", c.PStart.Index.ToString ());
												connector.SetAttribute ("end", c.PEnd.Owner.GUID.ToString ());
												connector.SetAttribute ("end_index", c.PEnd.Index.ToString ());
						
												if (c.PEnd.PortType == PortModel.porttype.input)
														connector.SetAttribute ("portType", "0");
										}
								}
						}

						var ExecconnectorList = xmlDoc.CreateElement ("ExecutionConnectors");
						//write the root element
						root.AppendChild (ExecconnectorList);
			
						foreach (var node in Nodes) {
								var ports = node.ExecutionOutputs;
								foreach (var port in ports) {
										foreach (
						var c in
						port.connectors.Where(c => c.PStart != null && c.PEnd != null)) {
												var connector = xmlDoc.CreateElement (c.GetType ().ToString ());
												ExecconnectorList.AppendChild (connector);
												connector.SetAttribute ("start", c.PStart.Owner.GUID.ToString ());
												connector.SetAttribute ("start_index", c.PStart.Index.ToString ());
												connector.SetAttribute ("end", c.PEnd.Owner.GUID.ToString ());
												connector.SetAttribute ("end_index", c.PEnd.Index.ToString ());
						
												if (c.PEnd.PortType == PortModel.porttype.input)
														connector.SetAttribute ("portType", "0");
										}
								}
						}
						
						

						return true;
				} catch (Exception ex) {
						Debug.Log (ex.Message + " : " + ex.StackTrace);
						return false;
				}
		}
		//TODO doesnt really belong here
		public void OnPointerClick (PointerEventData eventData)
		{
				if (eventData.clickCount != 2) {
						return;
				}
				var creationPoint = Vector3.zero;
				var mousePos = eventData.pressPosition;
				if (Nodes.Count > 0) {
                
						// this is basically reduce with a conditional either passing min or next, to find the min closest node
						// could replace with for loop...
						var closestNode = Nodes.Aggregate ((min, next) => Vector3.Distance (min.transform.position, mousePos) < Vector3.Distance (next.transform.position, mousePos) ? min : next);
						// get distance to closest node
						var distToClosest = Vector3.Distance (Camera.main.transform.position, closestNode.transform.position);
						creationPoint = BaseView<NodeModel>.ProjectCurrentDrag (distToClosest);
				}
           
				//todo creation of a new node or element needs to be redesigned - 
				// process will be in general - 
				// create an empty gameobject
				// add a Model to it
				// Model will create a view which will also be added to the root GO
				// the view will call a method on the model to construct UI elements
				// which will be added to the scene and form some tree structure under the root

				var newnode = InstantiateNode<TestNodeType> (creationPoint);

		}



}

