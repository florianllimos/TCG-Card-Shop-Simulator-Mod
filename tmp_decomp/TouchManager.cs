using UnityEngine;

public class TouchManager : CSingleton<TouchManager>
{
	public static TouchManager m_Instance;

	public float m_ScrollSpeedMultiplier = 1f;

	public bool m_IsEnabled = true;

	public bool m_IsPressed;

	public bool m_IsDragging;

	public bool m_IsDraggingHorizontally;

	public bool m_IsQuickSwipeX;

	public bool m_IsQuickSwipeY;

	public bool m_IsQuickSwipeXn;

	public bool m_IsQuickSwipeYn;

	public bool m_IsSnapping;

	public bool m_IsSnappingHorizontally;

	private float m_StartPosX;

	private float m_LastPosX;

	private float m_StartPosY;

	private float m_LastPosY;

	public float m_DragRatioX;

	public float m_DragRatioY;

	public float m_FreeDragRatioX;

	public float m_FreeDragRatioY;

	private float m_FreeLastPosX;

	private float m_FreeLastPosY;

	private float m_ScreenWidth;

	private float m_ScreenHeight;

	public Vector2 m_ReleasePos;

	private float m_QuickSwipeTimer;

	public float m_LerpSpeed = 5f;

	public float m_TargetLerpSpeed = 5f;

	public float m_DefaultLerpSpeed = 7f;

	public bool m_AllowButtonPress = true;

	private float m_AllowButtonPressTimer;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		if (!m_IsEnabled)
		{
			return;
		}
		if (Input.touchSupported && Input.touchCount > 1)
		{
			ResetSwipe();
		}
		else
		{
			if (CSingleton<InputManager>.Instance.m_IsControllerActive)
			{
				return;
			}
			if (!m_IsPressed)
			{
				m_DragRatioX = Mathf.Lerp(m_DragRatioX, 0f, Time.deltaTime * 6f);
				m_DragRatioY = Mathf.Lerp(m_DragRatioY, 0f, Time.deltaTime * 6f);
				m_FreeDragRatioX = Mathf.Lerp(m_FreeDragRatioX, 0f, Time.deltaTime * 6f);
				m_FreeDragRatioY = Mathf.Lerp(m_FreeDragRatioY, 0f, Time.deltaTime * 6f);
				if (InputManager.GetKeyDownAction(EGameAction.InteractLeft))
				{
					m_AllowButtonPressTimer = 0f;
					m_AllowButtonPress = true;
					m_IsQuickSwipeX = false;
					m_IsQuickSwipeY = false;
					m_IsQuickSwipeXn = false;
					m_IsQuickSwipeYn = false;
					m_ScreenWidth = Screen.width;
					m_ScreenHeight = Screen.height;
					m_IsPressed = true;
					m_IsDragging = false;
					m_IsDraggingHorizontally = false;
					m_LastPosX = Input.mousePosition.x;
					m_FreeLastPosX = Input.mousePosition.x;
					m_StartPosX = m_LastPosX;
					m_LastPosY = Input.mousePosition.y;
					m_FreeLastPosY = Input.mousePosition.y;
					m_StartPosY = m_LastPosY;
					m_DragRatioX = 0f;
					m_DragRatioY = 0f;
					m_FreeDragRatioX = 0f;
					m_FreeDragRatioY = 0f;
					m_IsSnapping = true;
					m_IsSnappingHorizontally = true;
				}
			}
			else
			{
				m_QuickSwipeTimer += Time.deltaTime;
				if (!m_IsDraggingHorizontally && !m_IsDragging && Mathf.Abs(m_LastPosY - m_StartPosY) > 20f)
				{
					m_IsDragging = true;
					RaycasterManager.SetUIRaycastEnabled(isEnabled: false);
				}
				else if (!m_IsDraggingHorizontally && !m_IsDragging && Mathf.Abs(m_LastPosX - m_StartPosX) > 20f)
				{
					m_IsDraggingHorizontally = true;
					RaycasterManager.SetUIRaycastEnabled(isEnabled: false);
				}
				if ((m_IsDragging || m_IsDraggingHorizontally) && m_QuickSwipeTimer > 0.02f && m_TargetLerpSpeed != 100f)
				{
					m_DragRatioX = 0f;
					m_DragRatioY = 0f;
					m_TargetLerpSpeed = 100f;
					m_LerpSpeed = 100f;
					if (m_IsDragging)
					{
						m_IsSnapping = true;
					}
					else if (m_IsDraggingHorizontally)
					{
						m_IsSnappingHorizontally = true;
					}
				}
				if (m_IsDraggingHorizontally)
				{
					m_DragRatioX = (Input.mousePosition.x - m_LastPosX) / m_ScreenWidth;
				}
				else if (m_IsDragging)
				{
					m_DragRatioY = (Input.mousePosition.y - m_LastPosY) / m_ScreenHeight;
				}
				m_FreeDragRatioX = (Input.mousePosition.x - m_FreeLastPosX) / m_ScreenWidth;
				m_FreeDragRatioY = (Input.mousePosition.y - m_FreeLastPosY) / m_ScreenHeight;
				if (!m_IsDragging)
				{
					m_LastPosX = Input.mousePosition.x;
				}
				if (!m_IsDraggingHorizontally)
				{
					m_LastPosY = Input.mousePosition.y;
				}
				m_FreeLastPosX = Input.mousePosition.x;
				m_FreeLastPosY = Input.mousePosition.y;
				if (Input.GetMouseButtonUp(0))
				{
					if (m_QuickSwipeTimer < 0.4f)
					{
						if (m_LastPosX - m_StartPosX > 150f)
						{
							m_IsQuickSwipeX = true;
						}
						if (m_LastPosY - m_StartPosY > 150f)
						{
							m_IsQuickSwipeY = true;
						}
						if (m_LastPosX - m_StartPosX < -150f)
						{
							m_IsQuickSwipeXn = true;
						}
						if (m_LastPosY - m_StartPosY < -150f)
						{
							m_IsQuickSwipeYn = true;
						}
					}
					float num = Mathf.Clamp((0.5f - m_QuickSwipeTimer) * 0.15f, 0f, 0.1f);
					if (m_IsQuickSwipeX)
					{
						m_DragRatioX += num;
					}
					if (m_IsQuickSwipeXn)
					{
						m_DragRatioX -= num;
					}
					if (m_IsQuickSwipeY)
					{
						m_DragRatioY += num;
					}
					if (m_IsQuickSwipeYn)
					{
						m_DragRatioY -= num;
					}
					m_QuickSwipeTimer = 0f;
					m_IsDragging = false;
					m_IsDraggingHorizontally = false;
					m_IsPressed = false;
					m_ReleasePos = Input.mousePosition;
					m_TargetLerpSpeed = m_DefaultLerpSpeed;
					m_LerpSpeed = m_DefaultLerpSpeed;
					m_AllowButtonPress = true;
					RaycasterManager.SetUIRaycastEnabled(isEnabled: true);
				}
			}
			if (m_AllowButtonPress && (IsDragging() || IsDraggingHorizontally()))
			{
				m_AllowButtonPress = false;
			}
			if (!m_IsPressed && !m_AllowButtonPress)
			{
				m_AllowButtonPressTimer += Time.deltaTime;
				_ = m_AllowButtonPressTimer;
				_ = 0.2f;
			}
		}
	}

	public void ResetSwipe()
	{
		m_AllowButtonPress = false;
		m_QuickSwipeTimer = 0f;
		m_IsDragging = false;
		m_IsDraggingHorizontally = false;
		m_IsSnapping = false;
		m_IsSnappingHorizontally = false;
		m_DragRatioX = 0f;
		m_DragRatioY = 0f;
		m_FreeDragRatioX = 0f;
		m_FreeDragRatioY = 0f;
		m_LastPosX = Input.mousePosition.x;
		m_LastPosY = Input.mousePosition.y;
		m_FreeLastPosX = Input.mousePosition.x;
		m_FreeLastPosY = Input.mousePosition.y;
		m_IsQuickSwipeX = false;
		m_IsQuickSwipeY = false;
		m_IsQuickSwipeXn = false;
		m_IsQuickSwipeYn = false;
		m_IsPressed = false;
		m_ReleasePos = Input.mousePosition;
		m_TargetLerpSpeed = m_DefaultLerpSpeed;
		m_LerpSpeed = m_DefaultLerpSpeed;
		RaycasterManager.SetUIRaycastEnabled(isEnabled: true);
	}

	public static bool IsDragging()
	{
		return CSingleton<TouchManager>.Instance.m_IsDragging;
	}

	public static bool IsDraggingHorizontally()
	{
		return CSingleton<TouchManager>.Instance.m_IsDraggingHorizontally;
	}

	public static int GetTouchFingerID()
	{
		if (Input.touchCount > 0)
		{
			return Input.GetTouch(Input.touchCount - 1).fingerId;
		}
		return -1;
	}

	public static bool IsSnappingToFinger()
	{
		if (CSingleton<TouchManager>.Instance.m_IsSnapping)
		{
			CSingleton<TouchManager>.Instance.m_IsSnapping = false;
			return true;
		}
		return false;
	}

	public static bool IsSnappingToFingerHorizontally()
	{
		if (CSingleton<TouchManager>.Instance.m_IsSnappingHorizontally)
		{
			CSingleton<TouchManager>.Instance.m_IsSnappingHorizontally = false;
			return true;
		}
		return false;
	}

	public static void ResetFingerSnapping()
	{
		CSingleton<TouchManager>.Instance.m_IsSnapping = false;
		CSingleton<TouchManager>.Instance.m_IsSnappingHorizontally = false;
	}

	public static Vector3 GetTouchPositionUsingID(int fingerID)
	{
		if ((!Input.touchSupported && !Input.multiTouchEnabled) || fingerID == -1)
		{
			return Input.mousePosition;
		}
		for (int i = 0; i < Input.touchCount; i++)
		{
			if (Input.GetTouch(i).fingerId == fingerID)
			{
				return Input.GetTouch(i).position;
			}
		}
		return Vector3.zero;
	}
}
