using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VirtualKeyboardScreenUI : UIScreenBase
{
	public TextMeshProUGUI m_OutputShowText;

	private TMP_InputField m_InputText;

	public List<VirtualKeyboardBtnKey> m_VirtualKeyboardBtnKeyList;

	public bool m_IsAltActive;

	private bool m_ShowLineCursor;

	private int m_MaxLength = 30;

	private float m_Timer;

	private string m_CurrentString = "";

	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < m_VirtualKeyboardBtnKeyList.Count; i++)
		{
			m_VirtualKeyboardBtnKeyList[i].Init(this);
		}
	}

	protected override void RunUpdate()
	{
		base.RunUpdate();
		m_Timer += Time.deltaTime;
		if (m_Timer >= 1f)
		{
			m_Timer = 0f;
			m_ShowLineCursor = !m_ShowLineCursor;
			UpdateOutputText();
		}
	}

	private void UpdateOutputText()
	{
		if (m_ShowLineCursor)
		{
			m_OutputShowText.text = m_CurrentString + "|";
		}
		else
		{
			m_OutputShowText.text = m_CurrentString + "<alpha=#00>|</color>";
		}
	}

	public void SetInitialText(string text, TMP_InputField inputText)
	{
		GameInstance.m_IsVirtualKeyboardActive = true;
		m_InputText = inputText;
		m_CurrentString = text;
		UpdateOutputText();
	}

	public void OnPressButton(string text)
	{
		if (m_CurrentString.Length < m_MaxLength)
		{
			m_CurrentString += text;
			UpdateOutputText();
		}
	}

	public void OnPressBackKey()
	{
		if (m_CurrentString.Length > 0)
		{
			m_CurrentString = m_CurrentString.Remove(m_CurrentString.Length - 1, 1);
			UpdateOutputText();
		}
	}

	public void OnPressCapitalKey()
	{
		ToggleAltActive();
	}

	public void OnPressDoneKey()
	{
		m_InputText.text = m_CurrentString;
		m_InputText.onEndEdit.Invoke(m_CurrentString);
		CloseScreen();
	}

	protected override void OnCloseScreen()
	{
		GameInstance.m_IsVirtualKeyboardActive = false;
		base.OnCloseScreen();
	}

	public void OnPressSpaceKey()
	{
		if (m_CurrentString.Length < m_MaxLength)
		{
			m_CurrentString += " ";
			UpdateOutputText();
		}
	}

	public void ToggleAltActive()
	{
		m_IsAltActive = !m_IsAltActive;
		for (int i = 0; i < m_VirtualKeyboardBtnKeyList.Count; i++)
		{
			m_VirtualKeyboardBtnKeyList[i].EvaluateKey();
		}
	}
}
