using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardUISettingData
{
	public List<ECardBorderType> applicableBorderList;

	public List<Sprite> cardBGList;

	public List<Sprite> cardBorderImageList;

	public List<Sprite> cardRarityImageList;

	public List<Sprite> cardFrontImageList;

	public Sprite cardCenterFrameMask;

	public Sprite cardBorderMask;

	public Sprite statImage;

	public Color cardBGColorNonFoil;

	public Color cardBGColorFoil;

	public Color evoBGColor;

	public Color playEffectBGColor;

	public Color descriptionBGColor;

	public bool showCardFront;

	public bool showBorder;

	public bool showName;

	public bool showNumber;

	public bool showRarity;

	public bool showEdition;

	public bool showStat1;

	public bool showStat2;

	public bool showStat3;

	public bool showStat4;

	public bool showEvoAndArtistNameGrp;

	public bool showFadeBarTop;

	public bool showFadeBarBtm;

	public bool showCenterFoilGlitter;

	public bool showBorderFoilGlitter;

	public Vector3 centerFrameMaskPosOffset;

	public Vector3 centerFrameMaskScaleOffset;

	public Vector3 cardFrontImagePosOffset;

	public Vector3 cardFrontImageScaleOffset;

	public Vector3 centerImageGrpPosOffset;

	public Vector3 centerImageGrpScaleOffset;

	public Vector3 namePosOffset;

	public Vector3 nameScaleOffset;

	public Vector3 numberPosOffset;

	public Vector3 numberScaleOffset;

	public Vector3 editionPosOffset;

	public Vector3 editionScaleOffset;

	public Vector3 statGrpPosOffset;

	public Vector3 statGrpScaleOffset;

	public Vector3 stat1PosOffset;

	public Vector3 stat1ScaleOffset;

	public Vector3 stat2PosOffset;

	public Vector3 stat2ScaleOffset;

	public Vector3 stat3PosOffset;

	public Vector3 stat3ScaleOffset;

	public Vector3 stat4PosOffset;

	public Vector3 stat4ScaleOffset;

	public Vector3 evoAndArtistNameGrpPosOffset;

	public Vector3 evoAndArtistNameGrpScaleOffset;

	public Vector3 evoGrpPosOffset;

	public Vector3 evoGrpScaleOffset;

	public Vector3 descriptionGrpPosOffset;

	public Vector3 descriptionGrpScaleOffset;

	public Vector3 artistNameGrpPosOffset;

	public Vector3 artistNameGrpScaleOffset;

	public Sprite GetCardBGSprite(EElementIndex element)
	{
		if (element == EElementIndex.None || (int)element >= cardBGList.Count)
		{
			return cardBGList[0];
		}
		return cardBGList[(int)element];
	}

	public Sprite GetCardBorderSprite(ECardBorderType borderType)
	{
		if (cardBorderImageList.Count == 0)
		{
			return null;
		}
		if ((int)borderType >= cardBorderImageList.Count)
		{
			return cardBorderImageList[0];
		}
		return cardBorderImageList[(int)borderType];
	}

	public Sprite GetCardRaritySprite(ERarity cardRarity)
	{
		if (cardRarityImageList.Count == 0)
		{
			return null;
		}
		if ((int)cardRarity >= cardRarityImageList.Count)
		{
			return cardRarityImageList[0];
		}
		return cardRarityImageList[(int)cardRarity];
	}

	public Sprite GetCardFrontSprite(EElementIndex elementIndex)
	{
		if (cardFrontImageList.Count == 0)
		{
			return null;
		}
		if (elementIndex == EElementIndex.None || (int)elementIndex >= cardFrontImageList.Count)
		{
			return cardFrontImageList[0];
		}
		return cardFrontImageList[(int)elementIndex];
	}
}
