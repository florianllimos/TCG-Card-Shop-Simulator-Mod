using UnityEngine;

public class RestockCategoryBar : MonoBehaviour
{
	public string m_URL;

	public void OnPressWebsite()
	{
		Application.OpenURL(m_URL);
	}
}
