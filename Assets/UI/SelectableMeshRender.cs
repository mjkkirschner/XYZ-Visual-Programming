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

    class SelectableMeshRender : Selectable
    {

        public List<Renderer> obUI;
		public Vector3 NormalScale { get; set; }
		public Vector3 HoverScale { get; set; }

		

        protected override void OnEnable()
        {
            obUI = this.GetComponentsInChildren<Renderer>().ToList();
            base.OnEnable();


        }

        protected virtual IEnumerator TweenScaleFromCurrent(Vector3 ToScale, float duration)
        {
            for (float f = 0; f <= duration; f = f + Time.deltaTime)
            {

                obUI.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.transform.localScale = Vector3.Lerp(x.transform.localScale, ToScale, f);
                    }
                });
                yield return null;

            }
            obUI.ForEach(x => x.transform.localScale = ToScale);
        }


        protected virtual IEnumerator TweenColorFromCurrent(Color ToColor, float duration)
        {
            for (float f = 0; f <= duration; f = f + Time.deltaTime)
            {

                obUI.ForEach(x =>
                {
                    if (x != null)
                    {
                        x.material.color = Color.Lerp(x.material.color, ToColor, f);
                    }
                });
                yield return null;

            }
            obUI.ForEach(x => x.material.color = ToColor);
        }

		public override void OnPointerEnter (PointerEventData eventData)
		{	if (!eventData.used){
			base.OnPointerEnter(eventData);
			eventData.Use();
				
			}
		}



        protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
        {
			
            obUI = this.GetComponentsInChildren<Renderer>().ToList();
            Debug.Log("inside state transition");

            if (state == SelectionState.Pressed)
            {
                StopAllCoroutines();
                StartCoroutine(TweenColorFromCurrent(this.colors.pressedColor, this.colors.fadeDuration));

            }

            if (state == Selectable.SelectionState.Highlighted)
            {
                Debug.Log("state was highlight");
                
                StartCoroutine(TweenColorFromCurrent(this.colors.highlightedColor, this.colors.fadeDuration));
                StartCoroutine(TweenScaleFromCurrent(HoverScale, this.colors.fadeDuration));
            }
            if (state == Selectable.SelectionState.Normal)
            {
                Debug.Log("state was normal");
                StopAllCoroutines();
                StartCoroutine(TweenColorFromCurrent(this.colors.normalColor, this.colors.fadeDuration));
                StartCoroutine(TweenScaleFromCurrent(NormalScale, this.colors.fadeDuration));
            }

        }
	}
}
