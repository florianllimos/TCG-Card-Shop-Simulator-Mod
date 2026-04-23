using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputTooltipUI : MonoBehaviour
{
	public Image m_KeyImage;

	public TextMeshProUGUI m_KeycodeText;

	public TextMeshProUGUI m_ActionText;

	public bool m_IsActive;

	public Transform m_Transform;

	public EGameAction m_CurrentGameAction;

	private bool m_IsHold;

	private KeyCode m_KeyCode;

	private EGamepadControlBtn m_GamepadControlBtn = EGamepadControlBtn.Start;

	private string m_ActionName = "";

	public void SetActive(bool isActive)
	{
		m_IsActive = isActive;
		base.gameObject.SetActive(isActive);
	}

	public void SetGamepadInputTooltip(EGameAction gameAction, EGamepadControlBtn gamepadControlBtn, string actionName, bool isHold)
	{
		m_CurrentGameAction = gameAction;
		m_ActionText.text = actionName;
		m_GamepadControlBtn = gamepadControlBtn;
		m_IsHold = isHold;
		m_KeycodeText.text = "";
		if (CSingleton<InputManager>.Instance.m_IsPSController)
		{
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_PSCtrlBtnSpriteList[(int)m_GamepadControlBtn];
		}
		else
		{
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_XBoxCtrlBtnSpriteList[(int)m_GamepadControlBtn];
		}
	}

	public void SetInputTooltip(EGameAction gameAction, KeyCode keycode, string actionName, bool isHold)
	{
		m_CurrentGameAction = gameAction;
		m_KeycodeText.text = keycode.ToString();
		m_ActionText.text = actionName;
		m_KeyCode = keycode;
		m_ActionName = actionName;
		m_IsHold = isHold;
		switch (keycode)
		{
		case KeyCode.Mouse0:
			m_KeycodeText.text = "";
			if (isHold)
			{
				m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_LeftMouseHoldImage;
			}
			else
			{
				m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_LeftMouseClickImage;
			}
			return;
		case KeyCode.Mouse1:
			m_KeycodeText.text = "";
			if (isHold)
			{
				m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_RightMouseHoldImage;
			}
			else
			{
				m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_RightMouseClickImage;
			}
			return;
		case KeyCode.Mouse2:
			m_ActionText.text = m_ActionText.text;
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_MiddleMouseScrollImage;
			return;
		case KeyCode.Return:
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_EnterImage;
			return;
		case KeyCode.Space:
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_SpacebarImage;
			return;
		case KeyCode.Tab:
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_TabImage;
			return;
		case KeyCode.RightShift:
		case KeyCode.LeftShift:
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_ShiftImage;
			return;
		case KeyCode.RightControl:
		case KeyCode.LeftControl:
			m_KeycodeText.text = "";
			m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_CtrlImage;
			return;
		case KeyCode.Escape:
			m_KeycodeText.text = "Esc";
			break;
		case KeyCode.Backspace:
			m_KeycodeText.text = "BSpace";
			break;
		}
		m_KeyImage.sprite = CSingleton<CGameManager>.Instance.m_TextSO.m_KeyboardBtnImage;
	}
}
