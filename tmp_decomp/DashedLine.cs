using UnityEngine;

public class DashedLine : MonoBehaviour
{
	private float m_TilingAmount = 8f;

	private float m_ScaleAmount = 0.05f;

	public MeshRenderer m_RendererHorizontal1;

	public MeshRenderer m_RendererHorizontal2;

	public MeshRenderer m_RendererVertical1;

	public MeshRenderer m_RendererVertical2;

	private Material m_MatHorizontal;

	private Material m_MatVertical;

	private void Awake()
	{
		m_MatHorizontal = m_RendererHorizontal1.material;
		m_RendererHorizontal1.material = m_MatHorizontal;
		m_RendererHorizontal2.material = m_MatHorizontal;
		m_MatVertical = m_RendererVertical1.material;
		m_RendererVertical1.material = m_MatVertical;
		m_RendererVertical2.material = m_MatVertical;
		ShelfManager.AddDashedLine(this);
		base.gameObject.SetActive(value: false);
		m_MatHorizontal.mainTextureScale = new Vector2(m_TilingAmount * base.transform.lossyScale.x, 1f);
		Vector3 one = Vector3.one;
		one.z = m_ScaleAmount / base.transform.lossyScale.z * 2f;
		m_RendererHorizontal1.transform.localScale = one;
		m_RendererHorizontal2.transform.localScale = one;
		m_MatVertical.mainTextureScale = new Vector2(m_TilingAmount * base.transform.lossyScale.z, 1f);
		Vector3 one2 = Vector3.one;
		one2.z = m_ScaleAmount / base.transform.lossyScale.x * 2f;
		m_RendererVertical1.transform.localScale = one2;
		m_RendererVertical2.transform.localScale = one2;
	}
}
