using System.Collections.Generic;
using UnityEngine;

public class BinderPageGrp : MonoBehaviour
{
	public Animator m_Anim;

	public GameObject m_Mesh;

	public GameObject m_Mesh2;

	public GameObject m_Joint;

	public List<Card3dUIGroup> m_CardList;

	public List<Transform> m_CardLocList;

	private List<Vector3> m_CardOriginalPosList = new List<Vector3>();

	private bool m_IsGradedCardMode;

	public void SetVisibility(bool isVisible)
	{
		m_Mesh.SetActive(isVisible);
		m_Mesh2.SetActive(isVisible);
		m_Joint.SetActive(isVisible);
	}

	public void SetGradedCardMode(bool isGradedCardMode)
	{
		m_IsGradedCardMode = isGradedCardMode;
		if (m_CardOriginalPosList.Count < m_CardList.Count)
		{
			m_CardOriginalPosList.Clear();
			for (int i = 0; i < m_CardList.Count; i++)
			{
				m_CardOriginalPosList.Add(m_CardList[i].transform.localPosition);
			}
		}
		if (isGradedCardMode)
		{
			for (int j = 0; j < m_CardLocList.Count; j++)
			{
				Vector3 localPosition = m_CardOriginalPosList[j];
				localPosition.y -= 0.001f;
				localPosition.z += 0.02f;
				m_CardList[j].transform.localPosition = localPosition;
				m_CardList[j].CheckValidCard3dUI();
				m_CardLocList[j].localScale = Vector3.one * 0.75f;
			}
		}
		else
		{
			for (int k = 0; k < m_CardLocList.Count; k++)
			{
				Vector3 localPosition2 = m_CardOriginalPosList[k];
				m_CardList[k].transform.localPosition = localPosition2;
				m_CardList[k].CheckValidCard3dUI();
				m_CardLocList[k].localScale = Vector3.one;
			}
		}
	}

	public void SetCard(int index, ECardExpansionType cardExpansionType, bool isDimensionCard)
	{
		if (index <= 0)
		{
			return;
		}
		for (int i = 0; i < m_CardList.Count; i++)
		{
			int num = (index - 1) * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType) + i;
			int cardAmountByIndex = CPlayerData.GetCardAmountByIndex(num, cardExpansionType, isDimensionCard);
			if (cardAmountByIndex <= 0)
			{
				m_CardList[i].SetVisibility(isVisible: false);
				continue;
			}
			bool isDestiny = false;
			if (cardExpansionType == ECardExpansionType.Ghost && num >= InventoryBase.GetShownMonsterList(cardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType))
			{
				isDestiny = true;
				num -= InventoryBase.GetShownMonsterList(cardExpansionType).Count * CPlayerData.GetCardAmountPerMonsterType(cardExpansionType);
			}
			CardData cardData = new CardData();
			ECardBorderType borderType = (ECardBorderType)(i % CPlayerData.GetCardAmountPerMonsterType(cardExpansionType, includeFoilCount: false));
			cardData.monsterType = CPlayerData.GetMonsterTypeFromCardSaveIndex(num, cardExpansionType);
			cardData.borderType = borderType;
			cardData.expansionType = cardExpansionType;
			cardData.isDestiny = isDestiny;
			cardData.isNew = false;
			cardData.isFoil = i >= CPlayerData.GetCardAmountPerMonsterType(cardExpansionType, includeFoilCount: false);
			m_CardList[i].m_CardUI.SetCardUI(cardData);
			m_CardList[i].SetVisibility(isVisible: true);
			m_CardList[i].SetCardCountText(cardAmountByIndex, showDuplicate: false);
			m_CardList[i].SetCardCountTextVisibility(isVisible: true);
		}
	}

	public void SetSingleCard(int cardIndex, CardData cardData, int cardCount, ECollectionSortingType sortingType)
	{
		if (sortingType == ECollectionSortingType.DuplicatePrice && !m_IsGradedCardMode)
		{
			cardCount--;
		}
		if (cardCount <= 0)
		{
			m_CardList[cardIndex].SetVisibility(isVisible: false);
			return;
		}
		m_CardList[cardIndex].m_CardUI.SetCardUI(cardData);
		m_CardList[cardIndex].SetVisibility(isVisible: true);
		m_CardList[cardIndex].SetCardCountText(cardCount, sortingType == ECollectionSortingType.DuplicatePrice);
		if (cardData.cardGrade > 0)
		{
			m_CardList[cardIndex].SetCardCountTextVisibility(isVisible: false);
		}
		else
		{
			m_CardList[cardIndex].SetCardCountTextVisibility(isVisible: true);
		}
	}
}
