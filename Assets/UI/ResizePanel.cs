using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler {
	
	public Vector2 minSize;
	public Vector2 maxSize;
	
	private RectTransform rectTransform;
	private LayoutElement layoutControls;
	private Vector2 currentPointerPosition;
	private Vector2 previousPointerPosition;
	
	void Awake () {
		rectTransform = transform.parent.GetComponent<RectTransform>();
		layoutControls = transform.parent.Find("InputPanel").GetComponent<LayoutElement>();
	}
	
	public void OnPointerDown (PointerEventData data) {
		rectTransform.SetAsLastSibling();
		RectTransformUtility.ScreenPointToLocalPointInRectangle (rectTransform, data.position, data.pressEventCamera, out previousPointerPosition);
	}
	
	public void OnDrag (PointerEventData data) {
		if (rectTransform == null)
			return;
		
		Vector2 sizeDelta = new Vector2(layoutControls.preferredWidth,layoutControls.preferredHeight);

		RectTransformUtility.ScreenPointToLocalPointInRectangle (rectTransform, data.position, data.pressEventCamera, out currentPointerPosition);
		Vector2 resizeValue = currentPointerPosition - previousPointerPosition;

		sizeDelta += new Vector2 (resizeValue.x*2, -resizeValue.y);
		sizeDelta = new Vector2 (
			Mathf.Clamp (sizeDelta.x, minSize.x, maxSize.x),
			Mathf.Clamp (sizeDelta.y, minSize.y, maxSize.y)
			);

		layoutControls.preferredWidth = sizeDelta.x;
		layoutControls.preferredHeight = sizeDelta.y;
		
		previousPointerPosition = currentPointerPosition;
	}
}