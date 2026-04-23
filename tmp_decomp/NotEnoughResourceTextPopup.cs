using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

public class NotEnoughResourceTextPopup : CSingleton<NotEnoughResourceTextPopup>
{
	public List<string> m_StringList;

	public List<GameObject> m_ShowTextGameObjectList;

	public List<TextMeshProUGUI> m_ShowTextList;

	private float m_ResetTimer;

	private ENotEnoughResourceText m_CurrentNotEnoughResourceText = ENotEnoughResourceText.None;

	private void Update()
	{
		if (m_CurrentNotEnoughResourceText != ENotEnoughResourceText.None)
		{
			m_ResetTimer += Time.deltaTime;
			if (m_ResetTimer > 2f)
			{
				m_ResetTimer = 0f;
				m_CurrentNotEnoughResourceText = ENotEnoughResourceText.None;
			}
		}
	}

	public static void ShowText(ENotEnoughResourceText notEnoughResourceText)
	{
		if (CSingleton<NotEnoughResourceTextPopup>.Instance.m_CurrentNotEnoughResourceText == notEnoughResourceText)
		{
			return;
		}
		CSingleton<NotEnoughResourceTextPopup>.Instance.m_ResetTimer = 0f;
		CSingleton<NotEnoughResourceTextPopup>.Instance.m_CurrentNotEnoughResourceText = notEnoughResourceText;
		string translation = LocalizationManager.GetTranslation(CSingleton<NotEnoughResourceTextPopup>.Instance.m_StringList[(int)notEnoughResourceText]);
		for (int i = 0; i < CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList.Count; i++)
		{
			if (!CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].activeSelf)
			{
				CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextList[i].text = translation;
				CSingleton<NotEnoughResourceTextPopup>.Instance.m_ShowTextGameObjectList[i].gameObject.SetActive(value: true);
				break;
			}
		}
	}
}
