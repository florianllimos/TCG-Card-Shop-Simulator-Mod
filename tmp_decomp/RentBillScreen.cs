using TMPro;
using UnityEngine;

public class RentBillScreen : UIScreenBase
{
	public RentBillPanelUI m_RentBillUI;

	public RentBillPanelUI m_ElectricBillUI;

	public RentBillPanelUI m_SalaryBillUI;

	public TextMeshProUGUI m_TotalAmountDueText;

	public GameObject m_PayAllLockBtn;

	public Color m_LateDayColor;

	public Color m_WarningDayColor;

	public Color m_NormalDayColor;

	private int m_DueDayMax = 8;

	private float m_TotalAmountToPay;

	public void OnGameFinishLoaded()
	{
		m_RentBillUI.Init(this, EBillType.Rent);
		m_ElectricBillUI.Init(this, EBillType.Electric);
		m_SalaryBillUI.Init(this, EBillType.Employee);
		EvaluateBillNotification();
	}

	private void EvaluateBillNotification()
	{
		BillData bill = CPlayerData.GetBill(EBillType.Rent);
		BillData bill2 = CPlayerData.GetBill(EBillType.Electric);
		BillData bill3 = CPlayerData.GetBill(EBillType.Employee);
		if (bill != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Rent).billDayPassed < 3)
		{
			CSingleton<PhoneManager>.Instance.SetBillNotificationVisible(isVisible: true);
		}
		else if (bill2 != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Electric).billDayPassed < 3)
		{
			CSingleton<PhoneManager>.Instance.SetBillNotificationVisible(isVisible: true);
		}
		else if (bill3 != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Employee).billDayPassed < 3)
		{
			CSingleton<PhoneManager>.Instance.SetBillNotificationVisible(isVisible: true);
		}
		else
		{
			CSingleton<PhoneManager>.Instance.SetBillNotificationVisible(isVisible: false);
		}
	}

	public void EvaluateNewDayBill()
	{
		BillData bill = CPlayerData.GetBill(EBillType.Rent);
		BillData bill2 = CPlayerData.GetBill(EBillType.Electric);
		BillData bill3 = CPlayerData.GetBill(EBillType.Employee);
		bool flag = false;
		if (bill != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Rent).billDayPassed < -5)
		{
			OnPressPayRentBill(forcePay: true);
			flag = true;
		}
		if (bill2 != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Electric).billDayPassed < -5)
		{
			OnPressPayElectricBill(forcePay: true);
			flag = true;
		}
		if (bill3 != null && m_DueDayMax - CPlayerData.GetBill(EBillType.Employee).billDayPassed < -5)
		{
			OnPressPaySalaryBill(forcePay: true);
			flag = true;
		}
		if (flag)
		{
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
		}
		float shopLightOnTime = LightManager.GetShopLightOnTime();
		int cashierCounterCount = ShelfManager.GetCashierCounterCount();
		int unlockRoomCount = CPlayerData.m_UnlockRoomCount;
		int num = CPlayerData.m_UnlockWarehouseRoomCount;
		bool isWarehouseRoomUnlocked = CPlayerData.m_IsWarehouseRoomUnlocked;
		float num2 = 100 + unlockRoomCount * 20;
		num2 = 50f;
		for (int i = 0; i < unlockRoomCount; i++)
		{
			num2 = ((i >= 4) ? ((i >= 4 && i < 8) ? (num2 + 30f) : ((i >= 8 && i < 12) ? (num2 + 40f) : ((i >= 12 && i < 16) ? (num2 + 50f) : ((i >= 16 && i < 20) ? (num2 + 60f) : ((i < 20 || i >= 24) ? (num2 + 100f) : (num2 + 80f)))))) : (num2 + 20f));
		}
		if (isWarehouseRoomUnlocked)
		{
			num2 += (float)(150 + num * 60);
		}
		float num3 = 1f;
		float num4 = 4f;
		if (isWarehouseRoomUnlocked)
		{
			num += 2;
		}
		float num5 = shopLightOnTime * (float)(unlockRoomCount + num) * (num3 / 60f) / 4f;
		float amountToPayChange = (float)cashierCounterCount * num4 + shopLightOnTime * (num3 / 60f) + num5;
		float totalSalaryCost = CSingleton<WorkerManager>.Instance.GetTotalSalaryCost();
		CPlayerData.UpdateBill(EBillType.Rent, 1, num2);
		CPlayerData.UpdateBill(EBillType.Electric, 1, amountToPayChange);
		CPlayerData.UpdateBill(EBillType.Employee, 1, totalSalaryCost);
		LightManager.ResetShopLightOnTime();
		EvaluateBillNotification();
	}

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		EvaluateUI();
	}

	private void EvaluateUI()
	{
		m_RentBillUI.EvaluateUI();
		m_ElectricBillUI.EvaluateUI();
		m_SalaryBillUI.EvaluateUI();
		BillData bill = CPlayerData.GetBill(EBillType.Rent);
		BillData bill2 = CPlayerData.GetBill(EBillType.Electric);
		BillData bill3 = CPlayerData.GetBill(EBillType.Employee);
		m_TotalAmountToPay = 0f;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (bill != null && bill.amountToPay > 0f)
		{
			num = bill.amountToPay;
		}
		if (bill2 != null && bill2.amountToPay > 0f)
		{
			num2 = bill2.amountToPay;
		}
		if (bill3 != null && bill3.amountToPay > 0f)
		{
			num3 = bill3.amountToPay;
		}
		m_TotalAmountToPay = num + num2 + num3;
		if (m_TotalAmountToPay > 0f)
		{
			m_PayAllLockBtn.SetActive(value: false);
		}
		else
		{
			m_PayAllLockBtn.SetActive(value: true);
		}
		m_TotalAmountDueText.text = GameInstance.GetPriceString(m_TotalAmountToPay);
	}

	protected override void OnCloseScreen()
	{
		base.OnCloseScreen();
	}

	public void OnPressPayRentBill(bool forcePay = false)
	{
		BillData bill = CPlayerData.GetBill(EBillType.Rent);
		if (bill == null || !(bill.amountToPay > 0f))
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)bill.amountToPay || forcePay)
		{
			if (!forcePay)
			{
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			}
			PriceChangeManager.AddTransaction(0f - bill.amountToPay, ETransactionType.PayBillRent, 0);
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(bill.amountToPay));
			CPlayerData.m_GameReportDataCollect.rentCost -= bill.amountToPay;
			CPlayerData.m_GameReportDataCollectPermanent.rentCost -= bill.amountToPay;
			CPlayerData.SetBill(EBillType.Rent, 0, 0f);
			EvaluateUI();
			EvaluateBillNotification();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public void OnPressPayElectricBill(bool forcePay = false)
	{
		BillData bill = CPlayerData.GetBill(EBillType.Electric);
		if (bill == null || !(bill.amountToPay > 0f))
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)bill.amountToPay || forcePay)
		{
			if (!forcePay)
			{
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			}
			PriceChangeManager.AddTransaction(0f - bill.amountToPay, ETransactionType.PayBillElectric, 0);
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(bill.amountToPay));
			CPlayerData.m_GameReportDataCollect.billCost -= bill.amountToPay;
			CPlayerData.m_GameReportDataCollectPermanent.billCost -= bill.amountToPay;
			CPlayerData.SetBill(EBillType.Electric, 0, 0f);
			EvaluateUI();
			EvaluateBillNotification();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public void OnPressPaySalaryBill(bool forcePay = false)
	{
		BillData bill = CPlayerData.GetBill(EBillType.Employee);
		if (bill == null || !(bill.amountToPay > 0f))
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)bill.amountToPay || forcePay)
		{
			if (!forcePay)
			{
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			}
			PriceChangeManager.AddTransaction(0f - bill.amountToPay, ETransactionType.WorkerSalary, 0);
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(bill.amountToPay));
			CPlayerData.m_GameReportDataCollect.employeeCost -= bill.amountToPay;
			CPlayerData.m_GameReportDataCollectPermanent.employeeCost -= bill.amountToPay;
			CPlayerData.SetBill(EBillType.Employee, 0, 0f);
			EvaluateUI();
			EvaluateBillNotification();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public void OnPressPayAllBill()
	{
		BillData bill = CPlayerData.GetBill(EBillType.Rent);
		BillData bill2 = CPlayerData.GetBill(EBillType.Electric);
		BillData bill3 = CPlayerData.GetBill(EBillType.Employee);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		if (bill != null && bill.amountToPay > 0f)
		{
			num2 = bill.amountToPay;
		}
		if (bill2 != null && bill2.amountToPay > 0f)
		{
			num3 = bill2.amountToPay;
		}
		if (bill3 != null && bill3.amountToPay > 0f)
		{
			num4 = bill3.amountToPay;
		}
		num = num2 + num3 + num4;
		if (!(num > 0f))
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)num)
		{
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			if (num2 > 0f)
			{
				PriceChangeManager.AddTransaction(0f - num2, ETransactionType.PayBillRent, 0);
			}
			if (num3 > 0f)
			{
				PriceChangeManager.AddTransaction(0f - num3, ETransactionType.PayBillElectric, 0);
			}
			if (num4 > 0f)
			{
				PriceChangeManager.AddTransaction(0f - num4, ETransactionType.WorkerSalary, 0);
			}
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(num));
			CPlayerData.m_GameReportDataCollect.rentCost -= num2;
			CPlayerData.m_GameReportDataCollectPermanent.rentCost -= num2;
			CPlayerData.m_GameReportDataCollect.billCost -= num3;
			CPlayerData.m_GameReportDataCollectPermanent.billCost -= num3;
			CPlayerData.m_GameReportDataCollect.employeeCost -= num4;
			CPlayerData.m_GameReportDataCollectPermanent.employeeCost -= num4;
			CPlayerData.SetBill(EBillType.Rent, 0, 0f);
			CPlayerData.SetBill(EBillType.Electric, 0, 0f);
			CPlayerData.SetBill(EBillType.Employee, 0, 0f);
			EvaluateUI();
			EvaluateBillNotification();
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			SoundManager.GenericCancel();
		}
	}

	public int GetDueDayMax()
	{
		return m_DueDayMax;
	}
}
