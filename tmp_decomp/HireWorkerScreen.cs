using System.Collections.Generic;

public class HireWorkerScreen : GenericSliderScreen
{
	public List<HireWorkerPanelUI> m_HireWorkerPanelUIList;

	protected override void Init()
	{
		base.Init();
		for (int i = 0; i < m_HireWorkerPanelUIList.Count; i++)
		{
			m_HireWorkerPanelUIList[i].SetActive(isActive: false);
		}
		for (int j = 0; j < CSingleton<WorkerManager>.Instance.m_WorkerDataList.Count; j++)
		{
			m_HireWorkerPanelUIList[j].Init(this, j);
			m_HireWorkerPanelUIList[j].SetActive(isActive: true);
			m_ScrollEndPosParent = m_HireWorkerPanelUIList[j].gameObject;
		}
	}

	protected override void OnOpenScreen()
	{
		Init();
		base.OnOpenScreen();
	}
}
