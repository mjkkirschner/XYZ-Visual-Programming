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

    class InputDisplay : UIBehaviour
    {
        public NodeModel model;
        public String NodeName { get; set; }
        private Dictionary<string, object> inputDict;
        private GameObject mainPanel;
		private GameObject inputPanel;
        private List<InputDisplayPair> currentInputpairs;

        protected override void Start()
        {
			//set the correct settings for this canvas renderer
			this.gameObject.GetComponent<Canvas>().worldCamera = Camera.main;

            //find child window and text controls here
            mainPanel = this.gameObject.GetComponentInChildren<ContentSizeFitter>().gameObject;
			//find input panel
			inputPanel = mainPanel.transform.FindChild("InputPanel").gameObject;
			var nodeNameControl = mainPanel.transform.FindChild("Header").gameObject;
			 nodeNameControl = nodeNameControl.GetComponentInChildren<Text>().gameObject;

            //find the model
            model = this.gameObject.GetComponentInParent<NodeModel>();
			inputDict = model.InputValueDict;
			nodeNameControl.GetComponent<Text>().text = model.GetType().Name;
			NodeName = model.GetType().Name;
			UpdateInputs();
        }

        public void HandleModelChanges(object sender, PropertyChangedEventArgs e)
        {

            // if the property on the model we are watching
            // was the stored value, then we should update our output preview
            if (e.PropertyName == "InputValues")
            {
                //inputDict = model.InputValueDict;
                //UpdateInputs();
            }
            else
            {
               // throw new System.NotImplementedException();
            }
        }

        public void UpdateInputs()
        {
            //destroy all currentoutputs
			Debug.Log("regening inputs");

			currentInputpairs = GetComponentsInChildren<InputDisplayPair>().ToList();

			if (currentInputpairs != null)
            {
				currentInputpairs.ForEach(x => GameObject.Destroy(x.gameObject));
            }
            //create new ones
			if (inputDict != null)
			{
            foreach (var entry in inputDict)
            {
                
                var newdisplay = Resources.Load("inputpair") as GameObject;
                newdisplay = GameObject.Instantiate(newdisplay) as GameObject;
                newdisplay.GetComponent<RectTransform>().SetParent(inputPanel.GetComponent<RectTransform>(),false);
                //update the labels with the current output dictionary data
                newdisplay.GetComponent<InputDisplayPair>().UpdateLabels(entry.Key, entry.Value);
				newdisplay.GetComponent<InputDisplayPair>().InputName = entry.Key;
				}
            }
        }

    }
}