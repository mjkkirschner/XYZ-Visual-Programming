using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using System;
using System.ComponentModel;

// <summary>
/// object representing an input port.
/// </summary>
public class PortModel :BaseModel
{
    
		public string NickName { get; set; }

		public NodeModel Owner { get; set; }

		public int Index { get; set; }

		public Boolean IsConnected { get; set; }

		public List<ConnectorModel> connectors = new List<ConnectorModel> ();

		public delegate void PortConnectedHandler (object sender,EventArgs e);

		public delegate void PortDisconnectedHandler (object sender,EventArgs e);

		public event PortConnectedHandler PortConnected;
		public event PortConnectedHandler PortDisconnected;

       protected override void Start()
		{
				// create the view here
				this.gameObject.AddComponent<PortView> ();
               
		}
    
		protected virtual void OnPortConnected (EventArgs e)
		{
				if (PortConnected != null)
						PortConnected (this, e);
		}

		protected virtual void OnPortDisconnected (EventArgs e)
		{
				if (PortDisconnected != null)
						PortDisconnected (this, e);
		}

		//		public void DestroyConnectors ()
		//		{
		//				if (Owner == null)
		//						return;
		//		
		//				while (connectors.Any()) {
		//						ConnectorModel connector = connectors [0];
		//						Owner.Connectors.Remove (connector);
		//						connector.NotifyConnectedPortsOfDeletion ();
		//				}
		//		}

		public virtual void Connect (ConnectorModel connector)
		{
				connectors.Add (connector);
				IsConnected = true;
				//throw the event for a connection
				OnPortConnected (EventArgs.Empty);

				
		}
		/// <summary>
		/// Disconnect the specified connector from this port.
		/// </summary>
		/// <param name="connector">Connector.</param>
		public virtual void Disconnect (ConnectorModel connector)
		{		
				
				if (connectors.Remove (connector) == false)
				{
					Debug.Log("could not disconnect connect, could not find it in connector list");
				}
				if (connectors.Count == 0) {
					IsConnected = false;
					}
				//throw the event for a connection
				OnPortDisconnected (EventArgs.Empty);
				
				
				
				//Owner.ValidateConnections ();
		}


		public porttype PortType { get; set; }


		public enum porttype
		{

				input,
				output
    }
		;

		public void ConnectorDisconnectEventHandler(object sender, EventArgs e){
		//a connector is about to be destroyed and needs to be removed and diconnected so if it's still connected remove it
		if (connectors.Contains(sender as ConnectorModel)){
			//if not, then connect
			this.Disconnect(sender as ConnectorModel);
		}
		}

		public void ConnectorConnectEventHandler(object sender, EventArgs e){
		//a connector just sent an event that it was connected, this port has been subscribed to this event through the creation of the connector
		// in it's initializer

		//check if this port is already connected to this connector
		if (connectors.Contains(sender as ConnectorModel)==false){
			//if not, then connect
			this.Connect(sender as ConnectorModel);
		}

		}

		/// this handler is used to respond to changes on the node owner of this port
		// right now it just forwards the notification
		public void NodePropertyChangeEventHandler (object sender, EventArgs args)
		{
				NotifyPropertyChanged ("OwnerProperties");

		}

		public virtual void init (NodeModel owner, int index, porttype type, string nickname = null)
		{


				this.Owner = owner;
				this.Index = index;
				this.PortType = type;
				if (nickname == null) {
						NickName = Owner.name;
						NickName = this.NickName + PortType.ToString () + Index.ToString ();

				}
                else
                {
                    NickName = nickname;
                }
		}

        public override GameObject BuildSceneElements()
        {

            GameObject UI = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            UI.transform.localPosition = this.gameObject.transform.position;
            UI.transform.parent = this.gameObject.transform;

			AddPortLabel();
            return UI;



        }

		protected void AddPortLabel()
		{
			var labelprefab = Resources.Load<GameObject>("PortLabelSimple");
			var label = GameObject.Instantiate(labelprefab, Vector3.zero, Quaternion.identity) as GameObject;
			label.GetComponent<RectTransform>().SetParent(this.transform, false);
			label.AddComponent<UILabel>();
		}


}

