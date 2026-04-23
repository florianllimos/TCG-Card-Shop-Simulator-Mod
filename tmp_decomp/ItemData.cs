using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[Serializable]
public class ItemData
{
	public string name;

	public EItemCategory category;

	public Sprite icon;

	public float iconScale;

	public float baseCost;

	public float marketPriceMinPercent;

	public float marketPriceMaxPercent;

	public EItemType boxFollowItemPrice;

	public bool isNotBoosterPack;

	public bool isTallItem;

	public bool isHideItemUntilUnlocked;

	public float posYOffsetInBox;

	public float scaleOffsetInBox;

	public Vector3 itemDimension;

	public Vector3 colliderPosOffset;

	public Vector3 colliderScale;

	public List<EPriceChangeType> affectedPriceChangeType;

	public string GetName()
	{
		return LocalizationManager.GetTranslation(name);
	}

	public float GetItemVolume()
	{
		if (isTallItem)
		{
			return itemDimension.x * itemDimension.y * itemDimension.z * 2f;
		}
		return itemDimension.x * itemDimension.y * itemDimension.z;
	}
}
