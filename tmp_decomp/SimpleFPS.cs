using UnityEngine;

public class SimpleFPS : MonoBehaviour
{
	private float deltaTime;

	private GUIStyle style;

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		Object.Destroy(this);
		style = new GUIStyle();
		style.fontSize = 32;
		style.normal.textColor = Color.white;
	}

	private void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI()
	{
		float num = 1f / deltaTime;
		GUI.Label(new Rect(10f, 10f, 100f, 20f), num.ToString("F0"), style);
	}
}
