using UnityEngine;

public class WorkerCollider : MonoBehaviour
{
	public Worker m_Worker;

	public Customer m_Customer;

	public void OnRaycasted()
	{
		if ((bool)m_Worker)
		{
			m_Worker.OnRaycasted();
		}
		else
		{
			_ = (bool)m_Customer;
		}
	}

	public void OnRaycastEnded()
	{
		if ((bool)m_Worker)
		{
			m_Worker.OnRaycastEnded();
		}
		else
		{
			_ = (bool)m_Customer;
		}
	}

	public void OnMousePress()
	{
		if ((bool)m_Worker)
		{
			m_Worker.OnMousePress();
		}
		else if ((bool)m_Customer)
		{
			m_Customer.OnMousePress();
		}
	}
}
