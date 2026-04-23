using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScreenUIExtManager : CSingleton<ControllerScreenUIExtManager>
{
	public bool m_IsControllerActive;

	public bool m_LockLJoystickVertical;

	public bool m_LockLJoystickHorizontal;

	public List<GraphicRaycaster> m_GraphicRaycasterList;

	public ControllerSelectorUIGrp m_ControllerSelectorUIGrp;

	public VirtualKeyboardScreenUI m_VirtualKeyboardScreenUI;

	public ControllerScreenUIExtension m_CurrentCtrlScreenUIExt;

	public List<ControllerScreenUIExtension> m_ActiveCtrlScreenUIExtList;

	private ControllerButton m_CurrentControllerButton;

	private float m_TimeSkipUpdate;

	public void Update()
	{
		if (m_TimeSkipUpdate > 0f)
		{
			m_TimeSkipUpdate -= Time.unscaledDeltaTime;
			if (m_TimeSkipUpdate <= 0f)
			{
				m_TimeSkipUpdate = 0f;
			}
		}
		else if (m_IsControllerActive && (bool)m_CurrentCtrlScreenUIExt)
		{
			m_CurrentCtrlScreenUIExt.RunUpdate();
		}
	}

	public static void SetLockLJoystickVertical(bool isLock)
	{
		CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickVertical = isLock;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickHorizontal = isLock;
	}

	public static void StartVirtualKeyboard(string initText, TMP_InputField inputText)
	{
		if (CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Count > 0)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt = CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList[CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Count - 1];
			CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt.ResetButtonHold();
		}
		CSingleton<ControllerScreenUIExtManager>.Instance.m_VirtualKeyboardScreenUI.SetInitialText(initText, inputText);
		CSingleton<ControllerScreenUIExtManager>.Instance.m_VirtualKeyboardScreenUI.OpenScreen();
	}

	public static void SetControllerActive(bool isActive)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentControllerButton != null)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_TopUIGrp.SetActive(isActive);
		}
		else
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_TopUIGrp.SetActive(value: false);
		}
		CSingleton<ControllerScreenUIExtManager>.Instance.m_IsControllerActive = isActive;
		SetGraphicRaycasterActive(!isActive);
		if (isActive)
		{
			if (CSingleton<InputManager>.Instance.m_CurrentMouse != null)
			{
				CSingleton<InputManager>.Instance.m_CurrentMouse.WarpCursorPosition(Vector2.zero);
			}
			CSingleton<TouchManager>.Instance.ResetSwipe();
		}
		else if ((bool)CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt.HideCurrentControllerButtonOverlayHighlight();
		}
	}

	public static void OnOpenScreen(ControllerScreenUIExtension ctrlScreenUIExt)
	{
		if ((bool)ctrlScreenUIExt)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Add(ctrlScreenUIExt);
			CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt = ctrlScreenUIExt;
			CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt.OnOpenScreen();
			CSingleton<ControllerScreenUIExtManager>.Instance.m_TimeSkipUpdate = 0.02f;
		}
	}

	public static void OnCloseScreen(ControllerScreenUIExtension ctrlScreenUIExt)
	{
		if ((bool)ctrlScreenUIExt)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Remove(ctrlScreenUIExt);
			if (CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Count > 0)
			{
				CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt = CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList[CSingleton<ControllerScreenUIExtManager>.Instance.m_ActiveCtrlScreenUIExtList.Count - 1];
				CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt.ResetButtonHold();
				CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt.OnCloseChildScreen();
			}
			else
			{
				CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentCtrlScreenUIExt = null;
			}
		}
	}

	public static void SetControllerSelectorUI(ControllerButton controllerButton, float rectTransformOffsetMultiplier = 1f, float btnHighlightScale = 1f)
	{
		CSingleton<ControllerScreenUIExtManager>.Instance.m_CurrentControllerButton = controllerButton;
		if (controllerButton == null)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_TopUIGrp.SetActive(value: false);
			return;
		}
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.parent = controllerButton.transform;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.localPosition = Vector3.zero;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.localRotation = Quaternion.identity;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.localScale = Vector3.one;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.SetSpriteGrpScale(btnHighlightScale);
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.anchorMin = new Vector2(0f, 0f);
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.anchorMax = new Vector2(1f, 1f);
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.pivot = new Vector2(0.5f, 0.5f);
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.offsetMin = Vector2.one * 60f * rectTransformOffsetMultiplier;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_Rect.offsetMax = Vector2.one * -60f * rectTransformOffsetMultiplier;
		CSingleton<ControllerScreenUIExtManager>.Instance.m_ControllerSelectorUIGrp.m_TopUIGrp.SetActive(value: true);
	}

	public static void SetGraphicRaycasterActive(bool isEnabled)
	{
		for (int i = 0; i < CSingleton<ControllerScreenUIExtManager>.Instance.m_GraphicRaycasterList.Count; i++)
		{
			CSingleton<ControllerScreenUIExtManager>.Instance.m_GraphicRaycasterList[i].enabled = isEnabled;
		}
	}
}
