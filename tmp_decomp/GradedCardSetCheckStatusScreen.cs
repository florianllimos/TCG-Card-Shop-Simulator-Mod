using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine.UI;

public class GradedCardSetCheckStatusScreen : UIScreenBase
{
	public List<GradeCardPanelUI> m_GradeCardPanelUIList;

	public TextMeshProUGUI m_SetNameText;

	public TextMeshProUGUI m_DeliveryDaysText;

	public TextMeshProUGUI m_PageText;

	public Button m_NextButton;

	public Button m_PreviousButton;

	private int m_PageIndex;

	private int m_PageMaxIndex;

	protected override void Start()
	{
		for (int i = 0; i < m_GradeCardPanelUIList.Count; i++)
		{
			m_GradeCardPanelUIList[i].UpdateCardUI(null);
		}
		base.Start();
	}

	public void UpdateCurrentSetIndex(int index)
	{
		m_PageIndex = index;
	}

	protected override void OnOpenScreen()
	{
		m_PageMaxIndex = CPlayerData.m_GradeCardInProgressList.Count - 1;
		UpdateSetUI(m_PageIndex);
		base.OnOpenScreen();
	}

	public void UpdateSetUI(int setSlotIndex)
	{
		if (CPlayerData.m_GradeCardInProgressList.Count > setSlotIndex)
		{
			GradeCardServiceData gradeCardServiceData = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.GetGradeCardServiceData(CPlayerData.m_GradeCardInProgressList[setSlotIndex].m_ServiceLevel);
			string translation = LocalizationManager.GetTranslation("XXX Set YYY");
			translation = translation.Replace("XXX", gradeCardServiceData.GetServiceName());
			translation = translation.Replace("YYY", (setSlotIndex + 1).ToString());
			m_SetNameText.text = translation;
			int num = gradeCardServiceData.m_ServiceDays - CPlayerData.m_GradeCardInProgressList[setSlotIndex].m_DayPassed;
			if (num > 1)
			{
				m_DeliveryDaysText.text = LocalizationManager.GetTranslation("Delivery in XXX days").Replace("XXX", num.ToString());
			}
			else
			{
				m_DeliveryDaysText.text = LocalizationManager.GetTranslation("Delivery in XXX day").Replace("XXX", num.ToString());
			}
			for (int i = 0; i < CPlayerData.m_GradeCardInProgressList[setSlotIndex].m_CardDataList.Count; i++)
			{
				m_GradeCardPanelUIList[i].UpdateCardUI(CPlayerData.m_GradeCardInProgressList[setSlotIndex].m_CardDataList[i]);
			}
		}
		m_PageText.text = setSlotIndex + 1 + " / " + (m_PageMaxIndex + 1);
		m_NextButton.interactable = m_PageIndex < m_PageMaxIndex;
		m_PreviousButton.interactable = m_PageIndex > 0;
	}

	public void OnPressNextPage()
	{
		if (m_PageIndex < m_PageMaxIndex)
		{
			m_PageIndex++;
			UpdateSetUI(m_PageIndex);
		}
	}

	public void OnPressPreviousPage()
	{
		if (m_PageIndex > 0)
		{
			m_PageIndex--;
			UpdateSetUI(m_PageIndex);
		}
	}
}
