using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Nodeplay.Engine;
using Nodeplay.Nodes;
using UnityEngine;

//forked from https://github.com/DynamoDS/Dynamo/blob/master/src/DynamoCore/Models/CustomNodeWorkspaceModel.cs

namespace Nodeplay.Core
{	/// <summary>
/// this class describes a custom node graph
/// </summary>
	public class CustomNodeGraphModel : GraphModel
	{
		public Guid CustomNodeId
		{
			get { return customNodeId; }
			private set
			{
				if (value == customNodeId) 
					return;
				
				var oldId = customNodeId;
				customNodeId = value;
				OnFunctionIdChanged(oldId);
				OnDefinitionUpdated();
				OnInfoChanged();
				NotifyPropertyChanged("CustomNodeId");
			}
		}
		private Guid customNodeId;
		
		#region Contructors

		//private GraphModel(String name, IEnumerable<NodeModel> nodes,
		//                   IEnumerable<ConnectorModel> connectors, float x, float y, float z, AppModel appmodel)
		//{
		//TODO do I need this constructor?
	/*	public CustomNodeGraphModel(
			string name, string description, float x, float y, float z, Guid customNodeId, string fileName="")
			: this(
				name,
				description,
				Enumerable.Empty<NodeModel>(),
				x,
				y,
			customNodeId, elementResolver, fileName) { }*/

		public CustomNodeGraphModel(string name, string category, string description, Guid customNodeId,AppModel appmodel,string fileName ="" )
			:base(name,appmodel)
		{

			CustomNodeId = customNodeId;
			HasUnsavedChanges = false;
			Category = category;
			Description = description;
			
			PropertyChanged += OnPropertyChanged;
		}

		public CustomNodeGraphModel(
			string name, string category, string description, IEnumerable<NodeModel> nodes, IEnumerable<ConnectorModel> connectors, 
			float x, float y, float z, Guid customNodeId,AppModel appmodel, string fileName="") 
			: base(name,nodes,connectors, x, y,z,appmodel)
		{
			CustomNodeId = customNodeId;
			HasUnsavedChanges = false;
			Category = category;
			Description = description;
			
			PropertyChanged += OnPropertyChanged;
		}
		
		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName == "Name")
				OnInfoChanged();
			
			if (args.PropertyName == "Category" || args.PropertyName == "Description")
			{
				HasUnsavedChanges = true;
				OnInfoChanged();
			}
		}
		
		#endregion
		
		/// <summary>
		///     All CustomNodeDefinitions which this Custom Node depends on.
		/// </summary>
		public IEnumerable<CustomNodeFunctionDescription> CustomNodeDependencies
		{
			get
			{
				return Nodes
					.OfType<CustomNodeWrapper>()
						.Select(node => node.Funcdef)
						.Where(def => def.FunctionId != CustomNodeId)
						.Distinct();
			}
		}
		
		/// <summary>
		///     The definition of this custom node, based on the current state of this workspace.
		/// </summary>
		public CustomNodeFunctionDescription CustomNodeFunctionDescription
		{
			get
			{
				return new CustomNodeFunctionDescription(CustomNodeId, Name, Nodes);
			}
		}
		
		/// <summary>
		///     The information about this custom node, based on the current state of this workspace.
		/// </summary>
		public CustomNodeInfo CustomNodeInfo
		{
			get
			{
				return new CustomNodeInfo(CustomNodeId, Name, Category, Description, FileName);
			}
		}
		
		public void SetInfo(string newName = null, string newCategory = null, string newDescription = null, string newFilename = null)
		{
			PropertyChanged -= OnPropertyChanged;
			
			Name = newName??Name;
			Category = newCategory??Category;
			Description = newDescription??Description;
			FileName = newFilename??FileName;
			
			PropertyChanged += OnPropertyChanged;
			
			if (newName != null || newCategory != null || newDescription != null || newFilename != null)
				OnInfoChanged();
		}
		
		/// <summary>
		///     Search category for this workspace, if it is a Custom Node.
		/// </summary>
		public string Category
		{
			get { return category; }
			set
			{
				category = value;
				NotifyPropertyChanged("Category");
			}
		}
		private string category;
		
		/// <summary>
		///     A description of the workspace
		/// </summary>
		public string Description
		{
			get { return description; }
			set
			{
				description = value;
				NotifyPropertyChanged("Description");
			}
		}
		private string description;
		
		public override void OnNodesModified()
		{
			base.OnNodesModified();
			HasUnsavedChanges = true;
			OnDefinitionUpdated();
		}
		
		public event Action InfoChanged;
		protected virtual void OnInfoChanged()
		{
			Action handler = InfoChanged;
			if (handler != null) handler();
		}
		
		public event Action DefinitionUpdated;
		protected virtual void OnDefinitionUpdated()
		{
			var handler = DefinitionUpdated;
			if (handler != null) handler();
		}
		
		public event Action<Guid> FunctionIdChanged;
		protected virtual void OnFunctionIdChanged(Guid oldId)
		{
			var handler = FunctionIdChanged;
			if (handler != null) handler(oldId);
		}
		
		protected override bool SaveGraphModel(string newPath)
		{
			var originalPath = FileName;
			//this is going to use the base graphmodel save, //which is going to to 
			//attempt to serialze this graph using regular graph models code... I'm not sure if it will end up 
			//calling this populate xml or not...
			//TODO WATCH THIS CODE AND SEE WHERE IT RUNS
			//WHEN SAVING A CUSTOM NODE
			if (!base.SaveGraphModel(newPath))
				return false;
			
			// A SaveAs to an existing function id prompts the creation of a new 
			// custom node with a new function id
			if (originalPath != newPath)
			{
				// If it is a newly created node, no need to generate a new guid
				if (!string.IsNullOrEmpty(originalPath))
					CustomNodeId = Guid.NewGuid();
				
				// This comes after updating the Id, as if to associate the new name
				// with the new Id.
				SetInfo(Path.GetFileNameWithoutExtension(newPath));
			}
			
			return true;
		}
		
		protected override bool PopulateXmlDocument(XmlDocument document)
		{
			Debug.Log("<color=orange>file save:</color> inside populatexml doc on the customnode class");

			if (!base.PopulateXmlDocument(document))
				return false;
			
			var root = document.DocumentElement;
			if (root == null)
				return false;
			
			var guid = CustomNodeFunctionDescription != null ? CustomNodeFunctionDescription.FunctionId : Guid.NewGuid();
			root.SetAttribute("ID", guid.ToString());
			root.SetAttribute("Description", Description);
			root.SetAttribute("Category", Category);
			
			return true;
		}
		
		//protected override void SerializeSessionData(XmlDocument document, ProtoCore.Core core)
	//	{
			// Since custom workspace does not have any runtime data to persist,
			// do not allow base class to serialize any session data.
	//	}
	}
}