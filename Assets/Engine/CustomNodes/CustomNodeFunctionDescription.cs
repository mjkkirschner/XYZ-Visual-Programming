using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Nodeplay.Nodes;
using System.Collections;

//originally forked from:https://github.com/DynamoDS/Dynamo/blob/master/src/DynamoCore/Core/CustomNodeDefinition.cs

namespace Nodeplay.Engine
{
	/// <summary>
	///     Compiler definition of a Custom Node.
	/// </summary>
	public class CustomNodeFunctionDescription
	{
		public CustomNodeFunctionDescription(
			Guid functionId,
			string displayName="",
			IList<NodeModel> nodeModels=null)
		{
			if (functionId == Guid.Empty)
				throw new ArgumentException(@"FunctionId invalid.", "functionId");
			
			nodeModels = nodeModels ?? new List<NodeModel>();
			
			#region Find outputs
			
			// Find output elements for the node
			
			var outputs = nodeModels.OfType<Output>().ToList();
			
			var topMost = new List<Tuple<int, NodeModel>>();
			
			List<string> outNames = new List<string>();
			
			// if we found output nodes, add their inputs
			// these will serve as the function output
			if (outputs.Any())
			{
				topMost.AddRange(
					outputs.Where(x => x.Inputs.First().IsConnected).Select(x => Tuple.New(0, x as NodeModel)));
				outNames = outputs.Select(x => x.Symbol).ToList();
			}
			//TODO bring thi back later
			/*else
			{
				outNames = new List<string>();
				
				// if there are no explicitly defined output nodes
				// get the top most nodes and set THEM as the output
				IEnumerable<NodeModel> topMostNodes = nodeModels.Where(node => node.IsTopMostDataNode);
				
				var rtnPorts =
					//Grab multiple returns from each node
					topMostNodes.SelectMany(
						topNode =>
						//If the node is a recursive instance...
						topNode is Function && (topNode as Function).Definition.FunctionId == functionId
						// infinity output
						? new[] {new {portIndex = 0, node = topNode, name = "∞"}}
					// otherwise, grab all ports with connected outputs and package necessary info
					: topNode.OutPortData
					.Select(
						(port, i) =>
						new {portIndex = i, node = topNode, name = port.NickName})
					.Where(x => !topNode.HasOutput(x.portIndex)));
				
				foreach (var rtnAndIndex in rtnPorts.Select((rtn, i) => new {rtn, idx = i}))
				{
					topMost.Add(Tuple.Create(rtnAndIndex.rtn.portIndex, rtnAndIndex.rtn.node));
					outNames.Add(rtnAndIndex.rtn.name ?? rtnAndIndex.idx.ToString());
				}
			}*/
			var nameDict = new Dictionary<string, int>();
			foreach (var name in outNames)
			{
				if (nameDict.ContainsKey(name))
					nameDict[name]++;
				else
					nameDict[name] = 0;
			}
			
			nameDict = nameDict.Where(x => x.Value != 0).ToDictionary(x => x.Key, x => x.Value);
			
			outNames.Reverse();
			
			var returnKeys = new List<string>();
			foreach (var name in outNames)
			{
				int amt;
				if (nameDict.TryGetValue(name, out amt))
				{
					nameDict[name] = amt - 1;
					returnKeys.Add(name == "" ? amt + ">" : name + amt);
				}
				else
					returnKeys.Add(name);
			}
			
			returnKeys.Reverse();
			
			#endregion
			
			#region Find inputs

			//Find function entry point, and then compile
			var inputNodes = nodeModels.OfType<Symbol>().ToList();
			var parameters = inputNodes.Select(x=>x.Parameter).ToList();;
			//will not support storing parameter types on the input nodes yet
			//TODO bring thiis back, maybe just using reflection and .net types in a dropdown...
			//TODO complete this feature
			//var parameters = inputNodes.Select(x => {var y = new ParameterInfo();y.ParameterType = x.Parameter.Type})
				//x.GetAstIdentifierForOutputIndex(0).Value, 
				//x.Parameter.Type, 
				//x.Parameter.DefaultValue));
			//var displayParameters = inputNodes.Select(x => x.Parameter.Name);
			
			#endregion
			
			FunctionBody = nodeModels.Where(node => !(node is Symbol));
			DisplayName = displayName;
			FunctionId = functionId;
			Parameters = parameters;
			ReturnKeys = returnKeys;
			DisplayParameters = parameters.Select(x=>x.Second.First).ToList();
			OutputNodes = topMost.Select(x => x.Second).Cast<Output>().ToList();
			DirectDependencies = nodeModels
				.OfType<CustomNodeWrapper>()
					.Select(node => node.Funcdef)
					.Where(def => def.FunctionId != functionId)
					.Distinct();
		}
		
		public static CustomNodeFunctionDescription MakeProxy(Guid functionId, string displayName)
		{
			Debug.Break();
			var def = new CustomNodeFunctionDescription(functionId, displayName);
			def.IsProxy = true;
			return def;
		}


		/// <summary>
		///     ports that trigger execution on this custom node
		/// </summary>
		public List<InputExecutionNode> InputExecutionNodes { get; private set; }

		/// <summary>
		///     ports that trigger execution on this custom node
		/// </summary>
		public List<OutPutExecutionNode> OutputExecutionNodes { get; private set; }

		/// <summary>
		///     Is this CustomNodeDefinition properly loaded?
		/// </summary>
		public bool IsProxy { get; private set; }
		
		/// <summary>
		///     Function name.
		/// </summary>
		public string FunctionName
		{
			get { return FunctionId.ToString().Replace("-", string.Empty); }
		}
		
		/// <summary>
		///     Function unique ID.
		/// </summary>
		public Guid FunctionId { get; private set; }
		
		/// <summary>
		///     User-friendly parameters
		/// </summary>
		public IEnumerable<string> DisplayParameters { get; private set; }
		
		/// <summary>
		///     Function parameters.
		/// </summary>
		public IEnumerable<Tuple<object,Tuple<string,System.Type>>> Parameters { get; private set; } 
		
		/// <summary>
		///     If the function returns a dictionary, this specifies all keys in
		///     that dictionary.
		/// </summary>
		public IEnumerable<string> ReturnKeys { get; private set; }
		
		/// <summary>
		///     NodeModels making up the body of the custom node.
		/// </summary>
		public IEnumerable<NodeModel> FunctionBody { get; private set; }
		
		/// <summary>
		///     Identifiers associated with the outputs of the custom node.
		/// </summary>
		public IEnumerable<Output> OutputNodes { get; private set; }
		
		/// <summary>
		///     User friendly name on UI.
		/// </summary>
		public string DisplayName { get; private set; }
		
		#region Dependencies
		
		public IEnumerable<CustomNodeFunctionDescription> Dependencies
		{
			get { return FindAllDependencies(new HashSet<CustomNodeFunctionDescription>()); }
		}
		
		public IEnumerable<CustomNodeFunctionDescription> DirectDependencies { get; private set; }
		
		private IEnumerable<CustomNodeFunctionDescription> FindAllDependencies(HashSet<CustomNodeFunctionDescription> dependencySet)
		{
			var query = DirectDependencies.Where(def => !dependencySet.Contains(def));
			
			foreach (var definition in query)
			{
				yield return definition;
				dependencySet.Add(definition);
				foreach (var def in definition.FindAllDependencies(dependencySet))
					yield return def;
			}
		}
		
		#endregion
	}
	
	/// <summary>
	///     Basic information about a custom node.
	/// </summary>
	public class CustomNodeInfo
	{
		public CustomNodeInfo(Guid functionId, string name, string category, string description, string path)
		{
			if (functionId == Guid.Empty)
				throw new ArgumentException(@"FunctionId invalid.", "functionId");
			
			FunctionId = functionId;
			Name = name;
			Category = category;
			Description = description;
			Path = path;
		}
		
		public Guid FunctionId { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public string Path { get; set; }
	}
}