using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GuiState
{
		public static bool selection_changed (List<GuiState> states)
		{
				if (states.Count > 1) {
						var selection1 = states [states.Count - 1].Selection;
						var selection2 = states [states.Count - 2].Selection;

						foreach (GameObject item in selection1) {
								var itemindex = selection1.IndexOf (item);
								if (itemindex < selection2.Count) {
										var item2 = selection2 [itemindex];
										//TODO replace this with ID or GUID
										if (item.name != item2.name) {
												return true;
										}
								} else if (itemindex >= selection2.Count) {
										return true;
								}
						}

				}
				return false;
				
						
		}

		public override string ToString ()
		{
				return string.Format ("{0},{1},{2},{3}", this.Connecting, this.Dragging, this.MousePos, this.Selection, this.DoubleClicked);
		}
		
		public Boolean DoubleClicked{ get; set; }
		public Boolean Connecting{ get; set; }
		public Boolean Dragging{ get; set; }
		public Vector2 MousePos{ get; set; }
		private List<GameObject> _selection;
		public List<GameObject> Selection { 

				get{ return _selection;} 
				// make sure we don't add duplicates to the selection
				set {


						_selection = value.Distinct ().ToList ();
				}
		}

		public GuiState (Boolean connecting, Boolean dragging, Vector2 mousepos, List<GameObject> selection, Boolean doubleclicked)
		{	
				Dragging = dragging;
				MousePos = convert_eventcoords_to_Screenspace (mousepos);
				Connecting = connecting;
				Selection = selection;
				DoubleClicked = doubleclicked;
		}	
    //Beware, we store all mouse positions in screenspace because the GUI is 3d...
		public Vector2 convert_eventcoords_to_Screenspace (Vector2 orgcoords)
		{

				return new Vector2 (orgcoords.x, Camera.main.pixelHeight - orgcoords.y);

		}

}

