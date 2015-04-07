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
	
	public class VariableInputOutputNodeModel : VariableInputNodeModel
	{
		protected override string GetPortName()
		{
			return "some port name" + Inputs.Count.ToString();
		}
		protected virtual string GetOutPortName()
		{
			return "OUTPUT" + Outputs.Count.ToString();
		}
		
		//instantiate and setup buttons for adding and removing ports
		public override GameObject BuildSceneElements()
		{
			var tempUI = base.BuildSceneElements();
			var inputdisplay = tempUI.transform.root.GetComponentInChildren<InputDisplay>().gameObject;
			var buttonPrefab = Resources.Load("LibraryButton") as GameObject;
			
			var addbutton = GameObject.Instantiate(buttonPrefab);
			var rembutton = GameObject.Instantiate(buttonPrefab);
			
			addbutton.GetComponentInChildren<Text>().text = "Add Output";
			rembutton.GetComponentInChildren<Text>().text = "Remove Output";
			
			addbutton.GetComponent<Button>().onClick.AddListener(() => { this.AddOutPutPort(GetOutPortName());});
			rembutton.GetComponent<Button>().onClick.AddListener(() => {
				this.RemoveOutputPort(Outputs.Select(x=>x.NickName).ToList().Last());});
			
			addbutton.transform.SetParent(inputdisplay.transform,false);
			rembutton.transform.SetParent(inputdisplay.transform,false);
			return tempUI;
		}
		
		//save and load are overidden to save ports created at runtime
		
		public override void Save(XmlDocument doc, XmlElement element)
			
		{
			base.Save(doc,element);
			
			var portData = doc.CreateElement("variableOutPorts");
			element.AppendChild(portData);
			//add an attribute foreach output port, can load this later to create ports in correct order
			if (Outputs != null){
				
				foreach(var port in Outputs)
				{	
					portData.SetAttribute("port"+ port.Index.ToString(),port.NickName);
				}
				
			}
			
		}
		
		public override void Load(XmlNode node)
		{
			base.Load(node);
			XmlNode portdata = null ;
			
			foreach(XmlNode subnode in node.ChildNodes)
			{
				if (subnode.Name == "variableOutPorts")
				{
					portdata = subnode;
					
				}
			}
			
			if (portdata != null)
			{
				
				foreach (XmlAttribute attribute in portdata.Attributes)
				{
					Debug.Log("loading a port named " + attribute.Value +" at index "+ attribute.Name.ToString());
					this.AddOutPutPort(attribute.Value);
				}
			}
			
			
		}
		
	}
}