using UnityEngine;
using UnityEngine.EventSystems;

namespace CC;

public class CameraController : MonoBehaviour
{
	public static CameraController instance;

	public float ZoomMin = -0.6f;

	public float ZoomMax = -3.1f;

	public float ZoomPanScale = 0.5f;

	private Camera _camera;

	private Transform cameraRoot;

	private float zoomTarget = 1f;

	private Vector3 mouseOldPos;

	private Vector3 mouseDelta;

	private Vector3 cameraRotationTarget = new Vector3(10f, -5f, 0f);

	private Vector3 cameraRotationDefault;

	private float rotateSpeed = 5f;

	private bool dragging;

	private Vector3 cameraOffset;

	private Vector3 cameraOffsetDefault;

	private float panSpeed = 3f;

	private bool panning;

	public Texture2D cursorTexture;

	private Vector2 hotSpot;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		_camera = Camera.main;
		cameraRoot = base.gameObject.transform;
		hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
		setDefaultCursor();
		cameraOffsetDefault = _camera.transform.localPosition;
		cameraOffset = cameraOffsetDefault;
		cameraRotationDefault = cameraRoot.localRotation.eulerAngles;
		cameraRotationTarget = cameraRotationDefault;
	}

	public void setCursor(Texture2D texture)
	{
		Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);
	}

	public void setDefaultCursor()
	{
		Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
	}

	private void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			Vector3 vector = Camera.main.ScreenToViewportPoint(Input.mousePosition) - new Vector3(0.5f, 0.5f, 0f);
			cameraOffset += vector * Mathf.Abs(Input.mouseScrollDelta.y) * ZoomPanScale * Mathf.Lerp(0.2f, 1f, zoomTarget);
			zoomTarget = Mathf.Clamp(zoomTarget * (1f - Input.mouseScrollDelta.y / 10f), 0f, 1f);
			if (Input.GetMouseButtonDown(1))
			{
				mouseOldPos = Input.mousePosition;
				dragging = true;
			}
			if (Input.GetMouseButtonDown(2))
			{
				mouseOldPos = Input.mousePosition;
				panning = true;
			}
		}
		if (Input.GetMouseButton(1) && dragging)
		{
			mouseDelta = mouseOldPos - Input.mousePosition;
			cameraRotationTarget.x += mouseDelta.y / 5f;
			cameraRotationTarget.y -= mouseDelta.x / 5f;
			mouseOldPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(1))
		{
			dragging = false;
		}
		if (Input.GetMouseButton(2) && panning)
		{
			mouseDelta = mouseOldPos - Input.mousePosition;
			cameraOffset -= mouseDelta / 500f;
			mouseOldPos = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp(2))
		{
			panning = false;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			cameraRotationTarget = cameraRotationDefault;
			cameraOffset = cameraOffsetDefault;
			zoomTarget = 1f;
		}
		cameraOffset.z = Mathf.Lerp(ZoomMin, ZoomMax, zoomTarget);
		_camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, cameraOffset, Time.deltaTime * panSpeed);
		cameraRoot.transform.localRotation = Quaternion.Slerp(cameraRoot.transform.localRotation, Quaternion.Euler(cameraRotationTarget), Time.deltaTime * rotateSpeed);
	}
}
