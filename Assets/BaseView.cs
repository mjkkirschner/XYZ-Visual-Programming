using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;

//TODO probably should repleace this with a nongeneric abstract base class
public class BaseView<M> : MonoBehaviour, Iinteractable, INotifyPropertyChanged where M:BaseModel
{
    //because views maybe created or destroyed multiple times per frame,
    //they may be destroyed before start finishes running
    protected Boolean started = false;
    public NodeManager NodeManager;
    public GuiTest GuiManager;
    public GuiState GeneratedDragState;
    public event PropertyChangedEventHandler PropertyChanged;
    protected float dist_to_camera;
    public M Model;

    protected virtual void NotifyPropertyChanged(String info)
    {
        Debug.Log("sending some property change notification");
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    protected virtual void Start()
    {   //TODO contract for hierarchy
        // we always search the root gameobject of this view for the model,
        // need to enforce this contract somehow, I think can use requires component.
        Model = this.gameObject.GetComponent<M>();

        dist_to_camera = Vector3.Distance(this.gameObject.transform.position, Camera.main.transform.position);
        // nodemanager manages nodes - like a workspacemodel
        NodeManager = GameObject.FindObjectOfType<NodeManager>();
        // guimanager - like a GUIcontroller
        GuiManager = GameObject.FindObjectOfType<GuiTest>();

        //setup callbacks for all objects inheriting from base view
        //todo can probably get rid of the interactable interface
        GuiManager.onMouseDown += new GuiTest.Mouse_Down(this.MyOnMouseDown);
        GuiManager.onMouseUp += new GuiTest.Mouse_Up(this.MyOnMouseUp);
        GuiManager.onMouseDrag += new GuiTest.Mouse_Drag(this.MyOnMouseDrag);
        GuiManager.onGuiRepaint += new GuiTest.GuiRepaint(this.onGuiRepaint);

        Debug.Log("just started BaseView");
        started = true;
    }

    protected virtual void OnDestroy()
    {
        if (!started)
        {
            Start();
        }

        GuiManager.onMouseDown -= (this.MyOnMouseDown);
        GuiManager.onMouseUp -= (this.MyOnMouseUp);
        GuiManager.onMouseDrag -= (this.MyOnMouseDrag);
        GuiManager.onGuiRepaint -= (this.onGuiRepaint);
        Debug.Log("just turned off BaseView and disconnected gui handlers");

    }

    public static Vector3 ProjectCurrentDrag(float distance)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var output = ray.GetPoint(distance);
        return output;
    }

    public Vector3 HitPosition(GameObject go_to_test)
    {

        // return the coordinate in world space where hit occured

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Mouse Down Hit " + hit.collider.gameObject);
            //Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.red, 2.0f);
            if (hit.collider.gameObject == go_to_test.gameObject)
            {
                return hit.point;
                // I was previoulsy returning hit.barycenter ... triangle?
            }

        }
        return go_to_test.transform.position;
    }
    /// <summary>
    /// check if we hit the current node to test, use the state to extract mouseposition
    /// </summary>
    /// <returns><c>true</c>, if test was hit, <c>false</c> otherwise.</returns>
    /// <param name="go_to_test">Node_to_test.</param>
    /// <param name="state">State.</param>
    public bool HitTest(GameObject go_to_test, GuiState state)
    {
        // raycast from the camera through the mouse and check if we hit this current
        // node, if we do return true

        Ray ray = Camera.main.ScreenPointToRay(state.MousePos);
        var hit = new RaycastHit();


        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Mouse Down Hit  " + hit.collider.gameObject);
            //Debug.DrawRay(ray.origin, ray.direction * 1000000);
            if (hit.collider.gameObject == go_to_test.gameObject)
            {
                return true;
            }

        }
        return false;
    }

    public virtual GuiState MyOnMouseUp(GuiState current_state)
    {
        Debug.Log("Mouse up event handler called");
        var newState = new GuiState(false, false, current_state.MousePos, new List<GameObject>(), false);
        //todo figure out how to deal with this situation - adding multiple upstates from all base views...
        //GuiTest.statelist.Add (newState);
        return newState;


    }
    //handler for dragging node event//
    public virtual GuiState MyOnMouseDrag(GuiState current_state)
    {
        GuiState newState = current_state;
        Debug.Log("drag event handler");
        
        if (current_state.Selection.Contains(this.gameObject))
        {				// If doing a mouse drag with this component selected...
            // since we are in 3d space now, we need to conver this to a vector3...
            // for now just use the z coordinate of the first object

            // get the hit world coord
            var pos = HitPosition(this.gameObject);

            // project from camera through mouse currently and use same distance
            Vector3 to_point = ProjectCurrentDrag(dist_to_camera);

            // move object to new coordinate
            this.gameObject.transform.position = to_point;
            newState = new GuiState(false, true, Input.mousePosition, current_state.Selection, false);
            GuiTest.statelist.Add(newState);
            Event.current.Use();

        }
        return newState;


    }
    //handler for clicks
    public virtual GuiState MyOnMouseDown(GuiState current_state)
    {
        Debug.Log("mouse down event handler called");
        // check if this node was actually clicked on
        if (HitTest(this.gameObject, current_state))
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

    public virtual void onGuiRepaint()
    {	// if we need to repaint the UI then update the property location based on the gameobjects underly transform
        //todo not sure if this should go here, it seems location should actually be bound to the BaseModel and this logic flows into the view...
        
    }


}