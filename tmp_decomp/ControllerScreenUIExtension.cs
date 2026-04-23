using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerScreenUIExtension : MonoBehaviour
{
	public List<ControllerBtnList> m_ControllerBtnColumnList;

	public List<ECtrlBtnXChangeMethod> m_CtrlBtnXChangeMethodList;

	public Button m_CloseScreenBtn;

	public ControllerButton m_StartControllerButton;

	public ControllerButton m_StartControllerButtonAlt;

	public bool m_LoopWithinRowOnly;

	public bool m_CanLoopY = true;

	public bool m_CanRapidFireConfirmBtn;

	public bool m_SettingScreenLoopSpecial;

	public int m_IndexYWhenLoopXEnd;

	public GenericSliderScreen m_SliderScreen;

	public float m_SliderOffsetY;

	public ControllerButton m_CurrentControllerButton;

	public int m_CurrentCtrlBtnXIndex;

	public int m_CurrentCtrlBtnYIndex;

	public int m_LastChangeMethodCtrlBtnXIndex;

	public bool m_IsHoldingConfirmBtn;

	public bool m_IsHoldingLeftJoystickRight;

	public bool m_IsHoldingLeftJoystickLeft;

	public bool m_IsHoldingLeftJoystickUp;

	public bool m_IsHoldingLeftJoystickDown;

	public bool m_IsJoystickRapidFire;

	public float m_JoystickRapidFireStartTimer;

	public float m_JoystickRapidFireStartTime = 0.5f;

	public float m_JoystickRapidFireTimer;

	public float m_JoystickRapidFireTime = 0.08f;

	private bool m_IsControllerUIActive = true;

	private void Awake()
	{
	}

	public void OnOpenScreen()
	{
		if ((bool)m_CurrentControllerButton)
		{
			m_CurrentControllerButton.OnSelectionDeactivate();
		}
		if ((bool)m_StartControllerButton)
		{
			m_CurrentControllerButton = m_StartControllerButton;
			if (!m_StartControllerButton.gameObject.activeInHierarchy && (bool)m_StartControllerButtonAlt)
			{
				m_CurrentControllerButton = m_StartControllerButtonAlt;
			}
			Vector2 controllerButtonIndex = GetControllerButtonIndex(m_CurrentControllerButton);
			m_CurrentCtrlBtnXIndex = Mathf.RoundToInt(controllerButtonIndex.x);
			m_CurrentCtrlBtnYIndex = Mathf.RoundToInt(controllerButtonIndex.y);
		}
		else
		{
			m_CurrentCtrlBtnXIndex = 0;
			m_CurrentCtrlBtnYIndex = 0;
			m_CurrentControllerButton = GetControllerButton(0, 0);
		}
		m_CurrentControllerButton.OnSelectionActive();
		if (GetCtrlBtnXChangeMethod(m_CurrentCtrlBtnYIndex) == ECtrlBtnXChangeMethod.RememberIndexX)
		{
			m_LastChangeMethodCtrlBtnXIndex = m_CurrentCtrlBtnXIndex;
		}
		ResetButtonHold();
	}

	public void ResetButtonHold()
	{
		m_IsHoldingConfirmBtn = false;
		m_IsHoldingLeftJoystickRight = false;
		m_IsHoldingLeftJoystickLeft = false;
		m_IsHoldingLeftJoystickUp = false;
		m_IsHoldingLeftJoystickDown = false;
		m_IsJoystickRapidFire = false;
		m_JoystickRapidFireStartTimer = 0f;
		m_JoystickRapidFireTimer = 0f;
	}

	public void OnCloseChildScreen()
	{
		if ((bool)m_CurrentControllerButton)
		{
			m_CurrentControllerButton.OnSelectionDeactivate();
			m_CurrentControllerButton.OnSelectionActive();
		}
	}

	private ControllerButton GetControllerButton(int indexX, int indexY)
	{
		if (indexY >= m_ControllerBtnColumnList.Count || indexY < 0)
		{
			return null;
		}
		if (indexX >= m_ControllerBtnColumnList[indexY].rowList.Count || indexX < 0)
		{
			return null;
		}
		return m_ControllerBtnColumnList[indexY].rowList[indexX];
	}

	private Vector2 GetControllerButtonIndex(ControllerButton ctrlBtn)
	{
		Vector2 result = default(Vector2);
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < m_ControllerBtnColumnList.Count; i++)
		{
			for (int j = 0; j < m_ControllerBtnColumnList[i].rowList.Count; j++)
			{
				if (m_ControllerBtnColumnList[i].rowList[j] == ctrlBtn)
				{
					num = j;
					num2 = i;
					break;
				}
			}
		}
		result.x = num;
		result.y = num2;
		return result;
	}

	public void RunUpdate()
	{
		if (!m_IsControllerUIActive || CSingleton<CGameManager>.Instance.m_IsPrologue || !CSingleton<InputManager>.Instance.m_IsControllerActive || CSingleton<CGameManager>.Instance.m_DisableController)
		{
			return;
		}
		if (InputManager.GetKeyDownAction(EGameAction.MenuConfirm))
		{
			if (m_CanRapidFireConfirmBtn)
			{
				m_IsHoldingConfirmBtn = true;
			}
			OnPressConfirm();
		}
		else if (InputManager.GetKeyUpAction(EGameAction.MenuConfirm))
		{
			if (m_CanRapidFireConfirmBtn)
			{
				m_IsHoldingConfirmBtn = false;
				m_JoystickRapidFireStartTimer = 0f;
				m_IsJoystickRapidFire = false;
			}
		}
		else if (InputManager.GetKeyDownAction(EGameAction.MenuBack))
		{
			OnPressCancel();
		}
		if (InputManager.GetLeftAnalogDown(0, positiveValue: true))
		{
			m_IsHoldingLeftJoystickRight = true;
			OnPressRight();
			return;
		}
		if (InputManager.GetLeftAnalogUp(0, positiveValue: true))
		{
			m_IsHoldingLeftJoystickRight = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_R))
		{
			m_IsHoldingLeftJoystickRight = true;
			OnPressRight();
			return;
		}
		if (InputManager.GetKeyUp(EGameBaseKey.JStick_DPad_R))
		{
			m_IsHoldingLeftJoystickRight = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetLeftAnalogDown(0, positiveValue: false))
		{
			m_IsHoldingLeftJoystickLeft = true;
			OnPressLeft();
			return;
		}
		if (InputManager.GetLeftAnalogUp(0, positiveValue: false))
		{
			m_IsHoldingLeftJoystickLeft = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_L))
		{
			m_IsHoldingLeftJoystickLeft = true;
			OnPressLeft();
			return;
		}
		if (InputManager.GetKeyUp(EGameBaseKey.JStick_DPad_L))
		{
			m_IsHoldingLeftJoystickLeft = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetLeftAnalogDown(1, positiveValue: true))
		{
			m_IsHoldingLeftJoystickUp = true;
			OnPressUp();
			return;
		}
		if (InputManager.GetLeftAnalogUp(1, positiveValue: true))
		{
			m_IsHoldingLeftJoystickUp = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Up))
		{
			m_IsHoldingLeftJoystickUp = true;
			OnPressUp();
			return;
		}
		if (InputManager.GetKeyUp(EGameBaseKey.JStick_DPad_Up))
		{
			m_IsHoldingLeftJoystickUp = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetLeftAnalogDown(1, positiveValue: false))
		{
			m_IsHoldingLeftJoystickDown = true;
			OnPressDown();
			return;
		}
		if (InputManager.GetLeftAnalogUp(1, positiveValue: false))
		{
			m_IsHoldingLeftJoystickDown = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (InputManager.GetKeyDown(EGameBaseKey.JStick_DPad_Down))
		{
			m_IsHoldingLeftJoystickDown = true;
			OnPressDown();
			return;
		}
		if (InputManager.GetKeyUp(EGameBaseKey.JStick_DPad_Down))
		{
			m_IsHoldingLeftJoystickDown = false;
			m_JoystickRapidFireStartTimer = 0f;
			m_IsJoystickRapidFire = false;
		}
		if (!m_IsJoystickRapidFire)
		{
			if (m_IsHoldingLeftJoystickRight || m_IsHoldingLeftJoystickLeft || m_IsHoldingLeftJoystickUp || m_IsHoldingLeftJoystickDown || (m_CanRapidFireConfirmBtn && m_IsHoldingConfirmBtn))
			{
				m_JoystickRapidFireStartTimer += Time.unscaledDeltaTime;
				if (m_JoystickRapidFireStartTimer >= m_JoystickRapidFireStartTime)
				{
					m_JoystickRapidFireStartTimer = 0f;
					m_JoystickRapidFireTimer = 0f;
					m_IsJoystickRapidFire = true;
				}
			}
		}
		else
		{
			if (!m_IsJoystickRapidFire)
			{
				return;
			}
			m_JoystickRapidFireTimer += Time.unscaledDeltaTime;
			if (m_JoystickRapidFireTimer >= m_JoystickRapidFireTime)
			{
				m_JoystickRapidFireTimer = 0f;
				if (m_IsHoldingLeftJoystickLeft)
				{
					OnPressLeft();
				}
				else if (m_IsHoldingLeftJoystickRight)
				{
					OnPressRight();
				}
				else if (m_IsHoldingLeftJoystickUp)
				{
					OnPressUp();
				}
				else if (m_IsHoldingLeftJoystickDown)
				{
					OnPressDown();
				}
				else if (m_IsHoldingConfirmBtn)
				{
					OnPressConfirm();
				}
			}
		}
	}

	public void UpdateControllerBtnIndex_X(int addX)
	{
		if (!m_IsControllerUIActive)
		{
			return;
		}
		m_CurrentCtrlBtnXIndex += addX;
		if (m_CurrentCtrlBtnXIndex < 0)
		{
			if (m_LoopWithinRowOnly)
			{
				for (int num = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count - 1; num >= 0; num--)
				{
					ControllerButton controllerButton = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[num];
					if (!controllerButton)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
					}
					else if (controllerButton.IsActive())
					{
						m_CurrentCtrlBtnXIndex = num;
						break;
					}
				}
			}
			else if (m_SettingScreenLoopSpecial)
			{
				EvaluateSpecialLoopCaseForSettingScreen();
			}
			else
			{
				UpdateControllerBtnIndex_Y(-1, ignoreUpdateControllerBtn: true);
				for (int num2 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count - 1; num2 >= 0; num2--)
				{
					ControllerButton controllerButton2 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[num2];
					if (!controllerButton2)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
					}
					else if (controllerButton2.IsActive())
					{
						m_CurrentCtrlBtnXIndex = num2;
						break;
					}
				}
			}
		}
		else if (m_CurrentCtrlBtnXIndex >= m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count)
		{
			if (m_LoopWithinRowOnly)
			{
				for (int i = 0; i < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; i++)
				{
					ControllerButton controllerButton3 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[i];
					if (!controllerButton3)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
					}
					else if (controllerButton3.IsActive())
					{
						m_CurrentCtrlBtnXIndex = i;
						break;
					}
				}
			}
			else if (m_SettingScreenLoopSpecial)
			{
				EvaluateSpecialLoopCaseForSettingScreen();
			}
			else
			{
				int currentCtrlBtnYIndex = m_CurrentCtrlBtnYIndex;
				UpdateControllerBtnIndex_Y(1, ignoreUpdateControllerBtn: true);
				if (currentCtrlBtnYIndex != m_CurrentCtrlBtnYIndex)
				{
					for (int j = 0; j < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; j++)
					{
						ControllerButton controllerButton4 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[j];
						if (!controllerButton4)
						{
							Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
						}
						else if (controllerButton4.IsActive())
						{
							m_CurrentCtrlBtnXIndex = j;
							break;
						}
					}
				}
			}
		}
		else
		{
			bool flag = true;
			if (addX > 0)
			{
				for (int k = m_CurrentCtrlBtnXIndex; k < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; k++)
				{
					ControllerButton controllerButton5 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[k];
					if (!controllerButton5)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
					}
					else if (controllerButton5.IsActive())
					{
						flag = false;
						m_CurrentCtrlBtnXIndex = k;
						break;
					}
				}
			}
			else
			{
				for (int num3 = m_CurrentCtrlBtnXIndex; num3 >= 0; num3--)
				{
					ControllerButton controllerButton6 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[num3];
					if (!controllerButton6)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
					}
					else if (controllerButton6.IsActive())
					{
						flag = false;
						m_CurrentCtrlBtnXIndex = num3;
						break;
					}
				}
			}
			if (flag)
			{
				if (addX > 0)
				{
					if (m_LoopWithinRowOnly)
					{
						for (int l = 0; l < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; l++)
						{
							ControllerButton controllerButton7 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[l];
							if (!controllerButton7)
							{
								Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
							}
							else if (controllerButton7.IsActive())
							{
								m_CurrentCtrlBtnXIndex = l;
								break;
							}
						}
					}
					else if (m_SettingScreenLoopSpecial)
					{
						EvaluateSpecialLoopCaseForSettingScreen();
					}
					else
					{
						UpdateControllerBtnIndex_Y(1, ignoreUpdateControllerBtn: true);
						for (int m = 0; m < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; m++)
						{
							ControllerButton controllerButton8 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[m];
							if (!controllerButton8)
							{
								Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
							}
							else if (controllerButton8.IsActive())
							{
								m_CurrentCtrlBtnXIndex = m;
								break;
							}
						}
					}
				}
				else if (addX < 0)
				{
					if (m_LoopWithinRowOnly)
					{
						for (int num4 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count - 1; num4 >= 0; num4--)
						{
							ControllerButton controllerButton9 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[num4];
							if (!controllerButton9)
							{
								Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
							}
							else if (controllerButton9.IsActive())
							{
								m_CurrentCtrlBtnXIndex = num4;
								break;
							}
						}
					}
					else if (m_SettingScreenLoopSpecial)
					{
						EvaluateSpecialLoopCaseForSettingScreen();
					}
					else
					{
						m_CurrentCtrlBtnXIndex = 1000;
						UpdateControllerBtnIndex_Y(-1, ignoreUpdateControllerBtn: true);
					}
				}
			}
		}
		if (GetCtrlBtnXChangeMethod(m_CurrentCtrlBtnYIndex) == ECtrlBtnXChangeMethod.IgnoreYIfLessThanTwoActive)
		{
			int num5 = 0;
			for (int n = 0; n < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; n++)
			{
				ControllerButton controllerButton10 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[n];
				if (!controllerButton10)
				{
					Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_X failed");
				}
				else if (controllerButton10.IsActive())
				{
					num5++;
				}
			}
			if (num5 <= 2)
			{
				UpdateControllerBtnIndex_Y(-1);
			}
		}
		if ((bool)m_CurrentControllerButton)
		{
			m_CurrentControllerButton.OnSelectionDeactivate();
		}
		m_CurrentControllerButton = GetControllerButton(m_CurrentCtrlBtnXIndex, m_CurrentCtrlBtnYIndex);
		if ((bool)m_CurrentControllerButton)
		{
			m_CurrentControllerButton.OnSelectionActive();
			if ((bool)m_SliderScreen && m_CurrentControllerButton.m_CanScrollerSlide)
			{
				m_SliderScreen.ScrollToUI(m_CurrentControllerButton.gameObject, instantSnapToPos: false, m_SliderOffsetY);
			}
		}
	}

	private void EvaluateSpecialLoopCaseForSettingScreen()
	{
		if (GetCtrlBtnXChangeMethod(m_CurrentCtrlBtnYIndex) == ECtrlBtnXChangeMethod.SettingScreenSkipSideButtonIndexY)
		{
			m_CurrentCtrlBtnXIndex = 0;
			m_CurrentCtrlBtnYIndex = CSingleton<SettingScreen>.Instance.m_SubSettingBtnHighlight.Count;
			bool flag = false;
			for (int i = m_CurrentCtrlBtnYIndex; i < m_ControllerBtnColumnList.Count; i++)
			{
				for (int j = 0; j < m_ControllerBtnColumnList[i].rowList.Count; j++)
				{
					ControllerButton controllerButton = m_ControllerBtnColumnList[i].rowList[j];
					if (!controllerButton)
					{
						Debug.LogError("[ControllerScreenUIExtension] EvaluateSpecialLoopCaseForSettingScreen failed");
					}
					else if (controllerButton.IsActive())
					{
						flag = true;
						m_CurrentCtrlBtnXIndex = j;
						m_CurrentCtrlBtnYIndex = i;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		else
		{
			m_CurrentCtrlBtnXIndex = 0;
			m_CurrentCtrlBtnYIndex = m_IndexYWhenLoopXEnd;
		}
	}

	public void UpdateControllerBtnIndex_Y(int addY, bool ignoreUpdateControllerBtn = false)
	{
		if (!m_IsControllerUIActive)
		{
			return;
		}
		ECtrlBtnXChangeMethod ctrlBtnXChangeMethod = GetCtrlBtnXChangeMethod(m_CurrentCtrlBtnYIndex);
		if (ctrlBtnXChangeMethod == ECtrlBtnXChangeMethod.RememberIndexX)
		{
			m_LastChangeMethodCtrlBtnXIndex = m_CurrentCtrlBtnXIndex;
		}
		m_CurrentCtrlBtnYIndex += addY;
		if (m_CurrentCtrlBtnYIndex < 0)
		{
			if (m_CanLoopY)
			{
				bool flag = false;
				for (int num = m_ControllerBtnColumnList.Count - 1; num >= 0; num--)
				{
					for (int num2 = m_ControllerBtnColumnList[num].rowList.Count - 1; num2 >= 0; num2--)
					{
						ControllerButton controllerButton = m_ControllerBtnColumnList[num].rowList[num2];
						if (!controllerButton)
						{
							Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
						}
						else if (controllerButton.IsActive())
						{
							flag = true;
							m_CurrentCtrlBtnYIndex = num;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			else
			{
				m_CurrentCtrlBtnYIndex = 0;
			}
		}
		else if (m_CurrentCtrlBtnYIndex >= m_ControllerBtnColumnList.Count)
		{
			if (m_CanLoopY)
			{
				m_CurrentCtrlBtnYIndex = 0;
			}
			else
			{
				m_CurrentCtrlBtnYIndex = m_ControllerBtnColumnList.Count - 1;
			}
		}
		else
		{
			bool flag2 = true;
			if (addY > 0)
			{
				bool flag3 = false;
				for (int i = m_CurrentCtrlBtnYIndex; i < m_ControllerBtnColumnList.Count; i++)
				{
					for (int num3 = m_ControllerBtnColumnList[i].rowList.Count - 1; num3 >= 0; num3--)
					{
						ControllerButton controllerButton2 = m_ControllerBtnColumnList[i].rowList[num3];
						if (!controllerButton2)
						{
							Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
						}
						else if (controllerButton2.IsActive())
						{
							flag3 = true;
							flag2 = false;
							m_CurrentCtrlBtnYIndex = i;
							if (m_CurrentCtrlBtnXIndex > num3)
							{
								m_CurrentCtrlBtnXIndex = num3;
							}
							break;
						}
					}
					if (!flag2)
					{
						break;
					}
				}
				if (!flag3 && m_CanLoopY)
				{
					for (int j = 0; j < m_CurrentCtrlBtnYIndex + 1; j++)
					{
						for (int num4 = m_ControllerBtnColumnList[j].rowList.Count - 1; num4 >= 0; num4--)
						{
							if (m_ControllerBtnColumnList[j].rowList[num4].IsActive())
							{
								flag3 = true;
								flag2 = false;
								m_CurrentCtrlBtnYIndex = j;
								if (m_CurrentCtrlBtnXIndex > num4)
								{
									m_CurrentCtrlBtnXIndex = num4;
								}
								break;
							}
						}
						if (!flag2)
						{
							break;
						}
					}
				}
			}
			else
			{
				bool flag4 = false;
				for (int num5 = m_CurrentCtrlBtnYIndex; num5 >= 0; num5--)
				{
					for (int num6 = m_ControllerBtnColumnList[num5].rowList.Count - 1; num6 >= 0; num6--)
					{
						ControllerButton controllerButton3 = m_ControllerBtnColumnList[num5].rowList[num6];
						if (!controllerButton3)
						{
							Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
						}
						else if (controllerButton3.IsActive())
						{
							flag4 = true;
							flag2 = false;
							m_CurrentCtrlBtnYIndex = num5;
							if (m_CurrentCtrlBtnXIndex > num6)
							{
								m_CurrentCtrlBtnXIndex = num6;
							}
							break;
						}
					}
					if (!flag2)
					{
						break;
					}
				}
				if (!flag4 && m_CanLoopY)
				{
					for (int num7 = m_ControllerBtnColumnList.Count - 1; num7 >= m_CurrentCtrlBtnYIndex; num7--)
					{
						for (int num8 = m_ControllerBtnColumnList[num7].rowList.Count - 1; num8 >= 0; num8--)
						{
							if (m_ControllerBtnColumnList[num7].rowList[num8].IsActive())
							{
								flag4 = true;
								flag2 = false;
								m_CurrentCtrlBtnYIndex = num7;
								if (m_CurrentCtrlBtnXIndex > num8)
								{
									m_CurrentCtrlBtnXIndex = num8;
								}
								break;
							}
						}
						if (!flag2)
						{
							break;
						}
					}
				}
			}
			if (flag2)
			{
				if (m_CanLoopY)
				{
					m_CurrentCtrlBtnYIndex = 0;
				}
				else
				{
					m_CurrentCtrlBtnYIndex--;
				}
			}
		}
		if (m_CurrentCtrlBtnYIndex < 0)
		{
			m_CurrentCtrlBtnYIndex = 0;
		}
		if (m_CurrentCtrlBtnYIndex < m_ControllerBtnColumnList.Count && m_CurrentCtrlBtnXIndex >= m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count)
		{
			m_CurrentCtrlBtnXIndex = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count - 1;
		}
		if (m_CurrentCtrlBtnYIndex < m_ControllerBtnColumnList.Count && m_CurrentCtrlBtnXIndex > m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count - m_CurrentCtrlBtnXIndex)
		{
			for (int k = m_CurrentCtrlBtnXIndex; k < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; k++)
			{
				ControllerButton controllerButton4 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[k];
				if (!controllerButton4)
				{
					Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
				}
				else if (controllerButton4.IsActive())
				{
					m_CurrentCtrlBtnXIndex = k;
					break;
				}
			}
		}
		else
		{
			for (int num9 = m_CurrentCtrlBtnXIndex; num9 >= 0; num9--)
			{
				ControllerButton controllerButton5 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[num9];
				if (!controllerButton5)
				{
					Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
				}
				else if (m_CurrentCtrlBtnYIndex < m_ControllerBtnColumnList.Count && controllerButton5.IsActive())
				{
					m_CurrentCtrlBtnXIndex = num9;
					break;
				}
			}
		}
		ctrlBtnXChangeMethod = GetCtrlBtnXChangeMethod(m_CurrentCtrlBtnYIndex);
		if (ctrlBtnXChangeMethod == ECtrlBtnXChangeMethod.RememberIndexX)
		{
			m_CurrentCtrlBtnXIndex = m_LastChangeMethodCtrlBtnXIndex;
		}
		else if (ctrlBtnXChangeMethod == ECtrlBtnXChangeMethod.AlwaysZeroifGoDown && addY > 0)
		{
			for (int l = 0; l < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; l++)
			{
				ControllerButton controllerButton6 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[l];
				if (!controllerButton6)
				{
					Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
				}
				else if (controllerButton6.IsActive())
				{
					m_CurrentCtrlBtnXIndex = l;
					break;
				}
			}
		}
		else
		{
			switch (ctrlBtnXChangeMethod)
			{
			case ECtrlBtnXChangeMethod.AlwaysGoIndexYOne:
				m_CurrentCtrlBtnYIndex = 1;
				break;
			case ECtrlBtnXChangeMethod.AlwaysIndexXZero:
				m_CurrentCtrlBtnXIndex = 0;
				break;
			case ECtrlBtnXChangeMethod.IgnoreYIfLessThanTwoActive:
			{
				int num10 = 0;
				for (int m = 0; m < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; m++)
				{
					ControllerButton controllerButton7 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[m];
					if (!controllerButton7)
					{
						Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
					}
					else if (controllerButton7.IsActive())
					{
						num10++;
					}
				}
				if (num10 <= 2)
				{
					UpdateControllerBtnIndex_Y(addY, ignoreUpdateControllerBtn);
				}
				break;
			}
			}
		}
		if (m_CurrentCtrlBtnXIndex < 0)
		{
			m_CurrentCtrlBtnXIndex = 0;
		}
		if (m_CurrentCtrlBtnYIndex < 0)
		{
			m_CurrentCtrlBtnYIndex = 0;
		}
		if (m_CurrentCtrlBtnYIndex > m_ControllerBtnColumnList.Count || m_CurrentCtrlBtnXIndex > m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count)
		{
			return;
		}
		ControllerButton controllerButton8 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[m_CurrentCtrlBtnXIndex];
		if (!controllerButton8)
		{
			Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
		}
		if ((bool)controllerButton8 && !controllerButton8.IsActive())
		{
			for (int n = 0; n < m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList.Count; n++)
			{
				ControllerButton controllerButton9 = m_ControllerBtnColumnList[m_CurrentCtrlBtnYIndex].rowList[n];
				if (!controllerButton9)
				{
					Debug.LogError("[ControllerScreenUIExtension] UpdateControllerBtnIndex_Y failed");
				}
				else if (controllerButton9.IsActive())
				{
					m_CurrentCtrlBtnXIndex = n;
					break;
				}
			}
		}
		if (!ignoreUpdateControllerBtn)
		{
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.OnSelectionDeactivate();
			}
			m_CurrentControllerButton = GetControllerButton(m_CurrentCtrlBtnXIndex, m_CurrentCtrlBtnYIndex);
			m_CurrentControllerButton.OnSelectionActive();
			if ((bool)m_SliderScreen && m_CurrentControllerButton.m_CanScrollerSlide)
			{
				m_SliderScreen.ScrollToUI(m_CurrentControllerButton.gameObject, instantSnapToPos: false, m_SliderOffsetY);
			}
		}
	}

	public void EvaluateCtrlBtnActiveChanged()
	{
		UpdateControllerBtnIndex_Y(-1);
	}

	public void OnPressLeft()
	{
		if (m_IsControllerUIActive)
		{
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.OnPressLeft();
				m_CurrentControllerButton.DropdownOnPressUp();
			}
			if (!CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickHorizontal)
			{
				SoundManager.GenericPop(0.5f);
				UpdateControllerBtnIndex_X(-1);
			}
		}
	}

	public void OnPressRight()
	{
		if (m_IsControllerUIActive)
		{
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.OnPressRight();
				m_CurrentControllerButton.DropdownOnPressDown();
			}
			if (!CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickHorizontal)
			{
				SoundManager.GenericPop(0.5f);
				UpdateControllerBtnIndex_X(1);
			}
		}
	}

	public void OnPressUp()
	{
		if (m_IsControllerUIActive)
		{
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.DropdownOnPressUp();
			}
			if (!CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickVertical)
			{
				SoundManager.GenericPop(0.5f);
				UpdateControllerBtnIndex_Y(-1);
			}
		}
	}

	public void OnPressDown()
	{
		if (m_IsControllerUIActive)
		{
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.DropdownOnPressDown();
			}
			if (!CSingleton<ControllerScreenUIExtManager>.Instance.m_LockLJoystickVertical)
			{
				SoundManager.GenericPop(0.5f);
				UpdateControllerBtnIndex_Y(1);
			}
		}
	}

	public void OnPressConfirm()
	{
		if (m_IsControllerUIActive)
		{
			SoundManager.GenericConfirm(0.5f);
			if ((bool)m_CurrentControllerButton)
			{
				m_CurrentControllerButton.OnPressConfirm();
			}
		}
	}

	public void OnPressCancel()
	{
		if (m_IsControllerUIActive && (bool)m_CloseScreenBtn)
		{
			SoundManager.GenericCancel(0.5f);
			if ((bool)m_CurrentControllerButton && !m_CurrentControllerButton.OnPressCancel())
			{
				m_CloseScreenBtn.onClick.Invoke();
			}
		}
	}

	private ECtrlBtnXChangeMethod GetCtrlBtnXChangeMethod(int columnIndex)
	{
		if (columnIndex >= m_CtrlBtnXChangeMethodList.Count)
		{
			return ECtrlBtnXChangeMethod.Normal;
		}
		return m_CtrlBtnXChangeMethodList[columnIndex];
	}

	public void ShowCurrentControllerButtonOverlayHighlight()
	{
		if ((bool)m_CurrentControllerButton)
		{
			for (int i = 0; i < m_CurrentControllerButton.m_OverlayButtonHighlight.Count; i++)
			{
				m_CurrentControllerButton.m_OverlayButtonHighlight[i].gameObject.SetActive(value: true);
			}
		}
	}

	public void HideCurrentControllerButtonOverlayHighlight()
	{
		if ((bool)m_CurrentControllerButton)
		{
			for (int i = 0; i < m_CurrentControllerButton.m_OverlayButtonHighlight.Count; i++)
			{
				m_CurrentControllerButton.m_OverlayButtonHighlight[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void SetControllerUIActive(bool isActive)
	{
		m_IsControllerUIActive = isActive;
	}
}
