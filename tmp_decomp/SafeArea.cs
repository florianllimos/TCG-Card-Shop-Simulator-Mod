using UnityEngine;

public class SafeArea : MonoBehaviour
{
	private RectTransform Panel;

	private Rect LastSafeArea = new Rect(0f, 0f, 0f, 0f);

	public Rect m_Test;

	private Rect m_OffsetUI;

	public bool m_DebugTestArea;

	public bool m_IsManual;

	private void Awake()
	{
		Panel = GetComponent<RectTransform>();
		if (!m_IsManual)
		{
			Refresh();
		}
	}

	private void Update()
	{
		if (!m_IsManual)
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		if (base.enabled)
		{
			Rect safeArea = GetSafeArea();
			if (safeArea != LastSafeArea)
			{
				ApplySafeArea(safeArea);
			}
		}
	}

	private Rect GetSafeArea()
	{
		_ = m_DebugTestArea;
		return Screen.safeArea;
	}

	private void ApplySafeArea(Rect r)
	{
		LastSafeArea = r;
		Vector2 position = r.position;
		Vector2 anchorMax = r.position + r.size;
		position.x /= Screen.width;
		position.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		position.x = Panel.anchorMin.x;
		anchorMax.x = Panel.anchorMax.x;
		Panel.anchorMin = position;
		Panel.anchorMax = anchorMax;
		Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", base.name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
	}
}
