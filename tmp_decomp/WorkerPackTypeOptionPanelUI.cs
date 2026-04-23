using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerPackTypeOptionPanelUI : UIElementBase
{
	public Image m_Image;

	public TextMeshProUGUI m_NameText;

	public GameObject m_TickImage;

	public GameObject m_LockedGrp;

	private bool m_IsEnabled;

	private bool m_IsUnlocked;

	private int m_Index;

	private WorkerSetPackOpenerTypeOptionScreen m_WorkerSetPackOpenerTypeOptionScreen;

	public void Init(WorkerSetPackOpenerTypeOptionScreen workerSetPackOpenerTypeOptionScreen, int index)
	{
		m_WorkerSetPackOpenerTypeOptionScreen = workerSetPackOpenerTypeOptionScreen;
		m_Index = index;
	}

	public void UpdatePackType(Sprite itemIcon, string itemName, bool isUnlocked)
	{
		m_Image.sprite = itemIcon;
		m_NameText.text = itemName;
		m_IsUnlocked = isUnlocked;
		m_LockedGrp.SetActive(!isUnlocked);
	}

	public void SetPackTypeEnabled(bool isEnabled)
	{
		m_TickImage.SetActive(isEnabled && m_IsUnlocked);
	}

	public override void OnPressButton()
	{
		if (m_IsUnlocked)
		{
			m_WorkerSetPackOpenerTypeOptionScreen.OnPressWorkerPackTypeToggle(m_Index);
		}
	}
}
