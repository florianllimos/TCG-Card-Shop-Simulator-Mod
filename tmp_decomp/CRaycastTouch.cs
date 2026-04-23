using UnityEngine;
using UnityEngine.UI;

public class CRaycastTouch : MonoBehaviour
{
	private RaycastHit hit;

	private Ray ray;

	private bool m_MouseDown;

	private bool m_FirstRayCast;

	private Vector3 m_MouseStartPos;

	private Vector3 m_MouseCurrentPos;

	private Vector3 m_LastMouseCoordinate = Vector3.zero;

	private Transform m_ObjectRootHit;

	private float m_MouseDownTime;

	public Text m_Text;

	private void Update()
	{
		if (m_MouseDown && Input.GetMouseButtonUp(0))
		{
			m_MouseDownTime = 0f;
			m_MouseDown = false;
			m_FirstRayCast = false;
			Vector3 vector = Input.mousePosition - m_MouseStartPos;
			CEventManager.QueueEvent(new CEventPlayer_TouchReleased(new Vector3(vector.x / (float)Screen.width, vector.y / (float)Screen.height, 0f)));
		}
		if (!m_MouseDown)
		{
			return;
		}
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.fingerId == 0 && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began))
				{
					EvaluateRaycast(touch.position);
					m_MouseCurrentPos = touch.position;
				}
			}
		}
		else
		{
			EvaluateRaycast(Input.mousePosition);
			m_MouseCurrentPos = Input.mousePosition;
		}
	}

	private Vector3 CalculateMouseSpeed()
	{
		Vector3 result = Vector3.zero;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.fingerId == 0)
				{
					Vector3 vector = new Vector3(touch.position.x, touch.position.y, 0f);
					result = m_MouseCurrentPos - vector;
				}
			}
		}
		else
		{
			result = m_MouseCurrentPos - Input.mousePosition;
		}
		return result;
	}

	private void EvaluateRaycast(Vector2 InputPosition)
	{
		if (!m_FirstRayCast)
		{
			m_FirstRayCast = true;
			ray = Camera.main.ScreenPointToRay(InputPosition);
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.root.tag == "Prop")
				{
					m_ObjectRootHit = hit.transform.root;
					TouchedProp(m_ObjectRootHit);
				}
				else if (hit.transform.tag == "CatFollow")
				{
					m_ObjectRootHit = hit.transform;
					TouchedProp(m_ObjectRootHit);
				}
				else if (hit.transform.tag == "Coin")
				{
					TouchedProp(hit.transform);
				}
			}
		}
		if (Physics.Raycast(Camera.main.ScreenPointToRay(InputPosition), out var hitInfo) && hitInfo.transform.tag == "Coin")
		{
			TouchedProp(hitInfo.transform);
		}
	}

	private void TouchedProp(Transform objectHitRoot)
	{
		objectHitRoot.SendMessage("RaycastHit");
	}

	private bool IsMouseMove()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Vector3 vector = default(Vector3);
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.fingerId == 0 && touch.phase == TouchPhase.Moved)
				{
					vector = (Vector3)touch.position - m_LastMouseCoordinate;
					m_LastMouseCoordinate = touch.position;
				}
				else if (touch.fingerId == 1 && touch.phase == TouchPhase.Moved)
				{
					vector = (Vector3)touch.position - m_LastMouseCoordinate;
					m_LastMouseCoordinate = touch.position;
				}
			}
			if (vector.x < 0f || vector.x > 0f)
			{
				return true;
			}
		}
		else
		{
			Vector3 vector2 = Input.mousePosition - m_LastMouseCoordinate;
			m_LastMouseCoordinate = Input.mousePosition;
			if (vector2.x < 0f || vector2.x > 0f)
			{
				return true;
			}
		}
		return false;
	}

	public void TouchScreenButton()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.touchCount == 1)
			{
				m_MouseDown = true;
				m_ObjectRootHit = null;
				CEventManager.QueueEvent(new CEventPlayer_TouchScreen());
				m_MouseStartPos = Input.mousePosition;
			}
		}
		else
		{
			m_MouseDown = true;
			m_ObjectRootHit = null;
			CEventManager.QueueEvent(new CEventPlayer_TouchScreen());
			m_MouseStartPos = Input.mousePosition;
		}
	}
}
