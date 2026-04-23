using UnityEngine;

public class scrolltex : MonoBehaviour
{
	public float Scrollx = 0.5f;

	public float Scrolly = 0.5f;

	private void Update()
	{
		float x = Time.time * Scrollx;
		float y = Time.time * Scrolly;
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(x, y);
	}
}
