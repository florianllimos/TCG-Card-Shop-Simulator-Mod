using TMPro;
using UnityEngine;

public class UI_CreditCardScreen : MonoBehaviour
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public FollowObject m_FollowObject;

	public TextMeshProUGUI m_TotalPriceText;

	private int m_CurrentDecimalPoint;

	private float m_CurrentNumberValue;

	private bool m_HasPressedDecimal;

	private bool m_IsCreditCardMode;

	private InteractableCashierCounter m_CashierCounter;

	private void Awake()
	{
		ResetCounter();
	}

	public void Update()
	{
		if (m_IsCreditCardMode)
		{
			if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
			{
				OnPressNumber(1);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
			{
				OnPressNumber(2);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
			{
				OnPressNumber(3);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4))
			{
				OnPressNumber(4);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha5) || Input.GetKeyUp(KeyCode.Keypad5))
			{
				OnPressNumber(5);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha6) || Input.GetKeyUp(KeyCode.Keypad6))
			{
				OnPressNumber(6);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha7) || Input.GetKeyUp(KeyCode.Keypad7))
			{
				OnPressNumber(7);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha8) || Input.GetKeyUp(KeyCode.Keypad8))
			{
				OnPressNumber(8);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha9) || Input.GetKeyUp(KeyCode.Keypad9))
			{
				OnPressNumber(9);
			}
			else if (Input.GetKeyUp(KeyCode.Alpha0) || Input.GetKeyUp(KeyCode.Keypad0))
			{
				OnPressNumber(0);
			}
			else if (Input.GetKeyUp(KeyCode.Period) || Input.GetKeyUp(KeyCode.KeypadPeriod) || Input.GetKeyUp(KeyCode.Comma))
			{
				OnPressDecimalBtn();
			}
			else if (Input.GetKeyUp(KeyCode.Backspace))
			{
				OnPressBackBtn();
			}
			else if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return) || InputManager.GetKeyUpAction(EGameAction.DoneCounter))
			{
				OnPressConfirmBtn();
			}
		}
	}

	public void OnPressNumber(int number)
	{
		if (!m_IsCreditCardMode)
		{
			return;
		}
		int num = 1000000;
		if (CSingleton<CGameManager>.Instance.m_CurrencyType == EMoneyCurrencyType.Yen)
		{
			num = 100000000;
		}
		if (m_HasPressedDecimal || !(m_CurrentNumberValue > (float)num))
		{
			if (!m_HasPressedDecimal)
			{
				m_CurrentNumberValue = m_CurrentNumberValue * 10f + (float)number;
			}
			else if (m_CurrentDecimalPoint == 0)
			{
				m_CurrentNumberValue += (float)number * 0.1f;
				m_CurrentDecimalPoint++;
			}
			else if (m_CurrentDecimalPoint == 1)
			{
				m_CurrentNumberValue += (float)number * 0.01f;
				m_CurrentDecimalPoint++;
			}
			m_TotalPriceText.text = GameInstance.GetPriceString(m_CurrentNumberValue / GameInstance.GetCurrencyConversionRate());
			SoundManager.PlayAudio("SFX_CheckoutCardPress", 0.2f);
		}
	}

	public void OnPressDecimalBtn()
	{
		if (m_IsCreditCardMode && GameInstance.GetCurrencyHasDecimal(CSingleton<CGameManager>.Instance.m_CurrencyType))
		{
			if (!m_HasPressedDecimal)
			{
				m_HasPressedDecimal = true;
			}
			SoundManager.PlayAudio("SFX_CheckoutCardPress", 0.2f);
		}
	}

	public void OnPressBackBtn()
	{
		if (m_IsCreditCardMode)
		{
			if (!m_HasPressedDecimal || m_CurrentDecimalPoint == 0)
			{
				m_HasPressedDecimal = false;
				m_CurrentNumberValue = Mathf.FloorToInt(m_CurrentNumberValue / 10f);
			}
			else if (m_CurrentDecimalPoint == 0)
			{
				m_HasPressedDecimal = false;
				m_CurrentDecimalPoint = 0;
			}
			else if (m_CurrentDecimalPoint == 1)
			{
				m_CurrentNumberValue = Mathf.FloorToInt(m_CurrentNumberValue);
				m_CurrentDecimalPoint--;
			}
			else if (m_CurrentDecimalPoint == 2)
			{
				m_CurrentNumberValue = (float)Mathf.FloorToInt(m_CurrentNumberValue * 10f) / 10f;
				m_CurrentDecimalPoint--;
			}
			m_TotalPriceText.text = GameInstance.GetPriceString(m_CurrentNumberValue / GameInstance.GetCurrencyConversionRate());
			SoundManager.PlayAudio("SFX_CheckoutCardPress", 0.2f);
		}
	}

	public void OnPressConfirmBtn()
	{
		if (m_IsCreditCardMode)
		{
			m_CashierCounter.EvaluateCreditCard(m_CurrentNumberValue / GameInstance.GetCurrencyConversionRate());
			SoundManager.PlayAudio("SFX_CheckoutCardPress", 0.2f);
		}
	}

	public void SetCashierCounter(InteractableCashierCounter cashierCounter)
	{
		m_CashierCounter = cashierCounter;
	}

	public void EnableCreditCardMode(bool isPlayer)
	{
		m_IsCreditCardMode = isPlayer;
		if (isPlayer)
		{
			ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
			m_TotalPriceText.text = GameInstance.GetPriceString(m_CurrentNumberValue);
		}
	}

	public void ResetCounter()
	{
		if (m_IsCreditCardMode)
		{
			ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
		}
		m_IsCreditCardMode = false;
		m_HasPressedDecimal = false;
		m_CurrentDecimalPoint = 0;
		m_CurrentNumberValue = 0f;
		m_TotalPriceText.text = GameInstance.GetPriceString(m_CurrentNumberValue);
	}
}
