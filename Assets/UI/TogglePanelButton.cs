using UnityEngine;
using System.Collections;


namespace Nodeplay.UI
{
	public class TogglePanelButton : MonoBehaviour
	{
		public void TogglePanel(GameObject panel)
		{
			panel.SetActive(!panel.activeSelf);
		}
	}
}