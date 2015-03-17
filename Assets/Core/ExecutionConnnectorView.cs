using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ExecutionConnectorView:ConnectorView
{

	public override List<GameObject> redraw()
	{
		var StartAttachment = StartPort.gameObject.transform.GetChild(0).Find("port").GetComponent<Renderer>().bounds.center;
		var EndAttachment = EndPort.gameObject.transform.GetChild(0).Find("port").GetComponent<Renderer>().bounds.center;

		var geo = redraw(StartAttachment ,EndAttachment,geometryToRepeat);
		if (UI != null)
		{
			geo.ForEach(x => x.transform.parent = UI.transform);
		}
		return geo;
	}
	public override List<GameObject> redraw(GameObject explicitgeoToRepeat)
	{
		var StartAttachment = StartPort.gameObject.transform.GetChild(0).Find("port").GetComponent<Renderer>().bounds.center;
		var EndAttachment = EndPort.gameObject.transform.GetChild(0).Find("port").GetComponent<Renderer>().bounds.center;
		
		var geo = redraw(StartAttachment ,EndAttachment,explicitgeoToRepeat);
		if (UI != null)
		{
			geo.ForEach(x => x.transform.parent = UI.transform);
		}
		return geo;
	}
	

}