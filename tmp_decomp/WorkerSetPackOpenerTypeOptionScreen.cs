using System.Collections.Generic;
using UnityEngine;

public class WorkerSetPackOpenerTypeOptionScreen : MonoBehaviour
{
	public List<WorkerPackTypeOptionPanelUI> m_WorkerPackTypeOptionPanelUIList;

	public ControllerScreenUIExtension m_ControllerScreenUIExtension;

	public GameObject m_ScreenGrp;

	private Worker m_Worker;

	private List<bool> m_IsPackTypeEnabled = new List<bool>();

	private List<EItemType> m_CardPackItemTypeList = new List<EItemType>();

	private List<ItemData> m_CardPackItemDataList = new List<ItemData>();

	private List<bool> m_CardPackItemUnlockedList = new List<bool>();

	private void Awake()
	{
		for (int i = 0; i < CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_CardPackItemTypeList.Count; i++)
		{
			m_CardPackItemTypeList.Add(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_CardPackItemTypeList[i]);
			m_CardPackItemDataList.Add(InventoryBase.GetItemData(CSingleton<InventoryBase>.Instance.m_StockItemData_SO.m_CardPackItemTypeList[i]));
		}
	}

	public void OpenScreen(Worker worker)
	{
		m_Worker = worker;
		m_ScreenGrp.SetActive(value: true);
		SoundManager.GenericMenuOpen();
		ControllerScreenUIExtManager.OnOpenScreen(m_ControllerScreenUIExtension);
		bool flag = false;
		m_CardPackItemUnlockedList.Clear();
		for (int i = 0; i < m_CardPackItemTypeList.Count; i++)
		{
			List<RestockData> restockDataUsingItemType = InventoryBase.GetRestockDataUsingItemType(m_CardPackItemTypeList[i]);
			flag = false;
			for (int j = 0; j < restockDataUsingItemType.Count; j++)
			{
				if (CPlayerData.GetIsItemLicenseUnlocked(restockDataUsingItemType[j].index))
				{
					flag = true;
					break;
				}
			}
			m_CardPackItemUnlockedList.Add(flag);
		}
		for (int k = 0; k < m_WorkerPackTypeOptionPanelUIList.Count; k++)
		{
			m_WorkerPackTypeOptionPanelUIList[k].Init(this, k);
			m_WorkerPackTypeOptionPanelUIList[k].UpdatePackType(m_CardPackItemDataList[k].icon, m_CardPackItemDataList[k].GetName(), m_CardPackItemUnlockedList[k]);
		}
		bool flag2 = false;
		m_IsPackTypeEnabled.Clear();
		for (int l = 0; l < m_Worker.GetCardPackItemTypeEnabledList().Count; l++)
		{
			flag2 = m_Worker.GetCardPackItemTypeEnabledList()[l];
			m_IsPackTypeEnabled.Add(flag2);
			m_WorkerPackTypeOptionPanelUIList[l].SetPackTypeEnabled(flag2);
		}
	}

	public void CloseScreen()
	{
		m_ScreenGrp.SetActive(value: false);
		SoundManager.GenericMenuClose();
		ControllerScreenUIExtManager.OnCloseScreen(m_ControllerScreenUIExtension);
	}

	public void OnPressWorkerPackTypeToggle(int index)
	{
		m_IsPackTypeEnabled[index] = !m_IsPackTypeEnabled[index];
		m_WorkerPackTypeOptionPanelUIList[index].SetPackTypeEnabled(m_IsPackTypeEnabled[index]);
	}

	public void OnPressConfirm()
	{
		for (int i = 0; i < m_IsPackTypeEnabled.Count; i++)
		{
			m_Worker.SetCardPackItemTypeEnabled(i, m_IsPackTypeEnabled[i]);
		}
		if (m_Worker.GetIsSetTaskSettingPrimarySecondary())
		{
			m_Worker.SetTask(EWorkerTask.RefillCardOpener);
			m_Worker.SetLastTask(EWorkerTask.RefillCardOpener);
		}
		else
		{
			m_Worker.SetSecondaryTask(EWorkerTask.RefillCardOpener);
		}
		SoundManager.GenericConfirm();
		m_Worker.OnPressStopInteract();
		CloseScreen();
	}
}
