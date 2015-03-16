using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using System;
using System.ComponentModel;


    public class ExecutionPortModel:PortModel
    {

        

        protected override void Start()
        {
            // create the view here
            this.gameObject.AddComponent<ExecutionPortView>();

        }


        public override GameObject BuildSceneElements()
        {
			GameObject model = Resources.Load("ExecutionPort") as GameObject;
			GameObject UI = GameObject.Instantiate(model) as GameObject;
            UI.transform.localPosition = this.gameObject.transform.position;
            UI.transform.parent = this.gameObject.transform;

			AddPortLabel();

            return UI;

        }

	protected override void AddPortLabel()
	{
		var labelprefab = Resources.Load<GameObject>("PortLabelSimple");
		var label = GameObject.Instantiate(labelprefab, Vector3.zero, Quaternion.identity) as GameObject;
		var labelAttachment = this.transform.GetChild(0).Find("port");
		label.GetComponent<RectTransform>().SetParent(this.transform, false);
		label.transform.Translate (0,0,labelAttachment.transform.position.z*-15 + (label.GetComponent<RectTransform>().sizeDelta.x/200));
		label.AddComponent<UILabel>();
	}
    }

