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

    private float dist_to_camera;

    public Boolean IsConnected { get; set; }

    public List<ConnectorModel> connectors = new List<ConnectorModel>();

    public delegate void PortConnectedHandler(object sender, EventArgs e);

    public delegate void PortDisconnectedHandler(object sender, EventArgs e);

    public event PortConnectedHandler PortConnected;
    public event PortConnectedHandler PortDisconnected;

     void OnEnable()
    {
        // create the view here
        this.gameObject.AddComponent<PortView>();
    }
    
    protected virtual void OnPortConnected(EventArgs e)
    {
        if (PortConnected != null)
            PortConnected(this, e);
    }

    protected virtual void OnPortDisconnected(EventArgs e)
    {
        if (PortDisconnected != null)
            PortDisconnected(this, e);
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

    public void Connect(ConnectorModel connector)
    {
        connectors.Add(connector);

        //throw the event for a connection
        OnPortConnected(EventArgs.Empty);

        IsConnected = true;
    }
    /// <summary>
    /// Disconnect the specified connector from this port.
    /// </summary>
    /// <param name="connector">Connector.</param>
    public void Disconnect(ConnectorModel connector)
    {
        if (!connectors.Contains(connector))
            return;
        //throw the event for a connection
        OnPortDisconnected(EventArgs.Empty);
        connectors.Remove(connector);
        //TODO Right now portmodel has way too much responsibility... all of this needs to be moved out of here - 
        // possibly a manager that recieves events and  constructs or destroys objets... eventually might need to
        // recycle these objects anyway
        //TODO portview should either be responsible for this or should forward a 
        // message to connector to disconnect and destroy itself, but the portmodel should not destroy a go
        GameObject.Destroy(connector.gameObject);
        //don't set back to white if
        //there are still connectors on this port
        if (connectors.Count == 0)
        {
            IsConnected = false;
        }

        //Owner.ValidateConnections ();
    }


    public porttype PortType { get; set; }


    public enum porttype
    {

        input,
        output
    }
		;

    

    /// this handler is used to respond to changes on the node owner of this port
    // right now it just forwards the notification
    public void NodePropertyChangeEventHandler(object sender, EventArgs args)
    {
        NotifyPropertyChanged("OwnerProperties");

    }

    public void init(NodeModel owner, int index, porttype type, string nickname = null)
    {


        this.Owner = owner;
        this.Index = index;
        this.PortType = type;
        if (nickname == null)
        {
            NickName = Owner.name;
            NickName = this.NickName + PortType.ToString() + Index.ToString();

        }

    }



}

