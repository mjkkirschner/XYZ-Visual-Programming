using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System.Collections;
using UnityEngine.UI;
using Nodeplay.UI.Utils;

/// <summary>
/// class that is responsible for defining how a node looks
/// this class instantiates gameobjects, geometry, and UI elements into the scene
/// it's possible that an entire node will be serialzed as a prefab and will not require
/// running the nodeview to be instantiated
/// </summary>
public class NodeView : BaseView<NodeModel>{


   
    //variable to help situate projection of mousecoords into worldspace
    private float dist_to_camera;
    private Color originalcolor;

    protected override void Start()
    {
        base.Start();
        UI = Model.BuildSceneElements();
        originalcolor = UI.renderer.material.color;
        Debug.Log("just started NodeView");
        
    }

    public void OnEvaluated(object sender, EventArgs e)
    {

        StartCoroutine("Blunk");
        //subclass this component so we can just look for the output box
        //need to marshal or implement to_string per output type somehow
        UI.GetComponentInChildren<Text>().text = Model.StoredValue.ToJSONstring();
    }

     IEnumerator Blink()
    {
        for (float f = 0; f < 1; f = f + .05f)
        {
            UI.renderer.material.color = Color.Lerp(originalcolor, Color.red, f);
            yield return null;
        }
    }

     IEnumerator Blunk()
     {
         for (float f = 0; f < 1; f = f + .05f)
         {
             UI.renderer.material.color = Color.Lerp(Color.red, originalcolor, f);
             yield return null;
         }
     }

    public void OnEvaluation(object sender, EventArgs e)
    {
        StartCoroutine("Blink");
    }

    public override GuiState MyOnMouseUp(GuiState current_state)
    {
        // if we're connecting to this node, then add this node
        // to the target list of each of the nodes in the selection.

        Debug.Log("Mouse up event handler called");
        var newState = new GuiState(false, false, current_state.MousePos, new List<GameObject>(), false);
        //GuiTest.statelist.Add (newState);
        return newState;


    }
    //handler for dragging node event//
    public override GuiState MyOnMouseDrag(GuiState current_state)
    {
        GuiState newState = current_state;
        //Debug.Log("drag even handler on node view");
        //TODO change this selection handler to filter events from gameobjects that are children of this object or tagged UI
        if (current_state.Selection.Contains(this.gameObject) || current_state.Selection.Contains(UI))
        {				// If doing a mouse drag with this component selected...
            // since we are in 3d space now, we need to conver this to a vector3...
            // for now just use the z coordinate of the first object

            // get the hit world coord
            var pos = HitPosition(this.gameObject);

            // project from camera through mouse currently and use same distance
            Vector3 to_point = ProjectCurrentDrag(dist_to_camera);

            // move object to new coordinate
            //TODO might want to do this by applying a force
            // or starting a coroutine that moves the node smoothly
            this.gameObject.transform.position = to_point;
            newState = new GuiState(false, true, Event.current.mousePosition, current_state.Selection, false);
            GuiTest.statelist.Add(newState);
            Event.current.Use();

        }
        return newState;


    }
    //handler for clicks
    public override GuiState MyOnMouseDown(GuiState current_state)
    {
        //Debug.Log("mouse down event handler called");
        // check if this node was actually clicked on
        if (HitTest(this.gameObject, current_state) || HitTest(UI,current_state))
        {
            Debug.Log("I" + this.name + " was just clicked");
            dist_to_camera = Vector3.Distance(this.transform.position, Camera.main.transform.position);

            Debug.Log(current_state);

            // check the dragstate from the GUI, either this is a double click
            // or a selection click
            // or possibly a click on nothing
            if (current_state.DoubleClicked == false)
            {

                // add this node to the current selection
                // update the drag state
                // store this drag state in the list of all dragstates

                List<GameObject> new_sel = (new List<GameObject>(current_state.Selection));
                new_sel.Add(this.gameObject);
                var newState = new GuiState(current_state.Connecting, current_state.Dragging, current_state.MousePos, new_sel, false);
                GuiTest.statelist.Add(newState);
                GeneratedDragState = newState;


            }
            else
            {
                //a double click occured on a node
                Debug.Log("I" + this.name + " was just DOUBLE clicked");

            }

            // finally return the new dragstate(not what this function should return)
            // since we actually have the dragstate stored
            // it might make more sense not to allow the Node to 
            // store the state on the GUI...
            return GeneratedDragState;

            //TODO OVERHAUL STATE STREAM	
            // this logic is important and ugly, we need to enforce that the callbacks return null if the event was not intended for that object so we don't
            // store superflous states... this needs to be redesigned asap
        }
        else
        {
            return null;
        }
    }

    public override void onGuiRepaint()
    {	
    }


}
