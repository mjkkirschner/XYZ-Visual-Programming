using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using System;
using System.ComponentModel;


    class ExecutionPortModel:PortModel
    {

        public List<ExecutionConnectorModel> ExecutionConnectors = new List<ExecutionConnectorModel>();

        protected override void Start()
        {
            // create the view here
            this.gameObject.AddComponent<ExecutionPortView>();

        }

        public virtual void Connect(ExecutionConnectorModel connector)
        {
            connectors.Add(connector);

            //throw the event for a connection
            OnPortConnected(EventArgs.Empty);

            IsConnected = true;
        }
       
        public virtual void Disconnect(ExecutionConnectorModel connector)
        {
            if (!connectors.Contains(connector))
                return;
            //throw the event for a connection
            OnPortDisconnected(EventArgs.Empty);
            connectors.Remove(connector);
           
            GameObject.Destroy(connector.gameObject);
            
            if (connectors.Count == 0)
            {
                IsConnected = false;
            }

            //Owner.ValidateConnections ();
        }

        public override GameObject BuildSceneElements()
        {

            GameObject UI = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            UI.transform.localPosition = this.gameObject.transform.position;
            UI.transform.parent = this.gameObject.transform;

            return UI;



        }
    }

