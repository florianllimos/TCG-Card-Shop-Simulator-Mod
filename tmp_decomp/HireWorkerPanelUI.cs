using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HireWorkerPanelUI : UIElementBase
{
	public Image m_IconImage;

	public TextMeshProUGUI m_NameText;

	public TextMeshProUGUI m_RestockSpeedText;

	public TextMeshProUGUI m_CheckoutSpeedText;

	public TextMeshProUGUI m_SalaryCostText;

	public TextMeshProUGUI m_HireFeeText;

	public TextMeshProUGUI m_LevelRequirementText;

	public TextMeshProUGUI m_Description;

	private string m_LevelRequirementString = "";

	public GameObject m_HiredText;

	public GameObject m_PurchaseBtn;

	public GameObject m_LockPurchaseBtn;

	public GameObject m_PrologueUIGrp;

	private HireWorkerScreen m_HireWorkerScreen;

	private bool m_IsHired;

	private int m_Index;

	private int m_LevelRequired;

	private float m_TotalHireFee;

	public void Init(HireWorkerScreen hireWorkerScreen, int index)
	{
		m_HireWorkerScreen = hireWorkerScreen;
		m_Index = index;
		WorkerData workerData = WorkerManager.GetWorkerData(index);
		m_IconImage.sprite = workerData.icon;
		m_NameText.text = workerData.GetName();
		m_RestockSpeedText.text = workerData.GetRestockSpeedText();
		m_CheckoutSpeedText.text = workerData.GetCheckoutSpeedText();
		m_SalaryCostText.text = workerData.GetSalaryCostText();
		m_Description.text = workerData.GetDescription();
		m_TotalHireFee = workerData.hiringCost;
		m_LevelRequired = workerData.shopLevelRequired;
		m_HireFeeText.text = GameInstance.GetPriceString(m_TotalHireFee);
		if (m_LevelRequirementString == "")
		{
			m_LevelRequirementString = m_LevelRequirementText.text;
		}
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			m_LevelRequirementText.gameObject.SetActive(value: false);
			m_HireFeeText.gameObject.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: false);
		}
		else
		{
			m_LevelRequirementText.text = LocalizationManager.GetTranslation(m_LevelRequirementString).Replace("XXX", m_LevelRequired.ToString());
			m_LevelRequirementText.gameObject.SetActive(value: true);
			m_HireFeeText.gameObject.SetActive(value: false);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
		}
		EvaluateHired();
		if (CSingleton<CGameManager>.Instance.m_IsPrologue && !workerData.prologueShow)
		{
			m_PrologueUIGrp.SetActive(value: true);
			m_LockPurchaseBtn.gameObject.SetActive(value: true);
		}
		else
		{
			m_PrologueUIGrp.SetActive(value: false);
		}
	}

	private void EvaluateHired()
	{
		m_IsHired = CPlayerData.GetIsWorkerHired(m_Index);
		if (m_IsHired)
		{
			m_HiredText.SetActive(value: true);
			m_PurchaseBtn.SetActive(value: false);
			m_HireFeeText.gameObject.SetActive(value: false);
		}
		else
		{
			m_HiredText.SetActive(value: false);
			m_PurchaseBtn.SetActive(value: true);
		}
	}

	public void OnPressHireButton()
	{
		if (m_IsHired)
		{
			return;
		}
		if (CPlayerData.m_ShopLevel + 1 >= m_LevelRequired)
		{
			if (CPlayerData.m_CoinAmountDouble >= (double)m_TotalHireFee)
			{
				PriceChangeManager.AddTransaction(0f - m_TotalHireFee, ETransactionType.HireWorker, m_Index);
				CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_TotalHireFee));
				CPlayerData.SetIsWorkerHired(m_Index, isHired: true);
				CSingleton<WorkerManager>.Instance.ActivateWorker(m_Index, resetTask: true);
				CPlayerData.m_GameReportDataCollect.employeeCost -= m_TotalHireFee;
				CPlayerData.m_GameReportDataCollectPermanent.employeeCost -= m_TotalHireFee;
				int num = 0;
				for (int i = 0; i < CPlayerData.m_IsWorkerHired.Count; i++)
				{
					if (CPlayerData.m_IsWorkerHired[i])
					{
						num++;
					}
				}
				AchievementManager.OnStaffHired(num);
				SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
				EvaluateHired();
			}
			else
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
			}
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.ShopLevelNotEnough);
		}
	}
}
