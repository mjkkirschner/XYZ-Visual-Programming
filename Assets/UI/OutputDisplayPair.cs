using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.EventSystems;
using Nodeplay.UI.Utils;


namespace Nodeplay.UI
{

    class OutputDisplayPair : UIBehaviour
    {
        public String outputname { get; set; }
        private String outputstring;
        private Text nameControl;
        private Text valueControl;
        bool started = false;

        protected override void Start()
        {
            //find the text controls by name
            nameControl = transform.FindChild("output name").GetComponent<Text>();
            valueControl = transform.FindChild("output value").GetComponent<Text>();
            started = true;
        }

       
        public void UpdateLabels(string name, object value)
        {
            if (!started){
                Start();
            }
            nameControl.text = name;
            outputstring = value.ToJSONstring();
            valueControl.text = outputstring;
        }



    }
}
