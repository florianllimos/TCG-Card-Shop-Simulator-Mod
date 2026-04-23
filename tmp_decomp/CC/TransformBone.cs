using UnityEngine;

namespace CC;

public class TransformBone : MonoBehaviour
{
	public enum mode
	{
		Height,
		FootRotation,
		BallRotation
	}

	public enum axis
	{
		X,
		Y,
		Z
	}

	public Vector3 offset = new Vector3(0f, 0f, 0f);

	public axis Axis;

	public mode Mode;

	private Transform m_Transform;

	private void Awake()
	{
		m_Transform = base.transform;
	}

	public void SetOffset(FootOffset footOffset)
	{
		float num = Mode switch
		{
			mode.Height => footOffset.HeightOffset, 
			mode.FootRotation => footOffset.FootRotation, 
			mode.BallRotation => footOffset.BallRotation, 
			_ => 0f, 
		};
		switch (Axis)
		{
		case axis.X:
			offset = new Vector3(num, 0f, 0f);
			break;
		case axis.Y:
			offset = new Vector3(0f, num, 0f);
			break;
		case axis.Z:
			offset = new Vector3(0f, 0f, num);
			break;
		}
	}

	private void LateUpdate()
	{
		if (!(offset == Vector3.zero))
		{
			switch (Mode)
			{
			case mode.Height:
			{
				Vector3 position = m_Transform.position;
				position += offset * 0.01f;
				m_Transform.position = position;
				break;
			}
			case mode.FootRotation:
			case mode.BallRotation:
			{
				Vector3 eulerAngles = m_Transform.eulerAngles;
				eulerAngles += offset;
				m_Transform.eulerAngles = eulerAngles;
				break;
			}
			}
		}
	}
}
