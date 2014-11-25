using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using System;
using System.ComponentModel;


    public class ExecutionPortModel:PortModel
    {

        public List<ExecutionConnectorModel> ExecutionConnectors = new List<ExecutionConnectorModel>();

        protected override void Start()
        {
            // create the view here
            this.gameObject.AddComponent<ExecutionPortView>();

        }


        public override GameObject BuildSceneElements()
        {

            GameObject UI = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            UI.transform.localPosition = this.gameObject.transform.position;
            UI.transform.parent = this.gameObject.transform;

            return UI;



        }
    }

