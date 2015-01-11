using UnityEngine;
using System.Collections;
using System.Linq;

namespace Nodeplay.UI
{
	public class TogglePanelButton : MonoBehaviour
	{
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
			panel.GetComponentsInChildren<Canvas>().Select(x=>x.enabled = !state).ToList();
		}
	
	}

}