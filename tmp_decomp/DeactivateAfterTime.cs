using System.Collections;
using UnityEngine;

public class DeactivateAfterTime : MonoBehaviour
{
	public float m_Timer = 1f;

	public bool m_IgnoreTimescale;

	private Coroutine m_CurrentCoroutine;

	private void OnEnable()
	{
		m_CurrentCoroutine = StartCoroutine(DestroyDelay());
	}

	private void OnDisable()
	{
		if (m_CurrentCoroutine != null)
		{
			StopCoroutine(m_CurrentCoroutine);
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator DestroyDelay()
	{
		if (m_IgnoreTimescale)
		{
			yield return new WaitForSecondsRealtime(m_Timer);
		}
		else
		{
			yield return new WaitForSeconds(m_Timer);
		}
		base.gameObject.SetActive(value: false);
	}
}
