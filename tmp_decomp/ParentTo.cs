using UnityEngine;

public class ParentTo : MonoBehaviour
{
	public bool m_AutoParent;

	public Transform m_Parent;

	private void Awake()
	{
		if (m_AutoParent)
		{
			StartParent();
		}
	}

	public void StartParent()
	{
		base.transform.parent = m_Parent;
	}
}
