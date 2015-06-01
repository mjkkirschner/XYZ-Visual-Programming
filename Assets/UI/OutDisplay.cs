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
		private GameObject outputDisplayPrefab = Resources.Load("OutputPairInputTest") as GameObject;
		private int lastUpdateFrame = 0;

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
            if (e.PropertyName == "StoredValue" && Time.frameCount > lastUpdateFrame)
            {
                outputDict = model.StoredValueDict;
                UpdateOutputs();
				lastUpdateFrame = Time.frameCount; 
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

         
				//instead of deleting, try just updating the labels instead....
                //currentOutputdisplaydata.ForEach(x => GameObject.Destroy(x.gameObject));
				//we need to lookup the name by the output name val

				foreach(var entry in outputDict)
				{
					var currentLabel = currentOutputdisplaydata.Find(x=>x.outputname == entry.Key);
					if (currentLabel != null)
					{
					currentLabel.UpdateLabels(entry.Key,entry.Value);
					}
					// if the value doesnt exist in the list of names, then create a new prefab
					else
						{
						var newdisplay = GameObject.Instantiate(outputDisplayPrefab) as GameObject;
						newdisplay.GetComponent<RectTransform>().SetParent(mainPanel.GetComponent<RectTransform>(),false);
						newdisplay.GetComponentInChildren<InputFieldDebug>().enabled = true;
						//update the labels with the current output dictionary data
						newdisplay.GetComponent<OutputDisplayPair>().UpdateLabels(entry.Key, entry.Value);
						}
					}
				
				}
            }
        }