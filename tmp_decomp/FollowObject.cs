using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public GameObject m_Target;

	public GameObject m_LastFollowTarget;

	public bool m_MaintainCurrentPos;

	public bool m_FollowRot;

	public bool m_FollowScale;

	private Vector3 m_PosOffset;

	public bool m_IgnoreY;

	public bool m_IgnoreDisableTarget;

	private void Awake()
	{
		if (!(m_Target == null))
		{
			m_PosOffset = m_Target.transform.position - base.transform.position;
		}
	}

	private void LateUpdate()
	{
		if (m_Target == null)
		{
			return;
		}
		if (m_MaintainCurrentPos)
		{
			base.transform.position = m_Target.transform.position - m_PosOffset;
			return;
		}
		if (m_IgnoreY)
		{
			base.transform.position = new Vector3(m_Target.transform.position.x, base.transform.position.y, m_Target.transform.position.z);
			return;
		}
		base.transform.position = m_Target.transform.position;
		if (m_FollowRot)
		{
			base.transform.rotation = m_Target.transform.rotation;
		}
		if (m_FollowScale)
		{
			base.transform.localScale = m_Target.transform.lossyScale;
		}
	}

	public void SetFollowTarget(Transform followTarget)
	{
		m_Target = followTarget.gameObject;
	}

	private void OnEnable()
	{
		if (m_Target == null)
		{
			m_Target = m_LastFollowTarget;
		}
	}

	private void OnDisable()
	{
		if (!m_IgnoreDisableTarget)
		{
			m_LastFollowTarget = m_Target;
			m_Target = null;
		}
	}
}
