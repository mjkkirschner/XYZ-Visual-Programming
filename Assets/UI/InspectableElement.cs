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
using System.ComponentModel;

namespace Nodeplay.UI
{
	/// <summary>
	/// TODO may design this class as generic so we can keep track of the type
	/// and have it work with a monobehavior based class that points here
	/// </summary>
	[RequireComponent(typeof(LayoutElement))]
	[RequireComponent(typeof(EventConsumer))]
	//[RequireComponent(typeof(HorizontalLayoutGroup))]
	public class InspectableElement: UIBehaviour ,IPointerClickHandler,INotifyPropertyChanged
	{
		public NodeModel Model;
		public Type ElementType;
		public object Reference;
		private bool exposesubElements = false;
		public string Name;
		// Use this for initialization


		private Vector3 location;
		public Vector3 Location
			
		{
			get
			{
				return this.location;
				
			}
			
			set
			{
				if (value != this.location)
				{
					this.location = value;
					NotifyPropertyChanged("Location");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void NotifyPropertyChanged(String info)
		{
			Debug.Log("sending " + info + " change notification");
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		private void handleRectChanges(object sender, PropertyChangedEventArgs info)
		{
			if (info.PropertyName == "Location"){
				UpdateVisualization();
			}
		}
		
		protected override void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();

			this.PropertyChanged += handleRectChanges;

		}
		protected virtual void Update()
		{
			if (transform.hasChanged) {
				Location = this.gameObject.transform.position;
				transform.hasChanged = false;
			}

			
		}
		
		public void UpdateVisualization(){


			var visualization = searchforvisualization(this.Reference);
			//visualization.transform.position = this.transform.position;
			
			//TODO move the 3d visualization down or something, another possibility
			//is to feed it to a custom button that places the renderer next to text
			visualization.transform.SetParent(this.transform);
			//disable all colliders on these visualizations
			visualization.GetComponentsInChildren<Collider>().ToList().ForEach(x=>x.enabled = false);
		}

		private Vector3 calculateCentroid (List<Vector3>points)
		{
			Vector3 center = Vector3.zero;
			foreach (var point in points)
			{
				center = center + point;
			}
			center = center / (points.Count);
			return center;
		}

		private void drawlineToVisualization(Vector3 To)
		{

			Debug.Log("current drawing a line that represents:" + this.gameObject.name + Name);

			var line = new GameObject("viz line");
			line.transform.SetParent(this.transform.parent);
			line.AddComponent<LineRenderer>();
			var linerenderer = line.GetComponent<LineRenderer>();
			linerenderer.useWorldSpace = true;
			linerenderer.SetVertexCount(2);

			var from = this.GetComponentInChildren<Text>().transform.position;
			linerenderer.SetPosition(0,from);
			linerenderer.SetPosition(1,To);
			linerenderer.material = Resources.Load<Material>("LineMat");
			linerenderer.SetWidth(.05f,.05f);
		}

		//tentatively return a new gameobject that renders some representation of this object
		//probably based on its type, so we'll need a mapping from type to visualization
		private GameObject searchforvisualization(object objectToVisualize)
		{
			//if a gameobject then just copy the gameobject which will use its
			//renderer if it has one
			if (objectToVisualize is UnityEngine.GameObject)
			{

				if (((GameObject)objectToVisualize).GetComponent<Renderer>() != null)
					{
					//just return the actual object and we'll just move it.
					return Instantiate(((GameObject)objectToVisualize)) as GameObject ;
					}

				else{
					//this is a unityobject with no renderer, potentially many things that would be good to visualize
					//like colliders, images,sprites,text...prefabs,meshes,etc,etc
					return (new GameObject("unimplemented visualization"));
					}

			}

			else
			{
				//this is any other type not a unity object:
				//if a vector2 or 3

				if (objectToVisualize is Vector2 || objectToVisualize is Vector3)
				{
					//first load the grid object
					//var gridprefab = Resources.Load<GameObject>("Grid");
					//var grid = GameObject.Instantiate(gridprefab);
					//then place a point into the grid

					//if we get a point or vector, lets visualize it as a point, and draw
					// a line between the point and the inspectable element button

					var point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					point.transform.position = (Vector3)objectToVisualize;

					drawlineToVisualization(point.transform.position);
					return point;
				}


				//if a transform

				//if a list

				//if a dictionary

				//if a number

				//if a string

				//
				return (new GameObject("unimplemented visualization"));

			}

		
		}


		public void UpdateText(object pointer = null)
		{
			var fontsize = GetComponentInChildren<Text>().fontSize;
			if (pointer == null)
			{

				GetComponentInChildren<Text>().text = "<color=teal>"+ElementType.ToString()+"</color>"  + 
					" : \n " +"<color=orange>"+Name+"</color>"+
					": \n " + "<size="+(fontsize*1.5).ToString()+">"+Reference.ToString()+"</size>"; 
			}
			else
			{
				GetComponentInChildren<Text>().text = "<color=teal>"+pointer.GetType().ToString()+"</color>" +
					": \n "+ "<size="+(fontsize*1.5).ToString()+">"+ pointer.ToString()+"</size>"; 
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (exposesubElements == false){
				exposesubElements = true;
				populateNextLevel(this.Reference);
				this.GetComponent<Image>().color = Color.green;

			}
			else{

				var childrenRoot = transform.parent.GetChild(1);
				GameObject.DestroyImmediate(childrenRoot.gameObject);
				exposesubElements = false;
				this.GetComponent<Image>().color = Color.white;
			}
		}


		private void populateNextLevel(System.Object subTreeRoot)
		{
			//build a new wrapper for this next level
			var wrapper = new GameObject("sub_tree_wrapper");
			//wrapper.transform.position = this.transform.position;
			wrapper.transform.SetParent(this.transform.parent,false);
			wrapper.AddComponent<HorizontalLayoutGroup>();
			

			if (InspectorVisualization.IsList(subTreeRoot))
			{
				Debug.Log("inputobject is a list");
				foreach (var item in (IEnumerable)subTreeRoot)
				{
					var inspectabelgo = InspectorVisualization.generateInspectableElementGameObject(item,wrapper);

				}
			}
			
			else if (InspectorVisualization.IsDictionary(subTreeRoot))
			{
				Debug.Log("inputobject is a dictionary");
				foreach (var pair in (IEnumerable)subTreeRoot)
				{
					var realpair = DictionaryHelpers.CastFrom(pair);
					var key = realpair.Key;
					var value = realpair.Value;
					
					InspectorVisualization.generateInspectableElementGameObject(value,wrapper);

				}
			}
			
			
			
			else
			{
				Debug.Log("inputobject is a object");

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
						
						var value = InspectorVisualization.GetDynamicValue(dynobj, name);
						
						if (value != null)
						{
							 InspectorVisualization.generateInspectableElementGameObject(value,wrapper);

							
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
						if (prop.GetIndexParameters().Length > 0)
						{
							// Property is an indexer
							Debug.Log("this property is an indexed property, for now we won't reflect further");
							continue;
						}

						var value = prop.GetValue(subTreeRoot, null);
						InspectorVisualization.generateInspectableElementGameObject(value,wrapper,prop.Name);

						
					}
				}
				
				
				
			}



		}

	}
}
