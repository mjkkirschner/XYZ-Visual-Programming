using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Nodeplay.Utils;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;


namespace Nodeplay.UI
{
	/// <summary>
	/// TODO may design this class as generic so we can keep track of the type
	/// and have it work with a monobehavior based class that points here
	/// </summary>
	[RequireComponent(typeof(LayoutElement))]
	[RequireComponent(typeof(EventConsumer))]
	//[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class InspectableElement: UIBehaviour ,IPointerClickHandler
	{
		public NodeModel Model;
		public GameObject tempgeo;
		public Type ElementType;
		public object reference;
		private bool exposesubElements = false;
		// Use this for initialization
		protected override void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();
			GetComponent<LayoutElement>().minHeight =1;
			GetComponent<LayoutElement>().preferredHeight = 1;
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			if (exposesubElements == false){
				populateNextLevel(this.reference);
				exposesubElements = true;
			}
			else{

				var children  = this.GetComponentsInChildren<Transform>().ToList();
				children.ForEach(x=>GameObject.Destroy(x));
				exposesubElements = false;
			}
		}


		private void populateNextLevel(System.Object subTreeRoot)
		{


			if (InspectorVusualization.IsList(subTreeRoot))
			{
				Debug.Log("inputobject is a list");
				foreach (var item in (IEnumerable)subTreeRoot)
				{
					var inspectabelgo = InspectorVusualization.generateInspectableElementGameObject(item,this.gameObject);
					inspectabelgo.transform.SetParent(this.transform);
				}
			}
			
			else if (InspectorVusualization.IsDictionary(subTreeRoot))
			{
				Debug.Log("inputobject is a dictionary");
				foreach (var pair in (IEnumerable)subTreeRoot)
				{
					var realpair = DictionaryHelpers.CastFrom(pair);
					var key = realpair.Key;
					var value = realpair.Value;
					
					var inspectabelgo = InspectorVusualization.generateInspectableElementGameObject(value,this.gameObject);
					inspectabelgo.transform.SetParent(this.transform);
				}
			}
			
			
			
			else
			{
				Debug.Log("inputobject is a object");
				//because this is the top level, we wont reflect over this object
				//but instead just generate an element that represents it as the root.
				
				if (subTreeRoot is IDynamicMetaObjectProvider)
				{
					Debug.Log("inputobject is a dynamic object");
					var names = new List<string>();
					var dynobj = subTreeRoot as IronPython.Runtime.Binding.IPythonExpandable;
					if (dynobj != null)
					{
						
						names.AddRange(dynobj.Context.LanguageContext.GetMemberNames(Expression.Constant(dynobj)));
					}
					
					//filter names so that python private and builtin members do not show
					var filterednames = names.Where(x => x.StartsWith("__") != true).ToList();
					
					foreach (var name in filterednames)
					{
						
						var value = InspectorVusualization.GetDynamicValue(dynobj, name);
						
						if (value != null)
						{
							var inspectabelgo = InspectorVusualization.generateInspectableElementGameObject(value,this.gameObject);
							inspectabelgo.transform.SetParent(this.transform);
							
						}
						
						
					}
					
				}
				
				// if object was not dynamic use regular reflection
				else
				{
					Debug.Log("inputobject is a non dynamic object");
					var propertyInfos = subTreeRoot.GetType().GetProperties(
						BindingFlags.Public | BindingFlags.NonPublic // Get public and non-public
						| BindingFlags.Static | BindingFlags.Instance  // Get instance + static
						| BindingFlags.FlattenHierarchy); // Search up the hierarchy
					
					
					
					foreach (var prop in propertyInfos.ToList())
					{
						var value = prop.GetValue(subTreeRoot, null);
						var inspectabelgo = InspectorVusualization.generateInspectableElementGameObject(value,this.gameObject);
						inspectabelgo.transform.SetParent(this.transform);
						
					}
				}
				
				
				
			}



		}

	}
}
