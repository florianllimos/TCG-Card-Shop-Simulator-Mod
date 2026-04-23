using UnityEngine;
using UnityEngine.EventSystems;

namespace CC;

public class Grid_Picker : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler
{
	private RectTransform rectTransform;

	private RectTransform rectTransformPicker;

	private Vector2 rectSize;

	public Vector2 imageSize;

	public Camera _camera;

	public bool UseCamera;

	public GameObject Picker;

	public GameObject Background;

	public OnPickerDrag m_onPickerDrag = new OnPickerDrag();

	public void Start()
	{
		rectTransform = base.gameObject.GetComponent<RectTransform>();
		rectTransformPicker = Picker.GetComponent<RectTransform>();
		rectSize = rectTransform.sizeDelta;
		if (_camera == null && UseCamera)
		{
			_camera = Camera.main;
		}
	}

	public void UpdatePosition(PointerEventData eventData)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, _camera, out var localPoint);
		localPoint.x = Mathf.Clamp(localPoint.x, 0f, rectSize.x);
		localPoint.y = Mathf.Clamp(localPoint.y, 0f, rectSize.y);
		rectTransformPicker.anchoredPosition = new Vector2(Mathf.Clamp(localPoint.x, 10f, rectSize.x - 10f), Mathf.Clamp(localPoint.y, 10f, rectSize.y - 10f));
		localPoint /= rectSize;
		m_onPickerDrag.Invoke(localPoint);
	}

	public void OnDrag(PointerEventData eventData)
	{
		UpdatePosition(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		rectSize = rectTransform.sizeDelta;
		UpdatePosition(eventData);
	}

	public void randomize()
	{
		float num = Random.Range(0f, 1f);
		float num2 = Random.Range(0f, 1f);
		m_onPickerDrag.Invoke(new Vector2(num, num2));
		rectTransformPicker.anchoredPosition = new Vector2(Mathf.Clamp(num * rectSize.x, 10f, rectSize.x - 10f), Mathf.Clamp(num2 * rectSize.y, 10f, rectSize.y - 10f));
	}
}
