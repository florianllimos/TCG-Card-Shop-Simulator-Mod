using I2.Loc;
using TMPro;
using UnityEngine;

public class RentBillPanelUI : MonoBehaviour
{
	public TextMeshProUGUI m_DayDueText;

	public TextMeshProUGUI m_AmountDueText;

	public TextMeshProUGUI m_LatePaymentText;

	public GameObject m_LockBtnGrp;

	public GameObject m_TickIcon;

	private EBillType m_BillType;

	private RentBillScreen m_RentBillScreen;

	private int m_DueDay;

	private int m_BillDayPassed;

	private float m_AmountDue;

	public void Init(RentBillScreen rentBillScreen, EBillType billType)
	{
		m_RentBillScreen = rentBillScreen;
		m_BillType = billType;
	}

	public void EvaluateUI()
	{
		BillData bill = CPlayerData.GetBill(m_BillType);
		if (bill == null)
		{
			m_DueDay = 0;
			m_AmountDue = 0f;
			m_BillDayPassed = 0;
		}
		else
		{
			m_DueDay = m_RentBillScreen.GetDueDayMax() - bill.billDayPassed;
			m_AmountDue = bill.amountToPay;
			m_BillDayPassed = bill.billDayPassed;
		}
		if (m_AmountDue <= 0f && m_BillDayPassed == 0)
		{
			m_DayDueText.text = "";
			m_AmountDueText.text = "";
			m_LockBtnGrp.SetActive(value: true);
			m_LatePaymentText.enabled = false;
			m_DayDueText.color = m_RentBillScreen.m_NormalDayColor;
			m_TickIcon.gameObject.SetActive(value: true);
		}
		else
		{
			if (!(m_AmountDue > 0f))
			{
				return;
			}
			m_AmountDueText.text = GameInstance.GetPriceString(m_AmountDue);
			m_LockBtnGrp.SetActive(value: false);
			m_TickIcon.gameObject.SetActive(value: false);
			if (m_DueDay < 0)
			{
				m_DayDueText.color = m_RentBillScreen.m_LateDayColor;
				m_LatePaymentText.enabled = true;
				int num = Mathf.Abs(m_DueDay);
				if (num == 1)
				{
					m_DayDueText.text = LocalizationManager.GetTranslation("Late By XXX Day").Replace("XXX", num.ToString());
				}
				else
				{
					m_DayDueText.text = LocalizationManager.GetTranslation("Late By XXX Days").Replace("XXX", num.ToString());
				}
				return;
			}
			if (m_DueDay < 3)
			{
				m_DayDueText.color = m_RentBillScreen.m_WarningDayColor;
			}
			else
			{
				m_DayDueText.color = m_RentBillScreen.m_NormalDayColor;
			}
			m_LatePaymentText.enabled = false;
			if (m_DueDay == 1)
			{
				m_DayDueText.text = m_DueDay + " " + LocalizationManager.GetTranslation("Day");
			}
			else if (m_DueDay == 0)
			{
				m_DayDueText.text = LocalizationManager.GetTranslation("Today");
			}
			else
			{
				m_DayDueText.text = m_DueDay + " " + LocalizationManager.GetTranslation("Days");
			}
		}
	}

	public void OnPressButton()
	{
		if (m_BillType == EBillType.Rent)
		{
			m_RentBillScreen.OnPressPayRentBill();
		}
		else if (m_BillType == EBillType.Electric)
		{
			m_RentBillScreen.OnPressPayElectricBill();
		}
		else if (m_BillType == EBillType.Employee)
		{
			m_RentBillScreen.OnPressPaySalaryBill();
		}
	}
}
