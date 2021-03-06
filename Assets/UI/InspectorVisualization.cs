﻿using System;
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
using UnityEngine.EventSystems;

namespace Nodeplay.UI
{
	/// <summary>
	/// this class is responsible for acting as the root of the inspection tree
	/// it contains methods for generating the top level of the elements contained the tree
	/// for instantiating those elements, and organizing the visualzation of all elements under it
	/// </summary>
	public class InspectorVisualization : MonoBehaviour
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

		/// <summary>
		/// where main work is done of generating a new inspectable object and setting its properties, the object is then made a child of the parent
		/// 
		/// </summary>
		/// 
		/// <param name="someObject"></param>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static GameObject generateInspectableElementGameObject(object someObject, GameObject parent, string name = null)
		{
			if (someObject == null){
				//GameObject.Destroy(parent);
				return null;
			}

			var wrapper = new GameObject("subelement_wrapper");
			//wrapper.transform.position = parent.transform.position;
			wrapper.transform.SetParent(parent.transform,false);
			wrapper.AddComponent<VerticalLayoutGroup>();
			//wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(1,1);
			//wrapper.transform.Rotate(0,90,0);

			//TODO replace with call to specific item type depending on item system.type
			var element = Resources.Load<GameObject>("listele");

			var instantiatedelement = GameObject.Instantiate(element) as GameObject;
			instantiatedelement.name = (someObject.GetType().ToString());
			//instantiatedelement.transform.position = wrapper.transform.position;

			instantiatedelement.transform.SetParent(wrapper.transform, false);

			var inspectable = instantiatedelement.AddComponent<InspectableElement>();
			inspectable.ElementType = someObject.GetType();
			inspectable.Reference = someObject;
			inspectable.Name = name;
			Debug.Log("building inspectable element representing: " + someObject.ToString());

			


			//TODO extract constants, fix this mess, possibly recalc based on distance to camera
			if (parent.transform.parent.GetChild(0).GetComponentInChildren<Text>().fontSize == 14)
			{
				//we are still creating the root visualization, so just set the size to 500
				parent.transform.parent.GetChild(0).GetComponentInChildren<Text>().fontSize = 500;
			}
			else
			{
				//in all other cases halve it
					var parentfontsize = parent.transform.parent.GetChild(0).GetComponentInChildren<Text>().fontSize;
					inspectable.GetComponentInChildren<Text>().fontSize = parentfontsize / 2;
			}
		
			
			if (someObject == null || ReferenceEquals(someObject,null) ||someObject.ToString() == "null" )
			{
				Debug.Log("in here mofo");
				inspectable.GetComponentInChildren<Text>().fontSize /= 2;
				instantiatedelement.GetComponent<Image>().color = Color.red;
			}

			//if the newly instantiated element is a list lets open it up by sending a click event
			if (InspectorVisualization.IsList(someObject))
			{
				Debug.Log("the item was a list so open it");
				ExecuteEvents.Execute<IPointerClickHandler>(instantiatedelement, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
				
			}
			
			// when the text is updated the layout is regenerated, this might happen on the next frame
			// and so lines need to be drawn, afer that, it's best if the visualizations are hooked to an event
			// watching for changes on the node or the 
			inspectable.UpdateText();
			inspectable.UpdateVisualization();
			return instantiatedelement;
		}



		public void PopulateTopLevel(object inputObject)
		{

			var wrapper = new GameObject("root_wrapper");
			wrapper.AddComponent<PositionWindowUnderPorts>();
			wrapper.transform.position = new Vector3(0,0,0);
			wrapper.transform.SetParent(this.transform,false);
			wrapper.AddComponent<VerticalLayoutGroup>();
			//wrapper.GetComponent<RectTransform>().sizeDelta = new Vector2(1,1);
			wrapper.AddComponent<Canvas>();
			wrapper.GetComponent<Canvas>().worldCamera = Camera.main;
			wrapper.AddComponent<GraphicRaycaster>();
			wrapper.GetComponent<RectTransform>().localScale = new Vector3(.0003f,.0003f,.0003f);
			var fitter = wrapper.AddComponent<ContentSizeFitter>();
			fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize; 
			//wrapper.transform.Rotate(0,90,0);
			TopLevelElement = inputObject;
			/*if (IsList(inputObject))
			{

				/*Debug.Log("inputobject is a list");
				foreach (var item in (IEnumerable)inputObject)
				{

					var inspectabelgo = generateInspectableElementGameObject(item,wrapper);


				}
			}

			else*/ if (IsDictionary(inputObject))
			{
				Debug.Log("inputobject is a dictionary");
				foreach (var pair in (IEnumerable)inputObject)
				{
					var realpair = DictionaryHelpers.CastFrom(pair);
					var key = realpair.Key;
					var value = realpair.Value;

					var inspectabelgo = generateInspectableElementGameObject(value,wrapper);

				}
			}



			else
			{
				Debug.Log("inputobject is a object");
				//because this is the top level, we wont reflect over this object
				//but instead just generate an element that represents it as the root.

				var inspectabelgo = generateInspectableElementGameObject(inputObject,wrapper);


			}

		}
		
		// this method will process renderableitems, extracting meshes and other representations that can be rendered
		// and place them near the inspectableItems representing them
		// might have methods for points,vectors,meshes,lines,images,etc etc.
		private GameObject processItem(){
			throw new NotImplementedException();
		}

		//method for grabbing member values from a dynamic object, we use this for python objects...
		//still untested
		//, modified from  http://stackoverflow.com/questions/1926776/getting-a-value-from-a-dynamic-object-dynamically
		public static object GetDynamicValue(IronPython.Runtime.Binding.IPythonExpandable ob, string name)
		{
			CallSite<Func<CallSite, object, object>> site
				= CallSite<Func<CallSite, object, object>>.Create(ob.Context.LanguageContext.CreateGetMemberBinder(name,false));
				
			return site.Target(site, ob);
		}

	}
}
