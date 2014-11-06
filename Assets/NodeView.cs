using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System.Collections;
using UnityEngine.UI;
using Nodeplay.UI.Utils;
using UnityEngine.EventSystems;

/// <summary>
/// class that is responsible for defining how a node looks
/// this class instantiates gameobjects, geometry, and UI elements into the scene
/// it's possible that an entire node will be serialzed as a prefab and will not require
/// running the nodeview to be instantiated
/// </summary>
public class NodeView : BaseView<NodeModel>{


   
    //variable to help situate projection of mousecoords into worldspace
    
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

    public override void OnPointerUp(PointerEventData pointerdata)
    {
        // if we're connecting to this node, then add this node
        // to the target list of each of the nodes in the selection.

        Debug.Log("Mouse up event handler called");
       


    }
    //handler for dragging node event//
    //TODO, this will not work  - this object will never get this event...
    public override void OnDrag(PointerEventData pointerdata)
    {
            // get the hit world coord
            var pos = HitPosition(this.gameObject);

            // project from camera through mouse currently and use same distance
            Vector3 to_point = ProjectCurrentDrag(dist_to_camera);

            // move object to new coordinate
            //TODO might want to do this by applying a force
            // or starting a coroutine that moves the node smoothly
            this.transform.position = to_point;
     


    }
    //handler for clicks
    public override void OnPointerClick(PointerEventData pointerdata)
    {
      
            Debug.Log("I" + this.name + " was just clicked");
            dist_to_camera = Vector3.Distance(this.transform.position, Camera.main.transform.position);

            // check the dragstate from the GUI, either this is a double click
            // or a selection click
            // or possibly a click on nothing
            if (pointerdata.clickCount != 2)
            {

                
            }
            else
            {
                //a double click occured on a node
                Debug.Log("I" + this.name + " was just DOUBLE clicked");

            }
        
    }


}
