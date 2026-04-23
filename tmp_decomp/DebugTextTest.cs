using TMPro;
using UnityEngine;

public class DebugTextTest : CSingleton<DebugTextTest>
{
	public static DebugTextTest m_Instance;

	public TextMeshProUGUI m_Text;

	public TextMeshProUGUI m_Text2;

	private static string m_CurrentString;

	private static int m_StringCount;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (base.transform.root == base.transform)
		{
			Object.DontDestroyOnLoad(this);
		}
	}

	public static void PrintText2(string text)
	{
	}

	public static void PrintText(string text)
	{
	}
}
