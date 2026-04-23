using UnityEngine;

public class ScreenRatioScaler : MonoBehaviour
{
	public Transform m_Transform;

	public float m_MinScale = 0.3f;

	public float m_MaxScale = 0.925f;

	public bool m_InitOnStart;

	public bool m_UseMaxScaleOnly = true;

	public bool m_SteamDeckBigScaling;

	private bool m_IsSteamDeckResolution;

	public float m_SteamDeckScale = 1.2f;

	private bool m_HasInit;

	private float m_OriginalScale = 1f;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!m_HasInit)
		{
			m_HasInit = true;
			m_OriginalScale = m_Transform.localScale.x;
		}
		m_IsSteamDeckResolution = (Screen.width < 1281 && Screen.height < 901) || CSingleton<InputManager>.Instance.m_IsSteamDeck;
	}

	private void Start()
	{
		Init();
		if (!m_InitOnStart)
		{
			return;
		}
		if (m_SteamDeckBigScaling)
		{
			if (m_IsSteamDeckResolution)
			{
				m_Transform.localScale = Vector3.one * m_SteamDeckScale;
			}
			return;
		}
		float num = (float)Screen.width / (float)Screen.height;
		if (num < 1.61f)
		{
			float num2 = Mathf.Lerp(m_MinScale, m_MaxScale, num / 1.6f);
			if (m_UseMaxScaleOnly)
			{
				num2 = m_MaxScale;
			}
			m_Transform.localScale = Vector3.one * num2;
		}
	}

	private void OnEnable()
	{
		Init();
		if (m_SteamDeckBigScaling)
		{
			if (m_IsSteamDeckResolution)
			{
				m_Transform.localScale = Vector3.one * m_SteamDeckScale;
			}
			return;
		}
		float num = (float)Screen.width / (float)Screen.height;
		if (num < 1.61f)
		{
			float num2 = Mathf.Lerp(m_MinScale, m_MaxScale, num / 1.6f);
			if (m_UseMaxScaleOnly)
			{
				num2 = m_MaxScale;
			}
			m_Transform.localScale = Vector3.one * num2;
		}
		else
		{
			m_Transform.localScale = Vector3.one * m_OriginalScale;
		}
	}
}
