﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DragState
{
		public static bool selection_changed (List<DragState> states)
		{
				if (states.Count > 1) {
						var selection1 = states [states.Count - 1].selection;
						var selection2 = states [states.Count - 2].selection;

						foreach (NodeSimple node in selection1) {
								var nodeindex = selection1.IndexOf (node);
								if (nodeindex < selection2.Count) {
										var node2 = selection2 [nodeindex];
										//TODO replace this with ID or GUID
										if (node.name != node2.name) {
												return true;
										}
								} else if (nodeindex >= selection2.Count) {
										return true;
								}
						}

				}
				return false;
				
						
		}

		public override string ToString ()
		{
				return string.Format ("{0},{1},{2},{3}", this._connecting, this._dragging, this._mousepos, this._selection);
		}

		public Boolean _connecting{ get; set; }

		public Boolean _dragging{ get; set; }

		public Vector2 _mousepos{ get; set; }

		private List<NodeSimple> _selection;

		public List<NodeSimple> selection { 
				get{ return _selection;} 
				// make sure we don't add duplicates to the selection
				set {


						_selection = value.Distinct ().ToList ();
				}
		}

		public DragState (Boolean connecting, Boolean dragging, Vector2 mousepos, List<NodeSimple> selection)
		{	
				_dragging = dragging;
				_mousepos = convert_eventcoords_to_Screenspace (mousepos);
				_connecting = connecting;
				_selection = selection;
		}

		public Vector2 convert_eventcoords_to_Screenspace (Vector2 orgcoords)
		{

				//Debug.Log (orgcoords);
				//Debug.Log (Camera.main.pixelHeight);
				return new Vector2 (orgcoords.x, Camera.main.pixelHeight - orgcoords.y);

		}

}

