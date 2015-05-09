using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Nodeplay.UI;
using System.Collections.Generic;
using System.Linq;

public class ObjectToEvaluation : MonoBehaviour, IPointerClickHandler {

	private NodeModel evalOwner;
	private Material linematerial = Resources.Load<Material> ("LineMat");
	// Use this for initialization
	void Start () {


	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData)
	{
		if ( evalOwner == null)
		{
			throw new UnityException("missing eval owner, object visualizer not initialized");
		}
		DrawLineToEvaluation();
	}
	#endregion

	public void Init(NodeModel evalOwner)
	{
		this.evalOwner = evalOwner;
	}

	public void DrawLineToEvaluation()
	{
		//first make sure evaluation results are enabled


		//now find the correct eval result in the visualization
		var evalresult = evalOwner.GetComponent<EvaluationResultsRenderer>()
			.EvaluationResulsts.ToList().Find(x=>(x.GetComponent<InspectorVisualization>().TopLevelElement as IList).Contains(this.gameObject));

		//now draw a line from the object we're clicking to the 
		drawlineToVisualization(evalresult.transform.position);

	}

	private void drawlineToVisualization(Vector3 To)
	{
		
		Debug.Log("current drawing a line that represents:" + this.gameObject.name);
		
		var line = new GameObject("viz line");
		line.transform.SetParent(this.transform);
		line.tag = "visualization";
		line.AddComponent<LineRenderer>();
		var linerenderer = line.GetComponent<LineRenderer>();
		linerenderer.useWorldSpace = true;
		linerenderer.SetVertexCount(2);
		
		var from = this.transform.position;
		linerenderer.SetPosition(0,from);
		linerenderer.SetPosition(1,To);
		linerenderer.material = linematerial;
		linerenderer.SetWidth(.05f,.05f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
