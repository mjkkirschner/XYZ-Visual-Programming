using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;

namespace Nodeplay.UI
{
	public class TogglePanelButton : MonoBehaviour
	{
		List<float> _originalMinSizes = null;
		List<float> _originalPrefSizes = null;
		float _minSize = 0;
		bool _minimized = false;

		public void TogglePanel(GameObject panel)
		{
			panel.SetActive(!panel.activeSelf);
		}
		/// <summary>
		/// Toggles the canvas panel in all children.
		/// </summary>
		/// <param name="panel">Panel.</param>
		public void ToggleCanvasPanel(GameObject panel)
		{
			var state = panel.GetComponentInChildren<Canvas>().enabled;
			panel.GetComponentsInChildren<Canvas>().Select(x => x.enabled = !state).ToList();
		}


		public void ToggleMinSize(GameObject panel)
		{
			

			var elements = panel.GetComponentsInChildren<LayoutElement>().ToList();
			if (_minimized)
			{
				//expand
				foreach (var element in elements)
				{
					//set each element's minheight and prefheight back to what they were when the
					//toggle was clicked TODO, possibly set on start
					
					element.minHeight = LayoutUtility.GetMinHeight(element.GetComponent<RectTransform>());
					element.preferredHeight = LayoutUtility.GetPreferredHeight(element.GetComponent<RectTransform>());
					
				}
			}
			else
			//minimize
			{
				// then record those min heights
				{	//record list of sizes
					_originalMinSizes = panel.GetComponentsInChildren<LayoutElement>().Select(x => x.minHeight).ToList();
					_originalPrefSizes = panel.GetComponentsInChildren<LayoutElement>().Select(x => x.preferredHeight).ToList();
				}

				elements.ForEach(x => x.minHeight = _minSize);
				elements.ForEach(x => x.preferredHeight = _minSize);
			}
			//invert state
			_minimized = !_minimized;

		}

		public void ToggleContentFitMethod(GameObject panel)
		{
			var _minSize = 0;
			var elements = panel.GetComponentsInChildren<LayoutElement>().ToList();
			elements.ForEach(x => x.minHeight = _minSize);
			var contentFitter = panel.GetComponentInParent<ContentSizeFitter>();
			
			if (contentFitter.verticalFit
			   == ContentSizeFitter.FitMode.PreferredSize)
			{
				contentFitter.verticalFit =
					ContentSizeFitter.FitMode.MinSize;
				
			}
			else
			{
				contentFitter.verticalFit =
			   ContentSizeFitter.FitMode.PreferredSize;
			}
		}

	}

}