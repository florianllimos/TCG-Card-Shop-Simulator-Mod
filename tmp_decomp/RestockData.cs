using System;

[Serializable]
public class RestockData
{
	public string name;

	public bool isBigBox;

	public bool ignoreDoubleImage;

	public int index;

	public int amount;

	public int licenseShopLevelRequired;

	public float licensePrice;

	public EItemType itemType;

	public bool prologueShow;

	public bool isHideItemUntilUnlocked;
}
