using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Nodeplay.UI;

//TODO probably should repleace this with a nongeneric abstract base class
public class BaseView<M> : EventTrigger, Iinteractable, INotifyPropertyChanged where M : BaseModel
{
    //because views maybe created or destroyed multiple times per frame,
    //they may be destroyed before start finishes running
    protected Boolean started = false;
    

    protected Color originalcolor;
	protected Color highlightcolor = Color.green;
	protected Vector3 NormalScale = new Vector3(1,1,1);
	protected Vector3 HoverScale = new Vector3(1.2f,1.2f,1.2f);

    public event PropertyChangedEventHandler PropertyChanged;
    protected float dist_to_camera;
    public M Model;
    //some gameobject root that represents the geometry this view represents/controls
    public GameObject UI;
    public Selectable selectable;

    protected virtual void NotifyPropertyChanged(String info)
    {
        Debug.Log("sending some property change notification");
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
    //overide this method to setup the transition colors for this view
    protected ColorBlock setupColorBlock(Color ogc, Color hlc)
    {
        var output = new ColorBlock();
        output.colorMultiplier = 1;
        output.disabledColor = Color.grey;
		output.normalColor = ogc;
		output.highlightedColor = hlc;
		output.pressedColor = new Color(hlc.r, hlc.g - .2f, hlc.b + .2f);
        output.fadeDuration = .1f;
        return output;
    }
	protected ColorBlock setupColorBlock()
	{
		return setupColorBlock(originalcolor, highlightcolor);
	}

    protected virtual void Start()
    {   //TODO contract for hierarchy
        // we always search the root gameobject of this view for the model,
        // need to enforce this contract somehow, I think can use requires component.
        Model = this.gameObject.GetComponent<M>();

        dist_to_camera = Vector3.Distance(this.gameObject.transform.position, Camera.main.transform.position);
        // nodemanager manages nodes - like a workspacemodel

        UI = Model.BuildSceneElements();
        var firstRenderer = UI.GetComponentInChildren<Renderer>();

        originalcolor = firstRenderer.material.color;

        //add our selectable mesh render component here to nodes since these are selectable and 3d objects
        this.gameObject.AddComponent<SelectableMeshRender>();
        var sel = this.GetComponent<SelectableMeshRender>();
		this.SetupSelectable(sel);
		

        Debug.Log("just started BaseView");
        started = true;
        
    }

	private void SetupSelectable(SelectableMeshRender selectable)
	{
		selectable.enabled = false;
		//setup the selectable'scolors
		selectable.colors = setupColorBlock();
		//setup the values for hover scaling
		selectable.NormalScale = NormalScale;
		selectable.HoverScale = HoverScale;
		selectable.enabled = true;
	}

    protected virtual void OnDestroy()
    {
        if (!started)
        {
            Start();
        }

        Debug.Log("just turned off BaseView");

    }

    public static Vector3 ProjectCurrentDrag(float distance)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var output = ray.GetPoint(distance);
        return output;
    }

    public static Vector3 HitPosition(GameObject go_to_test)
    {

        // return the coordinate in world space where hit occured

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        //Debug.DrawRay(ray.origin,ray.direction*dist_to_camera);
        if (Physics.Raycast(ray, out hit))
        {

            return hit.point;

        }
        return go_to_test.transform.position;
    }

    /// <summary>
    /// check if we hit the current node to test, use the state to extract mouseposition
    /// </summary>
    /// <returns><c>true</c>, if test was hit, <c>false</c> otherwise.</returns>
    /// <param name="go_to_test">Node_to_test.</param>
    /// <param name="state">State.</param>
    public static bool HitTest(GameObject go_to_test, PointerEventData pointerdata)
    {
        // raycast from the camera through the mouse and check if we hit this current
        // node, if we do return true

        Ray ray = Camera.main.ScreenPointToRay(pointerdata.position);
        var hit = new RaycastHit();


        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("a ray just hit  " + hit.collider.gameObject);
            //Debug.DrawRay(ray.origin, ray.direction * 1000000);
            if (hit.collider.gameObject == go_to_test.gameObject)
            {
                return true;
            }

        }
        return false;
    }

    public override void OnPointerUp(PointerEventData pointerdata)
    {
        Debug.Log("Mouse up event handler called");

    }
    //handler for dragging node event//
    public override void OnDrag(PointerEventData pointerdata)
    {
        Debug.Log("drag called");

		if (pointerdata.rawPointerPress == this.UI && pointerdata.button == PointerEventData.InputButton.Left ||
			this.GetComponentsInChildren<CanvasRenderer>().Select(x=>x.gameObject).ToList().Contains(pointerdata.rawPointerPress)  && pointerdata.button == PointerEventData.InputButton.Left)
        {
			Ray ray = Camera.main.ScreenPointToRay(pointerdata.position - pointerdata.delta);
			var orginalPoint = ray.GetPoint(dist_to_camera);
			Vector3 threeddelta = BaseView<BaseModel>.ProjectCurrentDrag(dist_to_camera) - orginalPoint;
			this.transform.position += threeddelta;
        }

    }

    public override void OnPointerDown(PointerEventData pointerdata)
    {
        Debug.Log("mouse down called");
        dist_to_camera = Vector3.Distance(this.transform.position, Camera.main.transform.position);
    }

    //handler for clicks
    public override void OnPointerClick(PointerEventData pointerdata)
    {
		if (pointerdata.pointerCurrentRaycast.gameObject == this.UI && pointerdata.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("I" + this.name + " was just clicked");
            dist_to_camera = Vector3.Distance(this.transform.position, Camera.main.transform.position);
			GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(this.gameObject);
			Debug.Log("setting " + this.name + "to selected game object");
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

    protected virtual IEnumerator Blink(Color ToColor, float duration)
    {
        for (float f = 0; f <= duration; f = f + Time.deltaTime)
        {
          
            UI.GetComponent<Renderer>().material.color = Color.Lerp(originalcolor, ToColor, f);
            yield return null;

        }
        UI.GetComponent<Renderer>().material.color = ToColor;
    }

    protected virtual IEnumerator Blunk(Color FromColor, float duration)
    {
        for (float f = 0; f <= duration; f = f + Time.deltaTime)
        {
           
            UI.GetComponent<Renderer>().material.color = Color.Lerp(FromColor, originalcolor, f);
            yield return null;
          
        }
        UI.GetComponent<Renderer>().material.color = originalcolor;
    }

   
}