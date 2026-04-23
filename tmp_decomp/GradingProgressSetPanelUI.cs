using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GradingProgressSetPanelUI : UIElementBase
{
	public GameObject m_SetInfoGrp;

	public TextMeshProUGUI m_SlotAvailableText;

	public TextMeshProUGUI m_SetNameText;

	public TextMeshProUGUI m_DaysLeftText;

	public Image m_BarImage;

	private GradeCardWebsiteUIScreen m_GradeCardWebsiteUIScreen;

	private int m_Index;

	public void Init(GradeCardWebsiteUIScreen gradeCardWebsiteUIScreen, int index)
	{
		m_GradeCardWebsiteUIScreen = gradeCardWebsiteUIScreen;
		m_Index = index;
	}

	public override void OnPressButton()
	{
		if ((bool)m_GradeCardWebsiteUIScreen)
		{
			m_GradeCardWebsiteUIScreen.OnPressSelectSetSlotIndex(m_Index);
		}
	}

	public void UpdateSetUI()
	{
		if (CPlayerData.m_GradeCardInProgressList.Count > m_Index)
		{
			GradeCardServiceData gradeCardServiceData = CSingleton<InventoryBase>.Instance.m_MonsterData_SO.GetGradeCardServiceData(CPlayerData.m_GradeCardInProgressList[m_Index].m_ServiceLevel);
			m_SetNameText.text = gradeCardServiceData.GetServiceName();
			m_DaysLeftText.text = gradeCardServiceData.GetServiceDayLeftString(CPlayerData.m_GradeCardInProgressList[m_Index].m_DayPassed);
			m_BarImage.fillAmount = (float)CPlayerData.m_GradeCardInProgressList[m_Index].m_DayPassed / (float)gradeCardServiceData.m_ServiceDays;
			m_SetInfoGrp.SetActive(value: true);
			m_SlotAvailableText.enabled = false;
			m_SetNameText.enabled = true;
		}
		else
		{
			m_SetInfoGrp.SetActive(value: false);
			m_SlotAvailableText.enabled = true;
			m_SetNameText.enabled = false;
		}
	}
}
