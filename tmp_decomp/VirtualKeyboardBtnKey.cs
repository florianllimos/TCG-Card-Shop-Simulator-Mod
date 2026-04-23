using TMPro;
using UnityEngine;

public class VirtualKeyboardBtnKey : MonoBehaviour
{
	public TextMeshProUGUI m_KeyText;

	public string m_String;

	public string m_AltString;

	public bool m_IsSpacebar;

	public bool m_IsBack;

	public bool m_IsCapital;

	public bool m_IsDone;

	private VirtualKeyboardScreenUI m_VirtualKeyboardScreenUI;

	public void Init(VirtualKeyboardScreenUI virtualKeyboardScreenUI)
	{
		m_VirtualKeyboardScreenUI = virtualKeyboardScreenUI;
		EvaluateKey();
	}

	public void EvaluateKey()
	{
		if (m_IsCapital || m_IsBack || m_IsSpacebar || m_IsDone)
		{
			return;
		}
		if (m_VirtualKeyboardScreenUI.m_IsAltActive)
		{
			if (m_AltString != null && m_AltString != "")
			{
				m_KeyText.text = m_AltString;
			}
			else
			{
				m_KeyText.text = m_String.ToUpper();
			}
		}
		else
		{
			m_KeyText.text = m_String.ToLower();
		}
	}

	public void OnPressButton()
	{
		if (m_IsCapital)
		{
			m_VirtualKeyboardScreenUI.OnPressCapitalKey();
		}
		else if (m_IsBack)
		{
			m_VirtualKeyboardScreenUI.OnPressBackKey();
		}
		else if (m_IsSpacebar)
		{
			m_VirtualKeyboardScreenUI.OnPressSpaceKey();
		}
		else if (m_IsDone)
		{
			m_VirtualKeyboardScreenUI.OnPressDoneKey();
		}
		else
		{
			m_VirtualKeyboardScreenUI.OnPressButton(m_KeyText.text);
		}
		SoundManager.GenericLightTap(0.4f);
	}
}
