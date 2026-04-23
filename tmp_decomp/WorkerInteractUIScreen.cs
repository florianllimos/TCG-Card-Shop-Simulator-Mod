using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerInteractUIScreen : CSingleton<WorkerInteractUIScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public WorkerOptionUIScreen m_WorkerOptionUIScreen;

	public WorkerOptionSetPriceUIScreen m_WorkerOptionSetPriceUIScreen;

	public WorkerSetPrimarySecondaryTaskScreen m_WorkerSetPrimarySecondaryTaskScreen;

	public WorkerSetPackOpenerTypeOptionScreen m_WorkerSetPackOpenerTypeOptionScreen;

	public Animation m_TopTextAnim;

	public TextMeshProUGUI m_TopText;

	public TextMeshProUGUI m_CurrentTaskText;

	public TextMeshProUGUI m_SecondaryTaskText;

	public TextMeshProUGUI m_CheckoutSpeedText;

	public TextMeshProUGUI m_RestockSpeedText;

	public TextMeshProUGUI m_SalaryCostText;

	public TextMeshProUGUI m_BonusCostText;

	public TextMeshProUGUI m_BonusAddAmountText;

	public GameObject m_BonusAddAmountGrp;

	public Button m_GiveBonusBtn;

	public List<WorkerSkillBarPanelUI> m_WorkerSkillBarPanelUIList;

	private string m_DefaultTopText = "What's up boss?";

	private float m_SalaryCost = 1000f;

	private Worker m_Worker;

	private EWorkerTask m_TaskToSet;

	private int m_MaxSalaryBoostedCount = 3;

	public static void OpenScreen(Worker worker)
	{
		CSingleton<WorkerInteractUIScreen>.Instance.m_Worker = worker;
		CSingleton<WorkerInteractUIScreen>.Instance.m_CurrentTaskText.text = WorkerManager.GetTaskName(CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.m_PrimaryTask);
		CSingleton<WorkerInteractUIScreen>.Instance.m_SecondaryTaskText.text = WorkerManager.GetTaskName(CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.m_SecondaryTask);
		CSingleton<WorkerInteractUIScreen>.Instance.m_CheckoutSpeedText.text = CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetWorkerData().GetCheckoutSpeedText();
		CSingleton<WorkerInteractUIScreen>.Instance.m_RestockSpeedText.text = CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetWorkerData().GetRestockSpeedText();
		CSingleton<WorkerInteractUIScreen>.Instance.m_SalaryCostText.text = CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetWorkerData().GetSalaryCostText();
		CSingleton<WorkerInteractUIScreen>.Instance.m_SalaryCost = CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetWorkerData().costPerDay;
		CSingleton<WorkerInteractUIScreen>.Instance.m_BonusCostText.text = GameInstance.GetPriceString(CSingleton<WorkerInteractUIScreen>.Instance.m_SalaryCost);
		CSingleton<WorkerInteractUIScreen>.Instance.m_TopText.text = LocalizationManager.GetTranslation(CSingleton<WorkerInteractUIScreen>.Instance.m_DefaultTopText);
		for (int i = 0; i < CSingleton<WorkerInteractUIScreen>.Instance.m_WorkerSkillBarPanelUIList.Count; i++)
		{
			CSingleton<WorkerInteractUIScreen>.Instance.m_WorkerSkillBarPanelUIList[i].UpdateWorkerSkillBar(CSingleton<WorkerInteractUIScreen>.Instance.m_Worker);
		}
		CSingleton<WorkerInteractUIScreen>.Instance.m_GiveBonusBtn.interactable = CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetBonusBoostedCount() < CSingleton<WorkerInteractUIScreen>.Instance.m_MaxSalaryBoostedCount;
		if (CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetBonusBoostedCount() > 0)
		{
			CSingleton<WorkerInteractUIScreen>.Instance.m_BonusAddAmountText.text = "+" + CSingleton<WorkerInteractUIScreen>.Instance.m_Worker.GetBonusBoostedCount();
			CSingleton<WorkerInteractUIScreen>.Instance.m_BonusAddAmountGrp.SetActive(value: true);
		}
		else
		{
			CSingleton<WorkerInteractUIScreen>.Instance.m_BonusAddAmountGrp.SetActive(value: false);
		}
		CSingleton<WorkerInteractUIScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<WorkerInteractUIScreen>.Instance.m_ControllerScreenUIExtension);
	}

	public void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		SoundManager.GenericMenuClose();
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<WorkerInteractUIScreen>.Instance.m_ControllerScreenUIExtension);
	}

	public void SetTaskAsPrimaryOrSecondary(bool isPrimary)
	{
		m_ScreenGrp.SetActive(value: true);
		EWorkerTask taskToSet = m_TaskToSet;
		m_Worker.SetTaskSettingPrimarySecondary(isPrimary);
		if (m_TaskToSet == EWorkerTask.RestockShelf)
		{
			m_WorkerOptionUIScreen.OpenScreen(m_Worker, (int)taskToSet);
			CloseScreen();
			return;
		}
		if (m_TaskToSet == EWorkerTask.SetPrice)
		{
			m_WorkerOptionSetPriceUIScreen.OpenScreen(m_Worker, m_TaskToSet);
			CloseScreen();
			return;
		}
		if (m_TaskToSet == EWorkerTask.RestockCardDisplay)
		{
			m_WorkerOptionSetPriceUIScreen.OpenScreen(m_Worker, m_TaskToSet);
			CloseScreen();
			return;
		}
		if (m_TaskToSet == EWorkerTask.RefillCardOpener)
		{
			m_WorkerSetPackOpenerTypeOptionScreen.OpenScreen(m_Worker);
			CloseScreen();
			return;
		}
		if (isPrimary)
		{
			m_Worker.SetTask(m_TaskToSet);
			m_Worker.SetLastTask(m_TaskToSet);
		}
		else
		{
			m_Worker.SetSecondaryTask(m_TaskToSet);
		}
		SoundManager.GenericConfirm();
		m_Worker.OnPressStopInteract();
		CloseScreen();
	}

	public void OnPressAssignTask(int taskIndex)
	{
		m_TaskToSet = (EWorkerTask)taskIndex;
		if (m_TaskToSet == EWorkerTask.RestockCardDisplay)
		{
			bool flag = false;
			for (int i = 0; i < ShelfManager.GetCardStorageShelfList().Count; i++)
			{
				if (ShelfManager.GetCardStorageShelfList()[i].IsValidObject())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.WorkerCannotRestockCardWithoutCardStorageShelf);
				return;
			}
		}
		m_WorkerSetPrimarySecondaryTaskScreen.OpenScreen();
		CloseScreen();
	}

	public void OnPressFire()
	{
		m_Worker.FireWorker();
		SoundManager.GenericConfirm();
		m_Worker.OnPressStopInteract();
		CloseScreen();
	}

	public void OnPressGiveBonus()
	{
		if (m_Worker.GetBonusBoostedCount() >= m_MaxSalaryBoostedCount)
		{
			return;
		}
		if (CPlayerData.m_CoinAmountDouble >= (double)m_SalaryCost)
		{
			PriceChangeManager.AddTransaction(0f - m_SalaryCost, ETransactionType.WorkerSalary, 0);
			CEventManager.QueueEvent(new CEventPlayer_ReduceCoin(m_SalaryCost));
			CPlayerData.m_GameReportDataCollect.employeeCost -= m_SalaryCost;
			CPlayerData.m_GameReportDataCollectPermanent.employeeCost -= m_SalaryCost;
			SoundManager.PlayAudio("SFX_CustomerBuy", 0.6f);
			m_Worker.GiveSalaryBonus();
			if (m_Worker.GetBonusBoostedCount() >= m_MaxSalaryBoostedCount)
			{
				m_GiveBonusBtn.interactable = false;
			}
			m_TopText.text = m_Worker.GetWorkerData().GetBonusConversation();
			m_TopTextAnim.Rewind();
			m_TopTextAnim.Play();
			m_BonusAddAmountText.text = "+" + m_Worker.GetBonusBoostedCount();
			m_BonusAddAmountGrp.SetActive(value: true);
		}
		else
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.Money);
		}
	}

	public void OnPressCancel()
	{
		m_Worker.OnPressStopInteract();
		SoundManager.GenericCancel();
		CloseScreen();
	}
}
