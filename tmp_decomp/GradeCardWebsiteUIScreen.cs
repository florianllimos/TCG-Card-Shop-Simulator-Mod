using System.Collections.Generic;

public class GradeCardWebsiteUIScreen : UIScreenBase
{
	public GradedCardSubmitSelectScreen m_GradedCardSubmitSelectScreen;

	public GradedCardSetCheckStatusScreen m_GradedCardSetCheckStatusScreen;

	public List<GradingProgressSetPanelUI> m_GradingProgressSetPanelUIList;

	protected override void Start()
	{
		for (int i = 0; i < m_GradingProgressSetPanelUIList.Count; i++)
		{
			m_GradingProgressSetPanelUIList[i].Init(this, i);
		}
		base.Start();
	}

	protected override void Init()
	{
		base.Init();
	}

	protected override void OnOpenScreen()
	{
		SoundManager.GenericMenuOpen();
		base.OnOpenScreen();
		UpdateSubmissionProgressPanelUI();
	}

	public void UpdateSubmissionProgressPanelUI()
	{
		for (int i = 0; i < m_GradingProgressSetPanelUIList.Count; i++)
		{
			m_GradingProgressSetPanelUIList[i].UpdateSetUI();
		}
	}

	public void OnPressNewSubmissionButton()
	{
		if (CPlayerData.m_GradeCardInProgressList.Count >= 4)
		{
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.GenericNoSlot);
		}
		else
		{
			OpenChildScreen(m_GradedCardSubmitSelectScreen);
		}
	}

	public void OnPressSelectSetSlotIndex(int setSlotIndex)
	{
		if (CPlayerData.m_GradeCardInProgressList.Count > setSlotIndex)
		{
			m_GradedCardSetCheckStatusScreen.UpdateCurrentSetIndex(setSlotIndex);
			OpenChildScreen(m_GradedCardSetCheckStatusScreen);
		}
		else
		{
			OnPressNewSubmissionButton();
		}
	}
}
