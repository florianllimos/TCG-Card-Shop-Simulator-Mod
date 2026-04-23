using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterDot : CSingleton<CenterDot>
{
	public Image m_DotImage;

	public List<Color> m_ColorList;

	public List<Sprite> m_SpriteList;

	public List<Sprite> m_OutlineSpriteList;

	private void Start()
	{
		EvaluateDot();
	}

	public static void SetVisibility(bool isVisible)
	{
		CSingleton<CenterDot>.Instance.m_DotImage.enabled = isVisible;
	}

	private void EvaluateDot()
	{
		if (CSingleton<CGameManager>.Instance.m_CenterDotHasOutline)
		{
			m_DotImage.sprite = m_OutlineSpriteList[CSingleton<CGameManager>.Instance.m_CenterDotSpriteTypeIndex];
		}
		else
		{
			m_DotImage.sprite = m_SpriteList[CSingleton<CGameManager>.Instance.m_CenterDotSpriteTypeIndex];
		}
		m_DotImage.color = m_ColorList[CSingleton<CGameManager>.Instance.m_CenterDotColorIndex];
		m_DotImage.transform.localScale = Vector3.one * Mathf.Clamp(CSingleton<CGameManager>.Instance.m_CenterDotSizeSlider, 0.05f, 1f);
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.AddListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
			CEventManager.RemoveListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		EvaluateDot();
	}

	protected void OnSettingUpdated(CEventPlayer_OnSettingUpdated evt)
	{
		EvaluateDot();
	}
}
