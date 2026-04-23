using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 4)]
public class StockItemData_ScriptableObject : ScriptableObject
{
	public List<EItemType> m_ShownAllItemType;

	public List<EItemType> m_ShownItemType;

	public List<EItemType> m_ShownAccessoryItemType;

	public List<EItemType> m_ShownFigurineItemType;

	public List<EItemType> m_ShownBoardGameItemType;

	public List<EItemType> m_CardPackItemTypeList;

	public List<ECollectionPackType> m_ShownCollectionPackType;

	public List<ECardExpansionType> m_ShownCardExpansionType;

	public List<ItemData> m_ItemDataList;

	public List<ItemMeshData> m_ItemMeshDataList;

	public List<RestockData> m_RestockDataList;

	public List<GameEventData> m_GameEventDataList;

	public List<CollectionPackImageSprite> m_CollectionPackImageSpriteList;

	public float m_ShelfUnlockCostMultiplier;

	public float m_ShelfUpgradeCostMultiplier;

	public List<EQuestType> m_IdealQuestSequenceList;

	public List<Sprite> m_AncientArtifactSpriteList;
}
