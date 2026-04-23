using UnityEngine;

public class TouchScrollHorizontal : MonoBehaviour
{
	public float m_ScrollLimitMinX = 60f;

	public float m_ScrollLimitMaxX = -500f;

	public float m_ScrollerLerpSpeed = 5f;

	public float m_ScrollSensitivity = 0.4f;

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

	private void Start()
	{
		m_RectTransform = GetComponent<RectTransform>();
		m_ScrollIndicatorStartPos = m_ScrollIndicator.transform.position;
		m_ScrollIndicatorEndPos = m_ScrollIndicatorEnd.transform.position;
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
		m_ScrollIndicatorLerpAlpha = m_RectTransform.localPosition.x / m_ScrollLimitMaxX;
		m_ScrollIndicator.transform.position = Vector3.Lerp(m_ScrollIndicatorStartPos, m_ScrollIndicatorEndPos, m_ScrollIndicatorLerpAlpha);
	}

	public void IsMouseDown()
	{
	}

	public void IsMouseUp()
	{
	}

	public void SetScrollLerpPos(float posX)
	{
		m_ScrollerLerpPos = new Vector3(posX, 0f, 0f);
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
		if (m_FingerTouchTime > 0.001f && m_RectTransform.localPosition.x <= m_ScrollLimitMinX && m_RectTransform.localPosition.x >= m_ScrollLimitMaxX)
		{
			float num2 = 0f;
			num2 = ((!m_IsScrollingInverse) ? (vector.x * m_ScrollSensitivity * -1f * 2f) : (vector.x * m_ScrollSensitivity));
			m_ScrollerLerpPos += new Vector3(num2, 0f, 0f);
			mStartPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (m_ScrollerLerpPos.x > m_ScrollLimitMinX)
			{
				m_ScrollerLerpPos = new Vector3(m_ScrollLimitMinX, 0f, 0f);
			}
			else if (m_ScrollerLerpPos.x < m_ScrollLimitMaxX)
			{
				m_ScrollerLerpPos = new Vector3(m_ScrollLimitMaxX, 0f, 0f);
			}
			if (m_RectTransform.localPosition.x > m_ScrollLimitMinX)
			{
				m_RectTransform.localPosition = new Vector3(m_ScrollLimitMinX, 0f, 0f);
			}
			else if (m_RectTransform.localPosition.x < m_ScrollLimitMaxX)
			{
				m_RectTransform.localPosition = new Vector3(m_ScrollLimitMaxX, 0f, 0f);
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
