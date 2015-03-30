using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;
using Nodeplay.Engine;
using Nodeplay.UI;
using UnityEngine.UI;
using System;
using System.Xml;

namespace Nodeplay.Nodes
{

	public class VariableInputNodeModel : NodeModel
	{
		protected virtual string GetPortName()
		{
			return "some port name" + Inputs.Count.ToString();
		}
	
		//instantiate and setup buttons for adding and removing ports
		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			var inputdisplay = tempUI.transform.root.GetComponentInChildren<InputDisplay>().gameObject;
			var buttonPrefab = Resources.Load("LibraryButton") as GameObject;
			
			var addbutton = GameObject.Instantiate(buttonPrefab);
			var rembutton = GameObject.Instantiate(buttonPrefab);
			
			addbutton.GetComponentInChildren<Text>().text = "Add Input";
			rembutton.GetComponentInChildren<Text>().text = "Remove Input";
			
			addbutton.GetComponent<Button>().onClick.AddListener(() => { this.AddInputPort(GetPortName());});
			rembutton.GetComponent<Button>().onClick.AddListener(() => {
				this.RemoveInputPort(Inputs.Select(x=>x.NickName).ToList().Last());});
			
			addbutton.transform.SetParent(inputdisplay.transform,false);
			rembutton.transform.SetParent(inputdisplay.transform,false);
			return tempUI;
		}

		//save and load are overidden to save ports created at runtime

		public override void Save(XmlDocument doc, XmlElement element)
			
		{
			base.Save(doc,element);

			var portData = doc.CreateElement("variablePorts");
			element.AppendChild(portData);
			//add an attribute foreach input port, can load this later to create ports in correct order
			if (Inputs != null){

			foreach(var port in Inputs)
			{

					portData.SetAttribute("port"+ port.Index.ToString(),port.NickName);
			}
			
			}

		}
		
		public override void Load(XmlNode node)
		{
			XmlNode portdata = null ;
			
			foreach(XmlNode subnode in node.ChildNodes)
			{
				if (subnode.Name == "variablePorts")
				{
					portdata = subnode;
					
				}
				
			}
			
			if (portdata != null)
			{

				foreach (XmlAttribute attribute in portdata.Attributes)
				{
					Debug.Log("loading a port named " + attribute.Value +" at index "+ attribute.Name.ToString());
					this.AddInputPort(attribute.Value);
				}
			}
			
			
		}
		
	}
}