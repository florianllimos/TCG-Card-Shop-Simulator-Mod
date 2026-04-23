using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Inventory/List", order = 8)]
public class ProgressionData_ScriptableObject : ScriptableObject
{
	public ProgressionArenaData m_EmptyData;

	public ProgressionArenaData m_TutorialArenaData;

	public List<ProgressionArenaData> m_ProgressionArenaDataList;

	public List<int> m_TamerLevelUpExpRequiredList;

	public List<string> m_TamerLevelUpUnlockedText;

	public string GetTamerLevelUpUnlockText(int level)
	{
		return LocalizationManager.GetTranslation(m_TamerLevelUpUnlockedText[level]);
	}

	public ProgressionArenaData GetProgressionData(int playerRank, bool hasFinishTutorial)
	{
		if (hasFinishTutorial)
		{
			if (playerRank >= 0 && playerRank < m_ProgressionArenaDataList.Count)
			{
				return m_ProgressionArenaDataList[playerRank];
			}
			return m_EmptyData;
		}
		return m_TutorialArenaData;
	}
}
