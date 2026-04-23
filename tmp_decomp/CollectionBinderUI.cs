using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectionBinderUI : MonoBehaviour
{
	public ControllerScreenUIExtension m_SortAlbumScreenUIExtension;

	public ControllerScreenUIExtension m_GhostCardTutorialScreenUIExtension;

	public CollectionBinderFlipAnimCtrl m_CollectionAlbum;

	public GameObject m_ScreenGrp;

	public GameObject m_SortAlbumScreen;

	public GameObject m_SortAlbumHighlightBtn;

	public GameObject m_ExpansionSelectScreen;

	public GameObject m_ExpansionSelectHighlightBtn;

	public GameObject m_GhostCardTutorialScreen;

	public List<Transform> m_SortAlbumBtnList;

	public List<Transform> m_ExpansionBtnList;

	public Transform m_GradedCardBtn;

	public TextMeshProUGUI m_CardCollectedText;

	public TextMeshProUGUI m_PageText;

	public TextMeshProUGUI m_CardNameText;

	public TextMeshProUGUI m_CardFullRarityNameText;

	public TextMeshProUGUI m_TotalValueText;

	public Animation m_CardNameFoilAnim;

	public Animation m_CardFullRarityNameFoilAnim;

	private int m_MaxPage;

	private int m_MaxCardCollectionCount;

	private void Awake()
	{
		m_ScreenGrp.SetActive(value: false);
		m_SortAlbumScreen.SetActive(value: false);
		m_CardNameText.gameObject.SetActive(value: false);
		m_CardFullRarityNameText.gameObject.SetActive(value: false);
	}

	public void OpenScreen()
	{
		m_ScreenGrp.SetActive(value: true);
	}

	public void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
	}

	public void SetMaxPage(int maxPage)
	{
		m_MaxPage = maxPage;
	}

	public void SetCardCollected(int current, ECardExpansionType expansionType)
	{
		if (expansionType == ECardExpansionType.None)
		{
			m_CardCollectedText.text = current.ToString();
		}
		else
		{
			m_CardCollectedText.text = current + " / " + m_MaxCardCollectionCount;
		}
	}

	public void SetMaxCardCollectCount(int maxCardCollectionCount)
	{
		m_MaxCardCollectionCount = maxCardCollectionCount;
	}

	public void SetTotalValue(float totalValue)
	{
		m_TotalValueText.text = GameInstance.GetPriceString(totalValue);
	}

	public void SetCurrentPage(int pageIndex)
	{
		m_PageText.text = pageIndex + " / " + m_MaxPage;
	}

	public void OpenExpansionSelectScreen(int currentExpansionIndex, bool isGradedCardAlbum)
	{
		if (m_ExpansionSelectScreen.activeSelf)
		{
			CloseExpansionSelectScreen();
			return;
		}
		InteractionPlayerController.TempHideToolTip();
		InteractionPlayerController.AddToolTip(EGameAction.CloseCardAlbum);
		InteractionPlayerController.AddToolTip(EGameAction.BackF);
		CSingleton<InteractionPlayerController>.Instance.ShowCursor();
		SoundManager.GenericMenuOpen();
		if (isGradedCardAlbum)
		{
			m_ExpansionSelectHighlightBtn.transform.position = m_GradedCardBtn.position;
		}
		else
		{
			m_ExpansionSelectHighlightBtn.transform.position = m_ExpansionBtnList[currentExpansionIndex].position;
		}
		m_ExpansionSelectScreen.SetActive(value: true);
	}

	public void CloseExpansionSelectScreen()
	{
		InteractionPlayerController.RestoreHiddenToolTip();
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		SoundManager.GenericMenuClose();
		m_ExpansionSelectScreen.SetActive(value: false);
	}

	public void OpenSortAlbumScreen(int sortingMethodIndex, int currentExpansionIndex, bool isGradedCardAlbum)
	{
		if (m_SortAlbumScreen.activeSelf)
		{
			CloseSortAlbumScreen();
			return;
		}
		InteractionPlayerController.TempHideToolTip();
		InteractionPlayerController.AddToolTip(EGameAction.CloseCardAlbum);
		InteractionPlayerController.AddToolTip(EGameAction.BackF);
		CSingleton<InteractionPlayerController>.Instance.ShowCursor();
		SoundManager.GenericMenuOpen();
		m_SortAlbumHighlightBtn.transform.position = m_SortAlbumBtnList[sortingMethodIndex].position;
		if (isGradedCardAlbum)
		{
			m_ExpansionSelectHighlightBtn.transform.position = m_GradedCardBtn.position;
		}
		else
		{
			m_ExpansionSelectHighlightBtn.transform.position = m_ExpansionBtnList[currentExpansionIndex].position;
		}
		m_SortAlbumScreen.SetActive(value: true);
		ControllerScreenUIExtManager.OnOpenScreen(m_SortAlbumScreenUIExtension);
	}

	public void CloseSortAlbumScreen()
	{
		InteractionPlayerController.RestoreHiddenToolTip();
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		SoundManager.GenericMenuClose();
		m_SortAlbumScreen.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_SortAlbumScreenUIExtension);
	}

	public bool IsSortAlbumScreenOpen()
	{
		return m_SortAlbumScreen.activeSelf;
	}

	public void OnPressSwitchSortingMethod(int sortingMethodIndex)
	{
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		SoundManager.GenericConfirm();
		m_CollectionAlbum.OnPressSwitchSortingMethod(sortingMethodIndex);
		m_SortAlbumScreen.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_SortAlbumScreenUIExtension);
	}

	public void OnPressSwitchExpansion(int expansionIndex)
	{
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		SoundManager.GenericConfirm();
		m_CollectionAlbum.OnPressSwitchExpansion(expansionIndex);
		m_SortAlbumScreen.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_SortAlbumScreenUIExtension);
	}

	public void OnPressSwitchToGradedAlbum()
	{
		CSingleton<InteractionPlayerController>.Instance.HideCursor();
		SoundManager.GenericConfirm();
		m_CollectionAlbum.OnPressSwitchToGradedAlbum();
		m_SortAlbumScreen.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_SortAlbumScreenUIExtension);
	}

	public void OpenGhostCardTutorialScreen()
	{
		SoundManager.GenericMenuOpen();
		CSingleton<InteractionPlayerController>.Instance.EnterUIMode();
		m_GhostCardTutorialScreen.SetActive(value: true);
		ControllerScreenUIExtManager.OnOpenScreen(m_GhostCardTutorialScreenUIExtension);
	}

	public void CloseGhostCardTutorialScreen()
	{
		SoundManager.GenericConfirm();
		CSingleton<InteractionPlayerController>.Instance.ExitUIMode();
		m_GhostCardTutorialScreen.SetActive(value: false);
		ControllerScreenUIExtManager.OnCloseScreen(m_GhostCardTutorialScreenUIExtension);
		CPlayerData.m_HasGetGhostCard = true;
	}
}
