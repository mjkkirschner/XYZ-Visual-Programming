using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Nodeplay.Utils;
using System.Dynamic;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.Ast;

namespace Nodeplay.UI
{
	/// <summary>
	/// this class is responsible for acting as the root of the inspection tree
	/// it contains methods for generating the top level of the elements contained the tree
	/// for instantiating those elements, and organizing the visualzation of all elements under it
	/// </summary>

	public class InspectorVusualization : MonoBehaviour
	{

		public NodeModel Model;
		public List<GameObject> SubElement;
		public object TopLevelElement;


		// Use this for initialization
		void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();

		}


		private bool IsList(object o)
		{
			return o is IList &&
				   o.GetType().IsGenericType &&
				   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}

		private bool IsDictionary(object o)
		{
			return o is IDictionary &&
				   o.GetType().IsGenericType &&
				   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
		}


		private GameObject generateInspectableElementGameObject(object someObject)
		{
			var x = new GameObject(someObject.GetType().ToString());
			var inspectable = x.AddComponent<InspectableElement>();
			inspectable.ElementType = x.GetType();
			inspectable.reference = someObject;

			return x;
		}

		public void PopulateTopLevel(object inputObject)
		{
			if (IsList(inputObject))
			{
				TopLevelElement = inputObject;
				foreach (var item in (IEnumerable)inputObject)
				{
					var inspectabelgo = generateInspectableElementGameObject(item);
					inspectabelgo.transform.SetParent(this.transform);
				}
			}

			else if (IsDictionary(inputObject))
			{

				foreach (var pair in (IEnumerable)inputObject)
				{
					var realpair = DictionaryHelpers.CastFrom(pair);
					var key = realpair.Key;
					var value = realpair.Value;
				}
			}



			else
			{
				//reflect over this object and grab propeties if there are any
				if (inputObject is IDynamicMetaObjectProvider)
				{
					var names = new List<string>();
					var dynobj = inputObject as IDynamicMetaObjectProvider;
					if (dynobj != null)
					{
						
						names.AddRange(dynobj.GetMetaObject(Expression.Constant(dynobj)).GetDynamicMemberNames());
					}

					//filter names so that python private and builtin members do not show
					var filterednames = names.Where(x => x.StartsWith("__") != true).ToList();

					foreach (var name in filterednames)
					{

						var val = GetDynamicValue(objectinput, name);
						
						if (val != null)
						{
							membersandvals.Add(new DynamoDropDownItem(string.Format("{0}:{1}", name, val), val));
						}

					}


				}



				  // if object was not dynamic use regular reflection
				else
				{
					var propertyInfos = inputObject.GetType().GetProperties(
			   BindingFlags.Public | BindingFlags.NonPublic // Get public and non-public
			 | BindingFlags.Static | BindingFlags.Instance  // Get instance + static
			 | BindingFlags.FlattenHierarchy); // Search up the hierarchy



					foreach (var prop in propertyInfos.ToList())
					{
						var val = prop.GetValue(objectinput, null);
						membersandvals.Add(new DynamoDropDownItem(string.Format("{0}:{1}", prop.Name, val), val));
					}
				}


			}

		}
		


		//method for grabbing member values from a dynamic object, we use this for python objects...
		//  http://stackoverflow.com/questions/1926776/getting-a-value-from-a-dynamic-object-dynamically
		private static object GetDynamicValue(IronPython.Runtime.Binding.IPythonExpandable ob, string name)
		{
			CallSite<Func<CallSite, object, object>> site
				= CallSite<Func<CallSite, object, object>>.Create(ob.Context.LanguageContext.CreateGetMemberBinder(name,false));
				
			return site.Target(site, ob);
		}

	}
}
