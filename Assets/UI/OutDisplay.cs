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

    class OutDisplay : UIBehaviour
    {
        private NodeModel model;
        public String NodeName { get; set; }
        private List<String> outputstrings;
        private Dictionary<string, object> outputDict;
        private GameObject mainPanel;
        private List<OutputDisplayPair> currentOutputdisplaydata;

        protected override void Start()
        {
            //find child window and text controls here
			mainPanel = this.gameObject;
            //find the model
            model = this.gameObject.GetComponentInParent<NodeModel>();
        }

        public void HandleModelChanges(object sender, PropertyChangedEventArgs e)
        {

            // if the property on the model we are watching
            // was the stored value, then we should update our output preview
            if (e.PropertyName == "StoredValue")
            {
                outputDict = model.StoredValueDict;
                UpdateOutputs();
            }
            else
            {
               // throw new System.NotImplementedException();
            }
        }

        public void UpdateOutputs()
        {
            //destroy all currentoutputs
            currentOutputdisplaydata = GetComponentsInChildren<OutputDisplayPair>().ToList();

            if (currentOutputdisplaydata != null)
            {
                currentOutputdisplaydata.ForEach(x => GameObject.Destroy(x.gameObject));
            }
            //create new ones
            foreach (var entry in outputDict)
            {
                //for now, we create the output pairs here based on the out
                //output dict we have been extracted from the model on its property change

                var newdisplay = Resources.Load("OutputPair") as GameObject;
                newdisplay = GameObject.Instantiate(newdisplay) as GameObject;
                newdisplay.GetComponent<RectTransform>().SetParent(mainPanel.GetComponent<RectTransform>(),false);
                //update the labels with the current output dictionary data
                newdisplay.GetComponent<OutputDisplayPair>().UpdateLabels(entry.Key, entry.Value);
            }
        }

    }
}
