using System.Collections.Generic;
using UnityEngine;

public class CardExpansionSelectScreen : CSingleton<CardExpansionSelectScreen>
{
	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	public List<GameObject> m_BtnHighlightList;

	private int m_CurrentIndex;

	public static void OpenScreen(ECardExpansionType initCardExpansion)
	{
		CSingleton<CardExpansionSelectScreen>.Instance.m_CurrentIndex = (int)initCardExpansion;
		for (int i = 0; i < CSingleton<CardExpansionSelectScreen>.Instance.m_BtnHighlightList.Count; i++)
		{
			CSingleton<CardExpansionSelectScreen>.Instance.m_BtnHighlightList[i].SetActive(value: false);
		}
		CSingleton<CardExpansionSelectScreen>.Instance.m_BtnHighlightList[CSingleton<CardExpansionSelectScreen>.Instance.m_CurrentIndex].SetActive(value: true);
		SoundManager.GenericMenuOpen();
		CSingleton<CardExpansionSelectScreen>.Instance.m_ScreenGrp.SetActive(value: true);
		ControllerScreenUIExtManager.OnOpenScreen(CSingleton<CardExpansionSelectScreen>.Instance.m_ControllerScreenUIExtension);
	}

	private void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(CSingleton<CardExpansionSelectScreen>.Instance.m_ControllerScreenUIExtension);
	}

	public void OnPressButton(int index)
	{
		m_CurrentIndex = index;
		CEventManager.QueueEvent(new CEventPlayer_OnCardExpansionSelectScreenUpdated(index));
		CloseScreen();
		SoundManager.GenericConfirm();
	}

	public void OnPressBackButton()
	{
		CEventManager.QueueEvent(new CEventPlayer_OnCardExpansionSelectScreenUpdated(m_CurrentIndex));
		CloseScreen();
		SoundManager.GenericMenuClose();
	}
}
