using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerSkillBarPanelUI : MonoBehaviour
{
	public EWorkerTask m_WorkerTask;

	public List<Image> m_StarImageList;

	public List<Sprite> m_StarSpriteList;

	public TextMeshProUGUI m_TaskText;

	public void UpdateWorkerSkillBar(Worker worker)
	{
		int taskSkillLevel = worker.GetTaskSkillLevel(m_WorkerTask);
		Sprite sprite = m_StarSpriteList[0];
		Sprite sprite2 = m_StarSpriteList[1];
		if (taskSkillLevel > 5)
		{
			sprite = m_StarSpriteList[1];
			sprite2 = m_StarSpriteList[2];
			for (int i = 0; i < m_StarImageList.Count; i++)
			{
				m_StarImageList[i].sprite = sprite;
			}
			for (int j = 0; j < taskSkillLevel - 5; j++)
			{
				m_StarImageList[j].sprite = sprite2;
			}
		}
		else
		{
			for (int k = 0; k < m_StarImageList.Count; k++)
			{
				m_StarImageList[k].sprite = sprite;
			}
			for (int l = 0; l < taskSkillLevel; l++)
			{
				m_StarImageList[l].sprite = sprite2;
			}
		}
		m_TaskText.text = WorkerManager.GetTaskName(m_WorkerTask);
	}
}
