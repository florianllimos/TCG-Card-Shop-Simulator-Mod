using TMPro;
using UnityEngine;

public class TouchKeyboardManager : CSingleton<TouchKeyboardManager>
{
	private static TouchScreenKeyboard touchScreenKeyboard;

	private static string inputText = string.Empty;

	private static int m_MaxLength = 1000;

	private static TextMeshProUGUI gt;

	private static bool m_IsWaitingInput;

	private static bool m_DoSpecialCharacterCheck = true;

	public static bool StartInput(string startText, int maxLength, bool IsHidden = false, bool doSpecialCharacterCheck = true)
	{
		if (m_IsWaitingInput)
		{
			return false;
		}
		if (gt == null)
		{
			gt = CSingleton<TouchKeyboardManager>.Instance.gameObject.AddComponent<TextMeshProUGUI>();
		}
		m_IsWaitingInput = true;
		m_DoSpecialCharacterCheck = doSpecialCharacterCheck;
		if (startText == null)
		{
			startText = "";
		}
		gt.text = startText;
		if (maxLength == 0)
		{
			m_MaxLength = 1000;
		}
		else
		{
			m_MaxLength = maxLength;
		}
		inputText = startText;
		touchScreenKeyboard = TouchScreenKeyboard.Open(inputText, TouchScreenKeyboardType.ASCIICapable, autocorrection: false, multiline: false, IsHidden);
		if (touchScreenKeyboard == null)
		{
			return false;
		}
		return true;
	}

	public static void CancelInput()
	{
		touchScreenKeyboard = null;
		m_IsWaitingInput = false;
	}

	private void Update()
	{
		if (touchScreenKeyboard != null)
		{
			DetectPCInput();
			DetectMobileInput();
		}
	}

	private void DetectMobileInput()
	{
		if (!Application.isMobilePlatform)
		{
			return;
		}
		inputText = touchScreenKeyboard.text;
		if (inputText.Length > m_MaxLength)
		{
			inputText = inputText.Substring(0, m_MaxLength);
		}
		if (touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done || touchScreenKeyboard.status == TouchScreenKeyboard.Status.LostFocus)
		{
			string text = inputText;
			if (text != null && text != "" && m_DoSpecialCharacterCheck)
			{
				text = text.Replace('_', ' ');
				text = text.Replace(',', ' ');
				text = text.Replace('`', ' ');
			}
			Debug.Log("User typed in " + text);
			CEventManager.QueueEvent(new CEventPlayer_WaitForKeyboardInput(text));
			touchScreenKeyboard = null;
			m_IsWaitingInput = false;
		}
		else if (touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
		{
			CEventManager.QueueEvent(new CEventPlayer_WaitForKeyboardInput(""));
			touchScreenKeyboard = null;
			m_IsWaitingInput = false;
		}
	}

	private void DetectPCInput()
	{
		if (Application.isMobilePlatform)
		{
			return;
		}
		Debug.Log(gt.text);
		string inputString = Input.inputString;
		for (int i = 0; i < inputString.Length; i++)
		{
			char c = inputString[i];
			switch (c)
			{
			case '\b':
				if (gt.text.Length == 0)
				{
				}
				break;
			case '\n':
			case '\r':
			{
				string text = gt.text;
				if (text != null && text != "" && m_DoSpecialCharacterCheck)
				{
					text = text.Replace('_', ' ');
					text = text.Replace(',', ' ');
					text = text.Replace('`', ' ');
				}
				Debug.Log("User typed in " + text);
				CEventManager.QueueEvent(new CEventPlayer_WaitForKeyboardInput(text));
				touchScreenKeyboard = null;
				m_IsWaitingInput = false;
				break;
			}
			default:
				gt.text += c;
				break;
			}
		}
	}
}
