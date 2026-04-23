using UnityEngine;

public class LegendCardGlowTexturePanner : MonoBehaviour
{
	public Material m_Material1;

	public Material m_Material2;

	public float m_SpeedX1 = 1f;

	public float m_SpeedX2 = 1f;

	public float m_SpeedY1 = 1f;

	public float m_SpeedY2 = 1f;

	private float m_X1;

	private float m_X2;

	private float m_Y1;

	private float m_Y2;

	private Vector2 m_Offset1;

	private Vector2 m_Offset2;

	private void Start()
	{
	}

	private void Update()
	{
		m_X1 += Time.deltaTime * m_SpeedX1;
		m_X2 += Time.deltaTime * m_SpeedX2;
		m_Y1 += Time.deltaTime * m_SpeedY1;
		m_Y2 += Time.deltaTime * m_SpeedY2;
		m_Offset1.x = m_X1;
		m_Offset2.x = m_X2;
		m_Offset1.y = m_Y1;
		m_Offset2.y = m_Y2;
		m_Material1.SetFloat("_OffsetX", m_X1);
		m_Material2.SetFloat("_OffsetX", m_X2);
		m_Material1.SetTextureOffset("_Texture", m_Offset1);
		m_Material2.SetTextureOffset("_Texture", m_Offset2);
	}

	private void OnApplicationQuit()
	{
		m_Material1.SetTextureOffset("_Texture", Vector2.zero);
		m_Material2.SetTextureOffset("_Texture", Vector2.zero);
		m_Material1.SetFloat("_OffsetX", 0f);
		m_Material2.SetFloat("_OffsetX", 0f);
	}
}
