using UnityEngine;

public class UpdateLineRendererPosition : MonoBehaviour
{
	private LineRenderer m_Line;

	public GameObject m_StartPosObject;

	public GameObject m_MouseFollowObject;

	private Renderer rend;

	private void Awake()
	{
		m_Line = GetComponent<LineRenderer>();
		rend = GetComponent<Renderer>();
	}

	private void OnEnable()
	{
		m_Line.SetPosition(0, Vector3.zero);
		m_Line.SetPosition(1, Vector3.zero);
	}

	private void Update()
	{
		m_Line.SetPosition(0, m_StartPosObject.transform.position);
		m_Line.SetPosition(1, m_MouseFollowObject.transform.position);
		rend.material.mainTextureScale = new Vector2(Mathf.CeilToInt(Vector2.Distance(m_StartPosObject.transform.position, m_MouseFollowObject.transform.position) / 4f) * 2, 1f);
	}
}
