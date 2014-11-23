using UnityEngine;
using System.Collections.Generic;
using System.Linq;



    class ExecutionConnectorModel:ConnectorModel
    {

        public new ExecutionPortModel PStart { get; set; }
        public new ExecutionPortModel PEnd { get; set; }
       



        protected override void OnEnable()
        {
            //create a connector view
            View = this.gameObject.AddComponent<ConnectorView>();
            View.Model = this;


        }


        public virtual void init(ExecutionPortModel start, ExecutionPortModel end)
        {

            PStart = start;
            PEnd = end;
            Debug.Log(start);
            Debug.Log(end);
            // hook listeners on the connector view to the ports
            View.EndPort = PEnd;
            View.StartPort = PStart;
            start.PropertyChanged += View.HandlePortChanges;
            end.PropertyChanged += View.HandlePortChanges;

        }


    }

