using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
using System.Reflection;
using UnityEngine;
using System.IO;

/// <summary>
/// sections of this class forked from https://github.com/DynamoDS/Dynamo/blob/DynamoCoreModularization/src/DynamoCore/Core/DynamoLoader.cs
/// </summary>
public class NodeModelLoader 

{

	public HashSet<Assembly> LoadedAssemblies {get;set;}
	public HashSet<String> LoadedAssemblyNames {get;set;}

	public NodeModelLoader(){
		LoadedAssemblies = new HashSet<Assembly>();
		LoadedAssemblyNames = new HashSet<string>();
	}


	/*public delegate void AssemblyLoadedHandler(AssemblyLoadedEventArgs args);
	
	public class AssemblyLoadedEventArgs
	{
		public Assembly Assembly { get; private set; }
		
		public AssemblyLoadedEventArgs(Assembly assembly)
		{
			Assembly = assembly;
		}
	}
	
	public event AssemblyLoadedHandler AssemblyLoaded;
	
	private void OnAssemblyLoaded(Assembly assem)
	{
		if (AssemblyLoaded != null)
		{
			AssemblyLoaded(new AssemblyLoadedEventArgs(assem));
		}
	}
	*/

	/// <summary>
	/// Load all types which inherit from NodeModel whose assemblies are located in
	/// the bin/nodes directory. Add the types to the searchviewmodel and
	/// the controller's dictionaries.
	/// </summary>
	public List<Type> LoadNodeModels()
	{
		var loadedAssembliesByPath = new Dictionary<string, Assembly>();
		var loadedAssembliesByName = new Dictionary<string, Assembly>();
		
		// cache the loaded assembly information
		foreach (
			var assembly in 
			AppDomain.CurrentDomain.GetAssemblies())
		{
			try
			{
				loadedAssembliesByPath[assembly.Location] = assembly;
				loadedAssembliesByName[assembly.FullName] = assembly;
			}
			catch { }
		}

		//var result = new List<TypeLoadData>();
		//var result2 = new List<TypeLoadData>();


		var loadedTypes = new List<Type>();

		// going to look in all currently loaded assemblies for nodes, then we'll also look
		// in a specific folder of the resources or data folder, unsure on this path yet...
		var allNodeAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
		string path = Application.dataPath;
		Debug.Log(path);
		path = Path.Combine(path,"Nodes");
		Debug.Log(path);
		List<Assembly> allAssembliesinbuild = new List<Assembly>();
		foreach (string dll in Directory.GetFiles(path, "*.dll"))
		{
			allAssembliesinbuild.Add(Assembly.LoadFile(dll));
		}

		allNodeAssemblies.AddRange(allAssembliesinbuild);

		//iterate each assembly location
		foreach (var assemblyPath in allNodeAssemblies.Select(x=>x.Location).ToList())
		{
			Debug.Log("current assembly path is " + assemblyPath);
			//get the filename at each location and check it's not null
			var fn = Path.GetFileName(assemblyPath);
			if (fn == null)
				continue;
			Debug.Log("filename is " + fn);
			// if the assembly has already been loaded, then
			// skip it, otherwise cache it.
			if (LoadedAssemblyNames.Contains(fn))
				continue;
			
			LoadedAssemblyNames.Add(fn);
			
			try
			{
				Assembly assembly;
				if (!loadedAssembliesByPath.TryGetValue(assemblyPath, out assembly))
				{
					Debug.Log("about to load assembly from" + assemblyPath);
					assembly = Assembly.LoadFrom(assemblyPath);
					loadedAssembliesByName[assembly.GetName().Name] = assembly;
					loadedAssembliesByPath[assemblyPath] = assembly;
				}
				
			loadedTypes.AddRange(LoadNodesFromAssembly(assembly));
				LoadedAssemblies.Add(assembly);
				//TODO possibly readd event that fires when assemlbly loaded
				//OnAssemblyLoaded(assembly);
			}
			catch (BadImageFormatException)
			{
				//swallow these warnings.
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		//AppDomain.CurrentDomain.AssemblyResolve -= resolver;
		return loadedTypes;
	}


	/// <summary>
	///     Enumerate the types in an assembly and add them to the appmodels's
	///     dictionaries(this will populate the searchview).  Internally catches exceptions and sends the error 
	///     to the console.
	/// </summary>
	/// <Returns>The list of node types loaded from this assembly</Returns>
	/// //TODO possibly change this back to using typeloadData or simplified version to embed search tags, name, etc in search view and appmodel easily
	public List<Type> LoadNodesFromAssembly(
		Assembly assembly)
	{
		Debug.Log("inside load nodes from specific assembly: " + assembly.FullName);
		var nodeModelTypes = new List<Type>();

		if (assembly == null)
			throw new ArgumentNullException("assembly");
		
		Type[] loadedTypes = null;
		
		try
		{
			loadedTypes = assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException e)
		{
			Debug.Log("Could not load types.");
			Debug.LogException(e);
			foreach (var ex in e.LoaderExceptions)
			{
				Debug.Log("Dll Load Exception:");
				Debug.Log(ex.ToString());
			}
		}
		catch (Exception e)
		{
			Debug.Log("Could not load types.");
			Debug.LogException(e);
		}
		
		foreach (var t in (loadedTypes ?? Enumerable.Empty<Type>()))
		{
			try
			{
				//only load types that are in the right namespace, are not abstract
				//and have the elementname attribute
				if (IsNodeSubType(t))
				{
					Debug.Log(t.ToString() + "was a nodemodel");
					//TODO adding type, could replace with typeload class later
					nodeModelTypes.Add(t);
				}

			}
			catch (Exception e)
			{
				Debug.Log("Failed to load type from " + assembly.FullName);
				Debug.Log("The type was " + t.FullName);
				Debug.LogException(e);
			}
		}

		return nodeModelTypes;
	}

	/// <summary>
	///     Determine if a Type is a node.  Used by LoadNodesFromAssembly to figure
	///     out what nodes to load from other libraries (.dlls).
	/// </summary>
	/// <parameter>The type</parameter>
	/// <returns>True if the type is node.</returns>
	public static bool IsNodeSubType(Type t)
	{
		//Debug.Log("checking if type " + t.ToString() + "is a nodemodel ");
		return //t.Namespace == "Dynamo.Nodes" &&
			!t.IsAbstract &&
				t.IsSubclassOf(typeof(NodeModel));

	}

}

