using I2.Loc;
using TMPro;
using UnityEngine;

public class SaveLoadSlotPanelUI : MonoBehaviour
{
	public int m_SlotIndex;

	public GameObject m_SaveBtn;

	public GameObject m_LoadBtn;

	public TextMeshProUGUI m_ShopName;

	public TextMeshProUGUI m_Money;

	public TextMeshProUGUI m_Level;

	public TextMeshProUGUI m_DaysPassed;

	private bool m_IsSaveState;

	public void SetSaveOrLoadState(bool isSave)
	{
		m_IsSaveState = isSave;
		if (m_IsSaveState)
		{
			m_SaveBtn.SetActive(value: true);
			m_LoadBtn.SetActive(value: false);
			if (m_SlotIndex == 0)
			{
				m_SaveBtn.SetActive(value: false);
			}
		}
		else
		{
			m_SaveBtn.SetActive(value: false);
			m_LoadBtn.SetActive(value: true);
		}
	}

	public void LoadSlotData(LoadSavedSlotData loadSavedSlotData)
	{
		if (loadSavedSlotData.hasSaveData)
		{
			m_ShopName.text = loadSavedSlotData.name;
			m_Money.text = GameInstance.GetPriceString(loadSavedSlotData.moneyAmount);
			m_Level.text = "Lv " + (loadSavedSlotData.level + 1);
			m_DaysPassed.text = LocalizationManager.GetTranslation("Day XXX").Replace("XXX", (loadSavedSlotData.daysPassed + 1).ToString());
		}
		else
		{
			m_ShopName.text = "-";
			m_Money.text = "-";
			m_Level.text = "-";
			m_DaysPassed.text = "-";
		}
	}

	public void OnPressLoadGame()
	{
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.OnPressLoadGame(m_SlotIndex);
	}

	public void OnPressSaveGame()
	{
		CSingleton<SaveLoadGameSlotSelectScreen>.Instance.OnPressSaveGame(m_SlotIndex);
	}
}
