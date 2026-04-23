using UnityEngine;

public class TouchScrollVertical : MonoBehaviour
{
	public float m_ScrollLimitMinY = 150f;

	public float m_ScrollLimitMaxY;

	public float m_ScrollerLerpSpeed = 5f;

	public float m_ScrollSensitivity = 0.4f;

	public float m_Inverse = -1f;

	public GameObject m_ScrollIndicator;

	public GameObject m_ScrollIndicatorEnd;

	private Vector3 m_ScrollIndicatorStartPos;

	private Vector3 m_ScrollIndicatorEndPos;

	private float m_ScrollIndicatorLerpAlpha;

	private bool m_IsScrollingInverse = true;

	private RectTransform m_RectTransform;

	private Vector3 m_ScrollerLerpPos;

	private bool m_FingerTouched;

	private float m_FingerTouchTime;

	private readonly Vector2 mXAxis = new Vector2(1f, 0f);

	private readonly Vector2 mYAxis = new Vector2(0f, 1f);

	private const float mAngleRange = 44f;

	private const float mMinSwipeDist = 0.1f;

	private const float mMinVelocity = 0.1f;

	private Vector2 mStartPosition;

	private float mSwipeStartTime;

	private void OnEnable()
	{
		ResetTouch();
	}

	private void Start()
	{
		m_RectTransform = GetComponent<RectTransform>();
		if ((bool)m_ScrollIndicator && (bool)m_ScrollIndicatorEnd)
		{
			m_ScrollIndicatorStartPos = m_ScrollIndicator.transform.position;
			m_ScrollIndicatorEndPos = m_ScrollIndicatorEnd.transform.position;
		}
	}

	private void Update()
	{
		SwipeDetection();
		EvaluateScrollIndicator();
	}

	public void SetScrollingNormal()
	{
		m_IsScrollingInverse = false;
	}

	public void SetScrollingInverse()
	{
		m_IsScrollingInverse = true;
	}

	private void EvaluateScrollIndicator()
	{
		if ((bool)m_ScrollIndicator && (bool)m_ScrollIndicatorEnd)
		{
			m_ScrollIndicatorLerpAlpha = m_RectTransform.localPosition.y / m_ScrollLimitMinY;
			m_ScrollIndicator.transform.position = Vector3.Lerp(m_ScrollIndicatorStartPos, m_ScrollIndicatorEndPos, m_ScrollIndicatorLerpAlpha);
		}
	}

	public void IsMouseDown()
	{
	}

	public void IsMouseUp()
	{
	}

	public void SetScrollLerpPos(float posY)
	{
		m_ScrollerLerpPos = new Vector3(0f, posY, 0f);
	}

	private void SwipeDetection()
	{
		m_RectTransform.localPosition = Vector3.Lerp(m_RectTransform.localPosition, m_ScrollerLerpPos, Time.deltaTime * m_ScrollerLerpSpeed);
		if (InputManager.GetKeyDownAction(EGameAction.InteractLeft))
		{
			mStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			mSwipeStartTime = Time.time;
			m_FingerTouched = true;
		}
		if (m_FingerTouched)
		{
			m_FingerTouchTime += Time.deltaTime;
		}
		Vector2 vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mStartPosition;
		float num = Time.time - mSwipeStartTime;
		_ = vector.magnitude / num;
		if (m_FingerTouchTime > 0.01f && m_RectTransform.localPosition.y <= m_ScrollLimitMinY && m_RectTransform.localPosition.y >= m_ScrollLimitMaxY)
		{
			float num2 = 0f;
			num2 = ((!m_IsScrollingInverse) ? (vector.y * m_ScrollSensitivity * -1f) : (vector.y * m_ScrollSensitivity));
			m_ScrollerLerpPos += new Vector3(0f, num2, 0f);
			mStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (m_ScrollerLerpPos.y > m_ScrollLimitMinY)
			{
				m_ScrollerLerpPos = new Vector3(0f, m_ScrollLimitMinY, 0f);
			}
			else if (m_ScrollerLerpPos.y < m_ScrollLimitMaxY)
			{
				m_ScrollerLerpPos = new Vector3(0f, m_ScrollLimitMaxY, 0f);
			}
			if (m_RectTransform.localPosition.y > m_ScrollLimitMinY)
			{
				m_RectTransform.localPosition = new Vector3(0f, m_ScrollLimitMinY, 0f);
			}
			else if (m_RectTransform.localPosition.y < m_ScrollLimitMaxY)
			{
				m_RectTransform.localPosition = new Vector3(0f, m_ScrollLimitMaxY, 0f);
			}
		}
		if (!Input.GetMouseButtonUp(0) || !m_FingerTouched)
		{
			return;
		}
		m_FingerTouched = false;
		m_FingerTouchTime = 0f;
		num = Time.time - mSwipeStartTime;
		vector = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - mStartPosition;
		if (vector.magnitude / num > 0.1f && vector.magnitude > 0.1f)
		{
			vector.Normalize();
			float f = Vector2.Dot(vector, mXAxis);
			f = Mathf.Acos(f) * 57.29578f;
			if (f < 44f)
			{
				OnSwipeRight();
				return;
			}
			if (180f - f < 44f)
			{
				OnSwipeLeft();
				return;
			}
			f = Vector2.Dot(vector, mYAxis);
			f = Mathf.Acos(f) * 57.29578f;
			if (f < 44f)
			{
				OnSwipeTop();
			}
			else if (180f - f < 44f)
			{
				OnSwipeBottom();
			}
		}
		else
		{
			OnTapDetection();
		}
	}

	private void ResetTouch()
	{
		m_FingerTouched = false;
		m_FingerTouchTime = 0f;
		m_ScrollerLerpPos = new Vector3(0f, 0f, 0f);
	}

	private void OnTapDetection()
	{
	}

	private void OnSwipeLeft()
	{
	}

	private void OnSwipeRight()
	{
	}

	private void OnSwipeTop()
	{
	}

	private void OnSwipeBottom()
	{
	}
}
