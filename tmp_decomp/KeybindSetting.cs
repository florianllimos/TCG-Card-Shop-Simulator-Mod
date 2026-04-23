using System.Collections;
using UnityEngine;

public class KeybindSetting : MonoBehaviour
{
	public EGameBaseKey m_GameBaseKey;

	public EGameBaseKey m_GameBaseKeySecondary;

	public bool m_IsGamepad;

	public GameObject m_UpdatingGrp;

	public InputTooltipUI m_TooltipUI;

	public GamepadSettingLinker m_GamepadSettingLinker;

	public void Start()
	{
		UpdateInputUI();
	}

	public void UpdateInputUI()
	{
		if (m_IsGamepad)
		{
			EGamepadControlBtn baseGamepadBtnBind = InputManager.GetBaseGamepadBtnBind(m_GameBaseKey);
			m_TooltipUI.SetGamepadInputTooltip(EGameAction.None, baseGamepadBtnBind, InputManager.GetGamepadButtonName(baseGamepadBtnBind), isHold: false);
		}
		else
		{
			KeyCode baseKeycodeBind = InputManager.GetBaseKeycodeBind(m_GameBaseKey);
			m_TooltipUI.SetInputTooltip(EGameAction.None, baseKeycodeBind, InputManager.GetKeycodeName(baseKeycodeBind), isHold: false);
		}
	}

	public void OnPressButton()
	{
		if ((!m_IsGamepad || CSingleton<InputManager>.Instance.m_IsControllerActive) && (m_IsGamepad || !CSingleton<InputManager>.Instance.m_IsControllerActive) && !SettingScreen.IsChangingKeybind())
		{
			m_UpdatingGrp.SetActive(value: true);
			CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: true);
			InputManager.OnPressKeybindButton(this, m_IsGamepad, m_GameBaseKey, m_GameBaseKeySecondary);
		}
	}

	public void OnFinishSetKeybind(KeyCode keycode)
	{
		m_UpdatingGrp.SetActive(value: false);
		CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: false);
		UpdateInputUI();
	}

	public void OnFinishSetGamepadKeybind(EGamepadControlBtn controlBtn)
	{
		if ((bool)m_GamepadSettingLinker)
		{
			m_GamepadSettingLinker.OnFinishSetGamepadKeybind();
		}
		m_UpdatingGrp.SetActive(value: false);
		StartCoroutine(DelaySetIsChangingKeybind(isChangingKeybind: false));
		UpdateInputUI();
	}

	private IEnumerator DelaySetIsChangingKeybind(bool isChangingKeybind)
	{
		yield return new WaitForSecondsRealtime(0.1f);
		CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind);
	}

	private IEnumerator SetIsChangingKeybindNextFrame(bool isChangingKeybind)
	{
		yield return null;
		CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind);
		InputManager.OnPressKeybindButton(this, m_IsGamepad, m_GameBaseKey, m_GameBaseKeySecondary);
	}
}
