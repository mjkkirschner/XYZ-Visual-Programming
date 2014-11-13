using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// object representing an input port.
/// </summary>
public class ConnectorModel : BaseModel
{
		public PortModel PStart { get; set; }
		public PortModel PEnd { get; set; }
		public ConnectorView View { get; set; }
		


		void OnEnable ()
		{
				//create a connector view
            View = this.gameObject.AddComponent<ConnectorView>();
				View.Model = this;
				

		}

		
		protected override void Update ()
		{
		
		}
		
		void OnDestroy(){
		//View.Destroy();
	// cleanup the view... TODO MOVE THIS LOGIC INTO A HANDELR THAT GETS CALLED ON DISCONNECT....s
	}


		

		public void init (PortModel start, PortModel end)
		{
		
				PStart = start;
				PEnd = end;
				Debug.Log (start);
				Debug.Log (end);
				// hook listeners on the connector view to the ports
				View.EndPort = PEnd;
				View.StartPort = PStart;
				start.PropertyChanged += View.HandlePortChanges;
				end.PropertyChanged += View.HandlePortChanges;
				
		}
		//TODO add method to verify the ports that this connector connects
        
    public override GameObject BuildSceneElements()
        {

            GameObject UI = new GameObject();
            UI.transform.localPosition = this.gameObject.transform.position;
            UI.transform.parent = this.gameObject.transform;
            var geo = View.redraw();
        //need to set these parents explicity since on first run redraw wont be able to 
        // nest these inside UI as UI is not created yet
            geo.ForEach(x => x.transform.parent = UI.transform);
            return UI;

        }

}