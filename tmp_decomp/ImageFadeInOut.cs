using UnityEngine;
using UnityEngine.UI;

public class ImageFadeInOut : MonoBehaviour
{
	public Image m_Image;

	public bool m_FadeOutOnStart = true;

	private bool m_IsFadingIn;

	private bool m_IsFadingOut;

	private float m_DefaultFadeAlpha;

	private float m_Timer;

	private float m_FadeSpeed = 1f;

	private float m_Delay;

	private Color m_Color;

	private void Awake()
	{
		m_DefaultFadeAlpha = m_Image.color.a;
		m_Color = m_Image.color;
		if (m_FadeOutOnStart)
		{
			m_Color.a = 0f;
			m_Image.color = m_Color;
		}
		m_Image.enabled = true;
	}

	public void SetFadeIn(float fadeSpeed = 1f, float delay = 0f)
	{
		m_IsFadingIn = true;
		m_IsFadingOut = false;
		m_FadeSpeed = fadeSpeed;
		m_Delay = delay;
	}

	public void SetFadeOut(float fadeSpeed = 1f, float delay = 0f)
	{
		m_IsFadingIn = false;
		m_IsFadingOut = true;
		m_FadeSpeed = fadeSpeed;
		m_Delay = delay;
	}

	private void Update()
	{
		if (m_Delay > 0f)
		{
			m_Delay -= Time.deltaTime;
		}
		else if (m_IsFadingIn)
		{
			m_Timer += Time.deltaTime * m_FadeSpeed;
			if (m_Timer >= 1f)
			{
				m_Timer = 1f;
				m_IsFadingIn = false;
			}
			m_Color.a = Mathf.Lerp(0f, m_DefaultFadeAlpha, m_Timer);
			m_Image.color = m_Color;
		}
		else if (m_IsFadingOut)
		{
			m_Timer -= Time.deltaTime * m_FadeSpeed;
			if (m_Timer <= 0f)
			{
				m_Timer = 0f;
				m_IsFadingOut = false;
			}
			m_Color.a = Mathf.Lerp(0f, m_DefaultFadeAlpha, m_Timer);
			m_Image.color = m_Color;
		}
	}
}
