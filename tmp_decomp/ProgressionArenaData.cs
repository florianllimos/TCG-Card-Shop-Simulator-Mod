using System;
using I2.Loc;
using UnityEngine;

[Serializable]
public struct ProgressionArenaData
{
	public int m_UnlockMMR;

	public Sprite m_ArenaImage;

	public string m_ArenaTitle;

	public string GetArenaName()
	{
		return LocalizationManager.GetTranslation(m_ArenaTitle);
	}

	public string GetArenaNumberText(int index)
	{
		string text = "";
		if (index == -1)
		{
			return LocalizationManager.GetTranslation("Beginner Arena");
		}
		return LocalizationManager.GetTranslation("Arena") + " " + (index + 1);
	}
}
