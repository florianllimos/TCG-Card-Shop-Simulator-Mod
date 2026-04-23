using UnityEngine;

namespace CC;

public class ScaleCharacter : MonoBehaviour
{
	public enum mode
	{
		Scale,
		CounterScale
	}

	public Vector3 TargetScale = new Vector3(1f, 1f, 1f);

	public float Height = 1f;

	public float Width = 1f;

	public float CounterScaleLerp = 0.5f;

	public mode Mode;

	public void SetHeight(float _Height)
	{
		Height = _Height;
		TargetScale = new Vector3(Height * Width, Height * Width, Height);
	}

	public void SetWidth(float _Width)
	{
		Width = _Width;
		TargetScale = new Vector3(Height * Width, Height * Width, Height);
	}

	private void LateUpdate()
	{
		switch (Mode)
		{
		case mode.Scale:
			base.transform.localScale = TargetScale;
			break;
		case mode.CounterScale:
			base.transform.localScale = new Vector3(1f / TargetScale.z * Mathf.Lerp(TargetScale.z, 1f, CounterScaleLerp), 1f / TargetScale.y * Mathf.Lerp(TargetScale.z, 1f, CounterScaleLerp), 1f / TargetScale.x * Mathf.Lerp(TargetScale.z, 1f, CounterScaleLerp));
			break;
		}
	}
}
