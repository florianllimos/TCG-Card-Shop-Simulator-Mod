using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	private float deltaTime;

	private void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	private void OnGUI()
	{
		int width = Screen.width;
		int height = Screen.height;
		GUIStyle gUIStyle = new GUIStyle();
		Rect position = new Rect(250f, 550f, width, height * 2 / 100);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		gUIStyle.fontSize = height * 4 / 100;
		gUIStyle.normal.textColor = new Color(0f, 1f, 0f, 1f);
		float num = deltaTime * 1000f;
		string text = string.Format(arg1: 1f / deltaTime, format: "{0:0.0} ms ({1:0.} fps)", arg0: num);
		GUI.Label(position, text, gUIStyle);
	}
}
