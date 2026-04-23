using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamepadSettingLinker : MonoBehaviour
{
	public Toggle m_Toggle;

	public TMP_Dropdown m_Dropdown;

	public Slider m_Slider;

	public GameObject m_SliderActiveThumbstickIcon;

	public KeybindSetting m_KeybindSetting;

	private bool m_IsShown;

	private bool m_CanUpdateKeybind = true;

	private List<Selectable> m_SelectableList = new List<Selectable>();

	private int m_ScrollElementCount;

	private ScrollRect m_ScrollRect;

	private Scrollbar m_ScrollBar;

	private GameObject m_DropdownObject;

	private int m_CurrentDropDownIndex;

	private void Awake()
	{
		if ((bool)m_SliderActiveThumbstickIcon)
		{
			m_SliderActiveThumbstickIcon.SetActive(value: false);
		}
	}

	public void OnPressConfirm()
	{
		if ((bool)m_Toggle)
		{
			m_Toggle.isOn = !m_Toggle.isOn;
		}
		if ((bool)m_Dropdown)
		{
			if (!m_IsShown)
			{
				m_Dropdown.Show();
				m_IsShown = true;
				ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: true);
				CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: true);
				InputManager.SetSliderActive(isActive: true);
				m_CurrentDropDownIndex = m_Dropdown.value;
				StartCoroutine(DelayEvaluateCurrentDropdownPosition());
			}
			else
			{
				m_Dropdown.Hide();
				m_Dropdown.interactable = false;
				m_Dropdown.interactable = true;
				m_IsShown = false;
				ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: false);
				CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: false);
				InputManager.SetSliderActive(isActive: false);
			}
		}
		if ((bool)m_Slider)
		{
			if (!m_IsShown)
			{
				m_Slider.interactable = true;
				m_Slider.Select();
				m_IsShown = true;
				ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: true);
				CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: true);
				m_SliderActiveThumbstickIcon.SetActive(value: true);
				InputManager.SetSliderActive(isActive: true);
			}
			else
			{
				m_Slider.interactable = false;
				m_Slider.interactable = true;
				m_IsShown = false;
				ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: false);
				CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: false);
				m_SliderActiveThumbstickIcon.SetActive(value: false);
				InputManager.SetSliderActive(isActive: false);
			}
		}
		if ((bool)m_KeybindSetting && m_KeybindSetting.m_IsGamepad && m_CanUpdateKeybind && !m_IsShown)
		{
			m_IsShown = true;
			m_KeybindSetting.OnPressButton();
			ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: true);
		}
	}

	public void OnPressCancel()
	{
		if ((bool)m_Dropdown && m_IsShown)
		{
			m_Dropdown.Hide();
			m_Dropdown.interactable = false;
			m_Dropdown.interactable = true;
			m_IsShown = false;
			ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: false);
			CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: false);
			InputManager.SetSliderActive(isActive: false);
		}
		if ((bool)m_Slider && m_IsShown)
		{
			m_Slider.interactable = false;
			m_Slider.interactable = true;
			m_IsShown = false;
			ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: false);
			CSingleton<SettingScreen>.Instance.SetIsChangingKeybind(isChangingKeybind: false);
			InputManager.SetSliderActive(isActive: false);
			m_SliderActiveThumbstickIcon.SetActive(value: false);
		}
	}

	public void DropdownOnPressUp()
	{
		if ((bool)m_Dropdown && m_IsShown)
		{
			StartCoroutine(DelayEvaluateCurrentDropdownPosition(updateDropDownIndex: true));
		}
	}

	public void DropdownOnPressDown()
	{
		if ((bool)m_Dropdown && m_IsShown)
		{
			StartCoroutine(DelayEvaluateCurrentDropdownPosition(updateDropDownIndex: true));
		}
	}

	private IEnumerator DelayEvaluateCurrentDropdownPosition(bool updateDropDownIndex = false)
	{
		yield return new WaitForSecondsRealtime(0.01f);
		if (m_DropdownObject == null)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				if (base.transform.GetChild(i).transform.name == "Dropdown List")
				{
					m_DropdownObject = base.transform.GetChild(i).gameObject;
					break;
				}
			}
			if (m_DropdownObject == null)
			{
				m_DropdownObject = GameObject.Find("Dropdown List");
			}
			m_ScrollRect = m_DropdownObject.GetComponent<ScrollRect>();
			if ((bool)m_ScrollRect)
			{
				m_ScrollRect.content.GetComponentsInChildren(m_SelectableList);
				if (m_DropdownObject.GetComponentsInChildren<Scrollbar>().Length != 0)
				{
					m_ScrollBar = m_DropdownObject.GetComponentsInChildren<Scrollbar>()[0];
				}
				m_ScrollElementCount = m_SelectableList.Count;
			}
		}
		if (!m_ScrollBar)
		{
			yield break;
		}
		if (updateDropDownIndex)
		{
			if (CSingleton<InputManager>.Instance.m_CurrentMouse != null)
			{
				CSingleton<InputManager>.Instance.m_CurrentMouse.WarpCursorPosition(Vector2.zero);
			}
			Selectable selectable = (EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null);
			if ((bool)selectable)
			{
				m_CurrentDropDownIndex = m_SelectableList.IndexOf(selectable);
			}
		}
		if (m_CurrentDropDownIndex > -1)
		{
			m_ScrollRect.verticalNormalizedPosition = 1f - (float)m_CurrentDropDownIndex / ((float)m_ScrollElementCount - 1f);
		}
	}

	public void OnFinishSetGamepadKeybind()
	{
		if ((bool)m_KeybindSetting && m_IsShown)
		{
			m_IsShown = false;
			ControllerScreenUIExtManager.SetLockLJoystickVertical(isLock: false);
			StartCoroutine(ResetCanUpdateKeybind());
		}
	}

	private IEnumerator ResetCanUpdateKeybind()
	{
		m_CanUpdateKeybind = false;
		yield return new WaitForSecondsRealtime(0.1f);
		m_CanUpdateKeybind = true;
	}
}
