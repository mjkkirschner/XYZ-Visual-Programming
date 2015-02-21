
//hoping I won't need to use this, instead use the methods on the manager and custmnode graph model
/*
using System;
using System.Linq;
using System.Xml;

	/// <summary>
	///     Xml Loader for Custom Nodes.
	/// </summary>
	public class CustomNodeLoader
	{
		private readonly ICustomNodeSource customNodeManager;
		private readonly bool isTestMode;
		
		public CustomNodeLoader(ICustomNodeSource customNodeManager, bool isTestMode = false)
		{
			this.customNodeManager = customNodeManager;
			this.isTestMode = isTestMode;
		}
		
		public Function CreateNodeFromXml(XmlElement nodeElement, SaveContext context)
		{
			XmlNode idNode =
				nodeElement.ChildNodes.Cast<XmlNode>()
					.LastOrDefault(subNode => subNode.Name.Equals("ID"));
			
			if (idNode == null || idNode.Attributes == null) 
				return null;
			
			string id = idNode.Attributes[0].Value;
			
			string nickname = nodeElement.Attributes["nickname"].Value;
			
			Guid funcId;
			if (!Guid.TryParse(id, out funcId))
				funcId = GuidUtility.Create(GuidUtility.UrlNamespace, nickname);
			
			var node = customNodeManager.CreateCustomNodeInstance(funcId, nickname, isTestMode);
			node.Deserialize(nodeElement, context);
			return node;
		}
	}
*/