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
	[RequireComponent(typeof(VerticalLayoutGroup))]
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


		public static bool IsList(object o)
		{
			return o is IList || o is IList &&
				   o.GetType().IsGenericType &&
				   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}

		public static bool IsDictionary(object o)
		{
			return o is IDictionary &&
				   o.GetType().IsGenericType &&
				   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
		}


		public static GameObject generateInspectableElementGameObject(object someObject,GameObject parent)
		{
			//TODO replace with call to specific item type depending on item system.type
			var x = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			x.name = (someObject.GetType().ToString());
			x.transform.position = parent.transform.position;
			var inspectable = x.AddComponent<InspectableElement>();
			inspectable.ElementType = x.GetType();
			inspectable.reference = someObject;
			Debug.Log("building inspectable element representing: " + someObject.ToString());
			//this.SubElement.Add(x);

			return x;
		}

		public void PopulateTopLevel(object inputObject)
		{

			TopLevelElement = inputObject;
			if (IsList(inputObject))
			{
				Debug.Log("inputobject is a list");
				foreach (var item in (IEnumerable)inputObject)
				{
					var inspectabelgo = generateInspectableElementGameObject(item,this.gameObject);
					inspectabelgo.transform.SetParent(this.transform);
				}
			}

			else if (IsDictionary(inputObject))
			{
				Debug.Log("inputobject is a dictionary");
				foreach (var pair in (IEnumerable)inputObject)
				{
					var realpair = DictionaryHelpers.CastFrom(pair);
					var key = realpair.Key;
					var value = realpair.Value;

					var inspectabelgo = generateInspectableElementGameObject(value,this.gameObject);
					inspectabelgo.transform.SetParent(this.transform);
				}
			}



			else
			{
				Debug.Log("inputobject is a object");
				//because this is the top level, we wont reflect over this object
				//but instead just generate an element that represents it as the root.

				var inspectabelgo = generateInspectableElementGameObject(inputObject,this.gameObject);
				inspectabelgo.transform.SetParent(this.transform);

			}

		}
		


		//method for grabbing member values from a dynamic object, we use this for python objects...
		//  http://stackoverflow.com/questions/1926776/getting-a-value-from-a-dynamic-object-dynamically
		public static object GetDynamicValue(IronPython.Runtime.Binding.IPythonExpandable ob, string name)
		{
			CallSite<Func<CallSite, object, object>> site
				= CallSite<Func<CallSite, object, object>>.Create(ob.Context.LanguageContext.CreateGetMemberBinder(name,false));
				
			return site.Target(site, ob);
		}

	}
}
