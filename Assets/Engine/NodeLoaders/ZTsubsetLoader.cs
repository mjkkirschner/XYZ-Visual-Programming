using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Nodes;
using System;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Reflection.Emit;
using Nodeplay.Engine;

/// <summary>
/// sections of this class forked from https://github.com/DynamoDS/Dynamo/blob/DynamoCoreModularization/src/DynamoCore/Core/DynamoLoader.cs
/// </summary>
public class ZTsubsetLoader : NodeModelLoader
{
	public Dictionary<string, FunctionDescription> functions { get; private set; }
	public ZTsubsetLoader()
		: base()
	{
		functions = new Dictionary<string, FunctionDescription>();

	}

	/// <summary>
	/// Enumerate the static methods of all types in an assembly, create nodemodels pointing to these methods
	/// and add them to the appmodels's
	/// dictionaries(this will populate the searchview).  Internally catches exceptions and sends the error 
	/// to the console.
	/// </summary>
	/// <Returns>The list of node types which are constructed from the methods loaded from this assembly</Returns>
	/// //TODO possibly change this back to using typeloadData or simplified version to embed search tags, name, etc in search view and appmodel easily
	public override List<Type> LoadNodesFromAssembly(
		Assembly assembly)
	{
		Debug.Log("inside load nodes from specific assembly: " + assembly.FullName);
		var nodeModelTypes = new List<Type>();


		if (assembly == null)
			throw new ArgumentNullException("assembly");

		Type[] loadedTypes = null;
		var loadedMethodDict = new Dictionary<Type, List<MethodInfo>>();
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

				//load all declared,static,public methods on the type
				loadedMethodDict[t] = t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static).ToList();
				//now we need to build a nodemodel type that represents each method
				foreach (var method in loadedMethodDict[t])
				{
					//http://stackoverflow.com/questions/9053440/create-type-at-runtime-that-inherits-an-abstract-class-and-implements-an-interfa
					AssemblyName asmName = new AssemblyName("ZeroTouchWrappers");
					string typename = t.FullName;
					AssemblyBuilder asmbuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
					ModuleBuilder modulebuilder = asmbuilder.DefineDynamicModule("loadedlib");
					TypeBuilder typebuilder = modulebuilder.DefineType(typename +method.Name + "Node");

					var @params = method.GetParameters();

					if(@params.Any(x=>x.IsDefined(typeof(ParamArrayAttribute), false)))
					{
						typebuilder.SetParent(typeof(VariableInputZTwrapperNode));
					}
					else
					{
						typebuilder.SetParent(typeof(ZTwrapperNode));
					}
					Type ztnode = typebuilder.CreateType();
					nodeModelTypes.Add(ztnode);
					//define a function descriptor that wraps the needed parameters, type, and method for a specific node
					var currentFunc = new FunctionDescription(method.GetParameters().ToList(), method, t,ztnode);
					//push this into the functions dictionary for this loader
					functions.Add(ztnode.FullName, currentFunc);

				}
				//TODO need to inject the methodinfo,typeinfo,and parameter info somehow or the type is really nothing....



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





}

