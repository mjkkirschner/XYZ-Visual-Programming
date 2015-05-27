using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using Nodeplay.Interfaces;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nodeplay.UI
{
	//
	//this component watches for evaluation events on it's owner Nodemodel and creates a gameobject representation of the
	//the evaluation results for a particular eval of the node
	public class EvaluationResultsRenderer : MonoBehaviour, IContextable
	{
		public event Action UpdatingResultsPositions;
		public NodeModel Model;
		private List<GameObject> evaluationResults = new List<GameObject>();
		public IEnumerable<GameObject> EvaluationResulsts {get {return evaluationResults;}}
		private List<GameObject> evalResultsRoots = new List<GameObject>();
		// Use this for initialization
		void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();
			Model.Evaluated += HandleEvaluated;
			Model.explicitGraphExecution.GraphEvaluationStarted += HandleGraphEvaluationStarted;
			AddEvalRoot("first EvalRoot",0);
			Model.gameObject.AddComponent<BaseModelBoundingRenderer>();

		}

		#region IContextable implementation

		public List<Button> RequestContextButtons ()
		{
			var button = Instantiate(Resources.Load("LibraryButton")) as GameObject;
			button.GetComponentInChildren<Text>().text = "Toggle Evaluation History";
			button.GetComponentInChildren<Button>().onClick.AddListener(() => {toggleDisplay();} );
			return new List<Button>(){button.GetComponentInChildren<Button>()};
		}

		#endregion

		private void OnUpdatingResultsPositions()
		{
			if (UpdatingResultsPositions != null)
			{
				UpdatingResultsPositions();
			}
		}

		private GameObject AddEvalRoot(string name,int zindex)
		{	
			var evalResultsRoot = new GameObject(name);
			evalResultsRoots.Add(evalResultsRoot);
			evalResultsRoot.transform.SetParent(this.transform,false);
			evalResultsRoot.transform.localScale = new Vector3(.5f,.5f,.5f);
			evalResultsRoot.AddComponent<GridLayoutGroup>();
			evalResultsRoot.GetComponent<GridLayoutGroup>().spacing = new Vector2(2,2);
			evalResultsRoot.GetComponent<GridLayoutGroup>().cellSize = new Vector2(1,1);
			(evalResultsRoot.transform as RectTransform).sizeDelta = new Vector2(20,20);
			evalResultsRoot.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
			evalResultsRoot.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;
			evalResultsRoot.AddComponent<PositionWindowUnderRect>();
			evalResultsRoot.GetComponent<PositionWindowUnderRect>().PostitionUnderThese = Model.transform.Find("ToggleLabel(Clone)/Window").GetComponent<RectTransform>();
			evalResultsRoot.GetComponent<PositionWindowUnderRect>().PostPositionTranslation = new Vector3(0,0,zindex*5);

			//finally if the any evel root was not active, also make this one inactive...
			if (evalResultsRoots.Any(x=>x.activeSelf == false))
			{
				evalResultsRoot.SetActive(false);
			}
			return evalResultsRoot;
		}

		//when the graph starts being evaluated, cleanup all the past results we builts
		void HandleGraphEvaluationStarted ()
		{
			evaluationResults.ForEach(x=>GameObject.Destroy(x));
			evaluationResults.Clear();

			//clear each root that is not the first
			evalResultsRoots.Skip(1).ToList().ForEach(x=>GameObject.Destroy(x));
			evalResultsRoots.RemoveRange(1, evalResultsRoots.Count - 1);
		}

		//when the nodemodel is finished evaluating, then build a record of this evaluation
		//we'll store the resultant outputs, from the storedValueDict on the nodemodel and
		//render these in space near the node.
		void HandleEvaluated (object sender, EventArgs e)
		{

			var keys = Model.Outputs.Select(x=>x.NickName);
			var vals =keys.Select(x=>Model.StoredValueDict[x]).ToList();

			//we can also inject a component onto any val that is a type of gameobject, this component will let
			//the user inspect the gameobject in space....//possibly even calling methods on that object... etc etc...

			var gameobjects = vals.OfType<GameObject>().ToList();
			gameobjects.ForEach(x=>x.AddComponent<ObjectToEvaluation>().Init(this.GetComponent<NodeModel>()));

			//we can inject a reference back to this node so that selection of this node can highlight this geo,
			// or so that selection of the geo will highlist the particular evaluation that created it.

			//now foreach of these we want to build some representation in 3d, but it needs
			//to be obvious they outputs from the same eval on the same node.... we can just
			//build a new OutputPair... or simply just grab the output pair and save it... this
			//is not useful for geometry.... what about adding all of these items to a list, and
			// building an inspector, and inspecting that list... might work nicely, will show geometry...
			var evalResultParent = new GameObject(keys.ToString());
			evalResultParent.AddComponent<ExecutionVisualizationModel>();
			if (evalResultsRoots.Last().transform.childCount > 30)
			{
				AddEvalRoot("newroot",evalResultsRoots.Count+1);
			}
			evalResultParent.transform.SetParent(evalResultsRoots.Last().transform,false);

			evalResultParent.AddComponent<InspectorVisualization>();
			evaluationResults.Add(evalResultParent);
			evalResultParent.AddComponent<LayoutElement>();
			evalResultParent.AddComponent<Button>();
			evalResultParent.GetComponent<Button>().onClick.AddListener(()=> {populateInspector(evalResultParent,vals);} );

		}

		private void populateInspector(GameObject evalparent, List<object> vals){


			if (evalparent.transform.childCount > 0)
			{

				evalparent.transform.Cast<Transform>().ToList().ForEach(x=>GameObject.Destroy(x.gameObject));
			}
			else{

			evalparent.GetComponent<InspectorVisualization>().PopulateTopLevel(vals,0);
			StartCoroutine(calcRectSizeNextFrame());
			}
			OnUpdatingResultsPositions();
		}


				private IEnumerator calcRectSizeNextFrame()
				{
					yield return new WaitForEndOfFrame();
					var maxsize =   evaluationResults.Where(x=>x.transform.childCount>0).Select(y=>y.transform.GetChild(0)
					                                                                            .GetComponent<RectTransform>().sizeDelta * y.transform.GetChild(0).localScale.x)
						.Aggregate(Vector2.zero,(max,next) => Vector2.Max(max,next));
					evalResultsRoots.ForEach(x=>x.GetComponent<GridLayoutGroup>().cellSize = maxsize);
				}


		public void toggleDisplay()
		{
			if (evalResultsRoots.Any(x=>x.activeSelf == false))
			{
				evalResultsRoots.ForEach(x=>x.SetActive(true));
			}
			else
			{
				evalResultsRoots.ForEach(x=>x.SetActive(false));
			}
		}
		
	}
	
}