// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ConnectorView:BaseView<ConnectorModel>
{

       
		public PortModel StartPort{ get; set; }
		public PortModel EndPort{ get; set; }
		public List<GameObject> TemporaryGeometry;
		protected GameObject geometryToRepeat;

		public void init (Vector3 startpoint, Vector3 endpoint)
		{
			if (!started)
			{
				Start();
			}
				redraw (startpoint, endpoint,geometryToRepeat);
                TemporaryGeometry.Select(x => x.transform.parent = this.gameObject.transform);
				
		}

		protected override void Start()
		{
			NormalScale = new Vector3(.2f, .2f, .2f);
			HoverScale = new Vector3(.2f, .2f, .2f);
			base.Start();
			geometryToRepeat = Model.UIsubgeo;
			
		}

        protected override void OnDestroy()
        {
            base.OnDestroy();
                if (TemporaryGeometry != null)
                {
                    TemporaryGeometry.ForEach(x => UnityEngine.GameObject.DestroyImmediate(x));
                }
                if (Model != null)
                {
                    Model.PStart.PropertyChanged -= HandlePortChanges;
                    Model.PEnd.PropertyChanged -= HandlePortChanges;
                }
            
        }

		
		
        public override void OnPointerUp(PointerEventData pointerdata)
        {
            
        }

        public override void OnPointerClick(PointerEventData pointerdata)
        {
          
        }


        public override void OnDrag(PointerEventData pointerdata)
        {
            
        }
        public List<GameObject> redraw()
        {
            var geo = redraw(StartPort.gameObject.transform.position, EndPort.gameObject.transform.position,geometryToRepeat);
            if (UI != null)
            {
                geo.ForEach(x => x.transform.parent = UI.transform);
            }
            return geo;
        }
		public List<GameObject> redraw(GameObject explicitgeoToRepeat)
		{
			var geo = redraw(StartPort.gameObject.transform.position, EndPort.gameObject.transform.position, explicitgeoToRepeat);
			if (UI != null)
			{
				geo.ForEach(x => x.transform.parent = UI.transform);
			}
			return geo;
		}

		public List<GameObject> redraw (Vector3 startPoint, Vector3 endpoint,GameObject geoToRepeat)
		{
				if (TemporaryGeometry != null) {
						TemporaryGeometry.ForEach (x => UnityEngine.GameObject.DestroyImmediate (x));
				}

				var range = Enumerable.Range (0, 100).Select (i => i / 100F).ToList ();
				var points = range.Select (x => Vector3.Slerp (startPoint, endpoint, x)).ToList ();
		
		
				var geos = points.Select (x => {
					var y = GameObject.Instantiate(geoToRepeat) as GameObject;
						GameObject.DestroyImmediate (y.GetComponent<Collider>());
						y.transform.position = x;
						y.transform.localScale = NormalScale;
						return y;}).ToList ();
		
				TemporaryGeometry = geos;
				return geos;
		}


		public void HandlePortChanges (object sender, EventArgs args)
		{
				Debug.Log (sender + " just sent event that port was modified and we should update the connector view");
				redraw ();
		}


}