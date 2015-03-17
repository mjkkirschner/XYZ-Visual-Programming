using UnityEngine;
using System.Collections.Generic;
using System.Linq;



    public class ExecutionConnectorModel:ConnectorModel
    {

		public override GameObject BuildSceneElements()
		{
			GameObject UI = new GameObject();
			UI.transform.localPosition = this.gameObject.transform.position;
			UI.transform.parent = this.gameObject.transform;
			UIsubgeo = Resources.Load<GameObject>("exec_connector");
			var geo = View.redraw(UIsubgeo);
			//need to set these parents explicity since on first run redraw wont be able to 
			// nest these inside UI as UI is not created yet
			geo.ForEach(x => x.transform.parent = UI.transform);
			return UI;
		}

	protected override void OnEnable ()
	{
		//create a connector view
		View = this.gameObject.AddComponent<ExecutionConnectorView>();
		View.Model = this;
		
	}

    }

