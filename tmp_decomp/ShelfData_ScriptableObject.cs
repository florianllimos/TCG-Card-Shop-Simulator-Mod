using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 9)]
public class ShelfData_ScriptableObject : ScriptableObject
{
	public List<ObjectData> m_ObjectDataList;

	public List<FurniturePurchaseData> m_FurniturePurchaseDataList;

	public List<DecoData> m_DecoDataList;

	public List<DecoPurchaseData> m_DecoPurchaseDataList;

	public List<ShopDecoData> m_FloorDecoDataList;

	public List<ShopDecoData> m_WallDecoDataList;

	public List<ShopDecoData> m_WallBarDecoDataList;

	public List<ShopDecoData> m_CeilingDecoDataList;

	public List<EDecoObject> m_PosterDecoList = new List<EDecoObject>();

	public List<EDecoObject> m_OtherDecoList = new List<EDecoObject>();
}
