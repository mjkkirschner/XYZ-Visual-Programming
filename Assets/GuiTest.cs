//attempting to remove guitest controller and implement all ui events using unity's eventsystem

/*using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine.EventSystems;

public class GuiTest : MonoBehaviour
{
    public NodeManager NManager;
    public static List<GuiState> statelist = new List<GuiState>();
    public EventSystem UnityEventSystem;
    // GuiController generates new ui states based on previous state
    // and current event

    //define delegates
    // will need to revist these.
    public delegate GuiState Mouse_Down(GuiState currentstate);
    //maybe on click
    public delegate GuiState Mouse_Up(GuiState currentstate);

    public delegate GuiState Mouse_Drag(GuiState currentstate);

    public delegate GuiState Canvas_Double_Click(GuiState currentstate);

    public delegate GuiState MouseMoved(GuiState currentstate);

    public delegate void GuiRepaint();

    //define our events

    public event Mouse_Down onMouseDown;
    public event Mouse_Up onMouseUp;
    public event Mouse_Drag onMouseDrag;
    public event Canvas_Double_Click onCanvasDoubleClick;
    public event GuiRepaint onGuiRepaint;
    public event MouseMoved onMouseMove;

    
    /// <summary>
    /// returns the most recent state's connecting bool
    /// </summary>
    public bool connecting()
    {
        
        return statelist.Last().Connecting;
    }

    /// <summary>
    /// Returns the most recent states's selection list 
    /// </summary>
    /// <returns>The selection.</returns>
    public List<GameObject> CurrentSelection()
    {
        if (statelist.Count > 0)
        {
            return statelist.Last().Selection;
        }
        else
        {

            return new List<GameObject>();
        }
    }
    /// <summary>
    /// initialization of nodes, node manager, and adds nodes as listeners to events.   
    /// </summary>
    void Start()
    {
        // collect all nodes to 
        NManager = GameObject.FindObjectOfType<NodeManager>();
        UnityEventSystem = GameObject.FindObjectOfType<EventSystem>();
        this.onCanvasDoubleClick += new Canvas_Double_Click(NManager.onCanvasDoubleClick);
        this.onGuiRepaint += new GuiRepaint(NManager.onGuiRepaint);
    }


    void Update()
    {


    }


    private GameObject HoveringOnGameObject()
    {
        // raycast from the camera through the mouse and check if we hit this current
        // node, if we do return true

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();


        if (Physics.Raycast(ray, out hit))
        {

            Debug.DrawRay(ray.origin, ray.direction * 1000000, Color.green);
            return hit.collider.gameObject;

        }
        return null;
    }


    /// <summary>
    /// Raises the GUI events and creates states that reflect the state of the UI.
    /// </summary>
    public void OnGUI()
    {  
    

        if (GuiState.selection_changed(statelist))
        {
            Debug.Log("selection has changed");
          
        }
       
        
        switch (Event.current.type)
        {
            case EventType.mouseDown:

                // generate a new state with the current mouse position 

                var currentSel = new List<GameObject>();
                var state = new GuiState(false, false, Event.current.mousePosition, currentSel, false);
                statelist.Add(state);


                if (onMouseDown != null)
                {
                    Debug.Log("raising event onmousedown");
                    onMouseDown(state);
                }

                if (Event.current.clickCount == 2)
                {// If we double-clicked it, send a double click event... some nodes might respond to this

                    // current selection should be the node we single clicked on previously
                    currentSel = CurrentSelection();
                    state = new GuiState(false, false, Event.current.mousePosition, currentSel, true);
                    statelist.Add(state);


                    if (onMouseDown != null)
                    {
                        var results = new List<GuiState>();
                        // this code gets all the results from all the onMouseDownFunctions,
                        // if they all return null, instead of dragstates then we know we clicked nothing... 
                        // this is bad. :(
                        Debug.Log("calling doubleclick function");
                        foreach (Mouse_Down d in onMouseDown.GetInvocationList())
                        {
                            results.Add(d(state));

                        }
                        if (results.All(element => element == null))
                        {
                            Debug.Log("just double clicked to the canvas, all nodes returned null");
                            Debug.Log("should create new node");
                            // Send a different event
                            // One that the nodemanager subscribes to
                            if (onCanvasDoubleClick != null)
                            {
                                onCanvasDoubleClick(state);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("calling doubleclick function");
                        //there were no nodes, so onMouseDown was null, but we still want to create a node
                        if (onCanvasDoubleClick != null)
                        {
                            onCanvasDoubleClick(state);
                        }
                    }
                }

                Event.current.Use();

                break;
            case EventType.mouseUp:
                // If we released the mouse button...
                // ... with no active selection, ignore the event
                // all these current selection checks might as well go into 
                // the last dragstate and check the current selection there
                // instead of even keeping any properties on the node manager.



                if (CurrentSelection().Count == 0)
                {
                    Debug.Log("ignoring event");
                    state = new GuiState(false, false, Event.current.mousePosition, new List<GameObject>(), false);
                    statelist.Add(state);
                    Debug.Log("calling mouseup function, this was an ignored event, there was a single click on nothing");
                    if (onMouseUp != null)
                    {
                       
                        onMouseUp(state);

                    }


                    // ... while some node was active selection
                    // then we need to clear the selection
                }
                else if (CurrentSelection().Count > 0)
                {


                    if (!connecting())
                    {	// ... and we were not in connect mode, clear the selection

                        Debug.Log("not connecting mouse up");
                        // clear the selection, and create a new state
                        state = new GuiState(false, false, Event.current.mousePosition, new List<GameObject>(), false);
                        statelist.Add(state);
                        Event.current.Use();
                        if (onMouseUp != null)
                        {
                            onMouseUp(state);
                        }
                        break;
                        // send the mouseup event


                    }
                    else
                    {
                        // we are connecting
                        // send a similar state but with connecting true

                        Event.current.Use();
                        if (onMouseUp != null)
                        {
                            onMouseUp(new GuiState(statelist.Last().Connecting,
                             statelist.Last().Dragging,
                             Event.current.mousePosition,
                             statelist.Last().Selection,
                             statelist.Last().DoubleClicked));
                        }
                        break;
                    }

                }

                break;
                
            case EventType.mouseDrag:
                var laststate = statelist.Last();
                state = new GuiState(laststate.Connecting, true, Event.current.mousePosition, CurrentSelection(), true);
                statelist.Add(state);

                //need to rate limit this event being sent, 
                //since we send to all nodes during the 
                //course of drag many times per frame...
                // may need to replace with unity send message
                // to particular node
                var distance = Vector2.Distance(statelist.Last().MousePos, Input.mousePosition);
                if (distance > .01)
                {

                    if (onMouseDrag != null)
                    {
                        onMouseDrag(state);
                    }
                }
                Event.current.Use();
                break;

            case EventType.repaint:
                if (onGuiRepaint != null)
                {
                    onGuiRepaint();
                   
                    }
                
                break;



        }

    }

}*/