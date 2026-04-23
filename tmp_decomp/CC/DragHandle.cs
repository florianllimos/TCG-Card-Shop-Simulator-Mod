using UnityEngine;
using UnityEngine.EventSystems;

namespace CC;

public class DragHandle : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	public GameObject WindowToDrag;

	private RectTransform rectTransform;

	private Canvas canvas;

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		rectTransform.SetAsLastSibling();
	}

	private void Start()
	{
		rectTransform = WindowToDrag.GetComponent<RectTransform>();
		canvas = GetComponentsInParent<Canvas>()[^1];
	}
}
