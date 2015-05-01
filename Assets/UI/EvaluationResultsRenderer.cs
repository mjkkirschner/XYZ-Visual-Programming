using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Pathfinding.Serialization.JsonFx;
using Nodeplay.Interfaces;
using UnityEngine.UI;

namespace Nodeplay.UI
{
	//
	//this component watches for evaluation events on it's owner Nodemodel and creates a gameobject representation of the
	//the evaluation results for a particular eval of the node
	public class EvaluationResultsRenderer : MonoBehaviour, IContextable
	{

		public NodeModel Model;
		private List<GameObject> evaluationResults = new List<GameObject>();
		private GameObject evalResultsRoot;
		// Use this for initialization
		void Start()
		{
			Model = this.transform.root.GetComponentInChildren<NodeModel>();
			Model.Evaluated += HandleEvaluated;
			Model.explicitGraphExecution.GraphEvaluationStarted += HandleGraphEvaluationStarted;
			evalResultsRoot = new GameObject("evalRoot");
			evalResultsRoot.transform.SetParent(this.transform,false);
			evalResultsRoot.transform.localScale = new Vector3(.5f,.5f,.5f);
			evalResultsRoot.AddComponent<GridLayoutGroup>();
			evalResultsRoot.GetComponent<GridLayoutGroup>().spacing = new Vector2(2,2);
			evalResultsRoot.GetComponent<GridLayoutGroup>().cellSize = new Vector2(1,1);
			(evalResultsRoot.transform as RectTransform).sizeDelta = new Vector2(20,20);
			evalResultsRoot.GetComponent<GridLayoutGroup>().childAlignment = TextAnchor.MiddleRight;
			evalResultsRoot.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Vertical;

			//evalResultsRoot.GetComponent<GridLayoutGroup>().padding.left = 5;

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

		//when the graph starts being evaluated, cleanup all the past results we builts
		void HandleGraphEvaluationStarted ()
		{
			evaluationResults.ForEach(x=>GameObject.Destroy(x));
			evaluationResults.Clear();
		}

		//when the nodemodel is finished evaluating, then build a record of this evaluation
		//we'll store the resultant outputs, from the storedValueDict on the nodemodel and
		//render these in space near the node.
		void HandleEvaluated (object sender, EventArgs e)
		{

			var keys = Model.Outputs.Select(x=>x.NickName);
			var vals =keys.Select(x=>Model.StoredValueDict[x]).ToList();

			//now foreach of these we want to build some representation in 3d, but it needs
			//to be obvious they outputs from the same eval on the same node.... we can just
			//build a new OutputPair... or simply just grab the output pair and save it... this
			//is not useful for geometry.... what about adding all of these items to a list, and
			// building an inspector, and inspecting that list... might work nicely, will show geometry...
			var evalResultParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
			evalResultParent.GetComponent<Renderer>().material.color  = Color.red;
			evalResultParent.AddComponent<BaseModel>();
			evalResultParent.transform.SetParent(evalResultsRoot.transform,false);

			evalResultParent.AddComponent<InspectorVisualization>();
			evaluationResults.Add(evalResultParent);
			evalResultParent.GetComponent<InspectorVisualization>().PopulateTopLevel(vals,0);
			evalResultParent.AddComponent<LayoutElement>();

			var maxsize =   evaluationResults.Select(x=>x.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta * x.transform.GetChild(0).localScale.x)
				.Aggregate(Vector2.zero,(max,next) => Vector2.Max(max,next));
			evalResultsRoot.GetComponent<GridLayoutGroup>().cellSize = maxsize;
			Debug.Log(maxsize);

		}
		
		private void toggleDisplay()
		{
			if (evalResultsRoot.activeSelf == false)
			{
				evalResultsRoot.SetActive(true);
			}
			else
			{
				evalResultsRoot.SetActive(false);
			}
		}
		
	}
	
}