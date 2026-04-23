using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ControllerButton : MonoBehaviour
{
	public bool m_CanScrollerSlide = true;

	public Button m_Button;

	public GameObject m_BGHighlight;

	public ControllerSelectorUIGrp m_ButtonHighlight;

	public float m_RectTransformOffsetMultiplier = 1f;

	public float m_BtnHighlightSpriteScale = 1f;

	public List<Button> m_OverlayButton;

	public List<GameObject> m_OverlayButtonHighlight;

	public bool m_EnableVirtualKeyboard;

	public bool m_InitVirtualKeyboardWithInputString;

	public TMP_InputField m_VirtualKeyboardInputText;

	public GamepadSettingLinker m_GamepadSettingLinker;

	public UnityEvent m_OnSelectedEvent;

	public UnityEvent m_OnDeselectedEvent;

	public UnityEvent m_OnPressLeftEvent;

	public UnityEvent m_OnPressRightEvent;

	private bool m_IsIgnoreSelection;

	public bool m_RemoveOnConsoles;

	private void Awake()
	{
		if ((bool)m_ButtonHighlight)
		{
			m_ButtonHighlight.SetActive(isActive: false);
		}
		for (int i = 0; i < m_OverlayButtonHighlight.Count; i++)
		{
			m_OverlayButtonHighlight[i].SetActive(value: false);
		}
		m_RemoveOnConsoles = false;
	}

	public void OnSelectionActive()
	{
		if (!CSingleton<ControllerScreenUIExtManager>.Instance.m_IsControllerActive)
		{
			return;
		}
		if (m_OnSelectedEvent != null)
		{
			m_OnSelectedEvent.Invoke();
		}
		if (!m_ButtonHighlight && m_OverlayButtonHighlight.Count == 0)
		{
			ControllerScreenUIExtManager.SetControllerSelectorUI(this, m_RectTransformOffsetMultiplier, m_BtnHighlightSpriteScale);
			return;
		}
		for (int i = 0; i < m_OverlayButtonHighlight.Count; i++)
		{
			m_OverlayButtonHighlight[i].SetActive(value: true);
		}
		if ((bool)m_ButtonHighlight)
		{
			m_ButtonHighlight.SetActive(isActive: true);
		}
	}

	public void OnSelectionDeactivate()
	{
		if (m_OnDeselectedEvent != null)
		{
			m_OnDeselectedEvent.Invoke();
		}
		if (!m_ButtonHighlight && m_OverlayButtonHighlight.Count == 0)
		{
			ControllerScreenUIExtManager.SetControllerSelectorUI(null);
			return;
		}
		for (int i = 0; i < m_OverlayButtonHighlight.Count; i++)
		{
			m_OverlayButtonHighlight[i].SetActive(value: false);
		}
		if ((bool)m_ButtonHighlight)
		{
			m_ButtonHighlight.SetActive(isActive: false);
		}
	}

	public void OnPressConfirm()
	{
		if ((bool)m_GamepadSettingLinker)
		{
			m_GamepadSettingLinker.OnPressConfirm();
			return;
		}
		for (int i = 0; i < m_OverlayButton.Count; i++)
		{
			if (m_OverlayButton[i].gameObject.activeInHierarchy)
			{
				m_OverlayButton[i].onClick.Invoke();
				return;
			}
		}
		if (m_EnableVirtualKeyboard)
		{
			if (m_InitVirtualKeyboardWithInputString)
			{
				ControllerScreenUIExtManager.StartVirtualKeyboard(m_VirtualKeyboardInputText.text, m_VirtualKeyboardInputText);
			}
			else
			{
				ControllerScreenUIExtManager.StartVirtualKeyboard("", m_VirtualKeyboardInputText);
			}
		}
		else
		{
			m_Button.onClick.Invoke();
		}
	}

	public bool OnPressCancel()
	{
		if ((bool)m_GamepadSettingLinker && InputManager.IsSliderActive())
		{
			m_GamepadSettingLinker.OnPressCancel();
			return true;
		}
		return false;
	}

	public bool DropdownOnPressUp()
	{
		if ((bool)m_GamepadSettingLinker)
		{
			m_GamepadSettingLinker.DropdownOnPressUp();
			return true;
		}
		return false;
	}

	public bool DropdownOnPressDown()
	{
		if ((bool)m_GamepadSettingLinker)
		{
			m_GamepadSettingLinker.DropdownOnPressDown();
			return true;
		}
		return false;
	}

	public void OnPressLeft()
	{
		if (m_OnPressLeftEvent != null)
		{
			m_OnPressLeftEvent.Invoke();
		}
	}

	public void OnPressRight()
	{
		if (m_OnPressRightEvent != null)
		{
			m_OnPressRightEvent.Invoke();
		}
	}

	public bool IsActive()
	{
		if (m_IsIgnoreSelection)
		{
			return false;
		}
		if ((bool)m_GamepadSettingLinker)
		{
			return base.gameObject.activeInHierarchy;
		}
		for (int i = 0; i < m_OverlayButton.Count; i++)
		{
			Button button = m_OverlayButton[i];
			if (!button)
			{
				Debug.LogError("[ControllerButton] IsActive failed");
			}
			else if (button.gameObject.activeInHierarchy)
			{
				return true;
			}
		}
		if (m_EnableVirtualKeyboard)
		{
			return base.gameObject.activeInHierarchy;
		}
		if (m_Button.gameObject.activeInHierarchy)
		{
			return true;
		}
		return false;
	}

	public void SetBGHighlightVisibility(bool isVisible)
	{
		m_BGHighlight.SetActive(isVisible);
	}

	public void SetIgnoreSelection(bool isIgnoreSelection)
	{
		m_IsIgnoreSelection = isIgnoreSelection;
	}
}
