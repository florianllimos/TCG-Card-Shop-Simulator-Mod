using UnityEngine;

public class UIElementBase : MonoBehaviour
{
	public GameObject m_TopUIGrp;

	public GameObject m_UIGrp;

	protected bool m_IsActive = true;

	public void SetActive(bool isActive)
	{
		m_IsActive = isActive;
		m_TopUIGrp.SetActive(isActive);
	}

	public virtual void OnPressButton()
	{
	}

	public bool IsActive()
	{
		return m_IsActive;
	}
}
