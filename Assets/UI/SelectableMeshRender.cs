using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Collections;

namespace Nodeplay.UI
{   //TODO may push this into BaseView, 
    
		class SelectableMeshRender:Selectable
		{

				public Renderer obUI;
        
       
				protected override void OnEnable ()
				{
						obUI = this.GetComponentInChildren<Renderer> ();
						base.OnEnable ();
        
        
				}


				protected virtual IEnumerator TweenColorFromCurrent (Color ToColor, float duration)
				{
						for (float f = 0; f <= duration; f = f + Time.deltaTime) {

								obUI.material.color = Color.Lerp (renderer.material.color, ToColor, f);
								yield return null;

						}
						obUI.material.color = ToColor;
				}


				protected override void DoStateTransition (Selectable.SelectionState state, bool instant)
				{
           
						Debug.Log ("inside state transition");

						if (state == SelectionState.Pressed) {
								StopAllCoroutines ();
								StartCoroutine (TweenColorFromCurrent (this.colors.pressedColor, this.colors.fadeDuration));
						}
           
						if (state == Selectable.SelectionState.Highlighted) {
								Debug.Log ("state was highlight");
								//StopAllCoroutines();
								StartCoroutine (TweenColorFromCurrent (this.colors.highlightedColor, this.colors.fadeDuration));
						}
						if (state == Selectable.SelectionState.Normal) {
								Debug.Log ("state was normal");
								StopAllCoroutines ();
								StartCoroutine (TweenColorFromCurrent (this.colors.normalColor, this.colors.fadeDuration));
						}
            
				}
		}
}
