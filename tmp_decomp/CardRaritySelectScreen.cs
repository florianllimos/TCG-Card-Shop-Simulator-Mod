using System.Collections.Generic;
using UnityEngine;

public class CardRaritySelectScreen : CSingleton<CardRaritySelectScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public List<GameObject> m_BtnHighlightList;

	private int m_CurrentIndex;

	public static void OpenScreen(ERarity initCardRarity)
	{
		CSingleton<CardRaritySelectScreen>.Instance.m_CurrentIndex = (int)(initCardRarity + 1);
		for (int i = 0; i < CSingleton<CardRaritySelectScreen>.Instance.m_BtnHighlightList.Count; i++)
		{
			CSingleton<CardRaritySelectScreen>.Instance.m_BtnHighlightList[i].SetActive(value: false);
		}
		CSingleton<CardRaritySelectScreen>.Instance.m_BtnHighlightList[CSingleton<CardRaritySelectScreen>.Instance.m_CurrentIndex].SetActive(value: true);
		SoundManager.GenericMenuOpen();
		CSingleton<CardRaritySelectScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<CardRaritySelectScreen>.Instance.m_ControllerScreenUIExtension);
	}

	private void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<CardRaritySelectScreen>.Instance.m_ControllerScreenUIExtension);
	}

	public void OnPressButton(int index)
	{
		m_CurrentIndex = index + 1;
		CEventManager.QueueEvent(new CEventPlayer_OnCardRaritySelectScreenUpdated(index));
		CloseScreen();
		SoundManager.GenericConfirm();
	}

	public void OnPressBackButton()
	{
		CEventManager.QueueEvent(new CEventPlayer_OnCardRaritySelectScreenUpdated(m_CurrentIndex - 1));
		CloseScreen();
		SoundManager.GenericMenuClose();
	}
}
