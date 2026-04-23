using UnityEngine;

namespace CMF;

public class CameraMouseInput : CameraInput
{
	public string mouseHorizontalAxis = "Mouse X";

	public string mouseVerticalAxis = "Mouse Y";

	public string joystickHorizontalAxis = "RJoystick X";

	public string joystickVerticalAxis = "RJoystick Y";

	public string joystickHorizontalAxisPS = "RJoystick X PS";

	public string joystickVerticalAxisPS = "RJoystick Y PS";

	public bool invertHorizontalInput;

	public bool invertVerticalInput;

	public float mouseInputMultiplier = 0.01f;

	public float joystickInputMultiplier = 1f;

	private float stickResponseExponent = 1.2f;

	private float stickAcceleration = 4f;

	private float stickMaxBoost = 2f;

	private float stickReleaseSpeed = 10f;

	private float horizontalStickBoost = 1f;

	private float verticalStickBoost = 1f;

	private void Awake()
	{
		joystickInputMultiplier = 1f;
	}

	public override float GetHorizontalCameraInput()
	{
		float value;
		if (CSingleton<InputManager>.Instance.m_IsControllerActive && CSingleton<InputManager>.Instance.m_CurrentGamepad != null)
		{
			value = CSingleton<InputManager>.Instance.m_CurrentGamepad.rightStick.x.value;
			float num = Mathf.Abs(value);
			if (num > 0.001f)
			{
				float num2 = Mathf.Sign(value) * Mathf.Pow(num, stickResponseExponent);
				horizontalStickBoost = Mathf.MoveTowards(horizontalStickBoost, stickMaxBoost, stickAcceleration * Time.unscaledDeltaTime);
				value = num2 * joystickInputMultiplier * horizontalStickBoost;
			}
			else
			{
				horizontalStickBoost = Mathf.MoveTowards(horizontalStickBoost, 1f, stickReleaseSpeed * Time.unscaledDeltaTime);
				value = 0f;
			}
		}
		else
		{
			value = Input.GetAxisRaw(mouseHorizontalAxis);
			if (Time.timeScale > 0f && Time.deltaTime > 0f)
			{
				value /= Time.deltaTime;
				value *= Time.timeScale;
			}
			else
			{
				value = 0f;
			}
			value *= mouseInputMultiplier;
		}
		if (invertHorizontalInput)
		{
			value *= -1f;
		}
		return value;
	}

	public override float GetVerticalCameraInput()
	{
		float f;
		if (CSingleton<InputManager>.Instance.m_IsControllerActive && CSingleton<InputManager>.Instance.m_CurrentGamepad != null)
		{
			f = 0f - CSingleton<InputManager>.Instance.m_CurrentGamepad.rightStick.y.value;
			float num = Mathf.Abs(f);
			if (num > 0.001f)
			{
				float num2 = Mathf.Sign(f) * Mathf.Pow(num, stickResponseExponent);
				verticalStickBoost = Mathf.MoveTowards(verticalStickBoost, stickMaxBoost, stickAcceleration * Time.unscaledDeltaTime);
				f = num2 * joystickInputMultiplier * verticalStickBoost;
			}
			else
			{
				verticalStickBoost = Mathf.MoveTowards(verticalStickBoost, 1f, stickReleaseSpeed * Time.unscaledDeltaTime);
				f = 0f;
			}
		}
		else
		{
			f = 0f - Input.GetAxisRaw(mouseVerticalAxis);
			if (Time.timeScale > 0f && Time.deltaTime > 0f)
			{
				f /= Time.deltaTime;
				f *= Time.timeScale;
			}
			else
			{
				f = 0f;
			}
			f *= mouseInputMultiplier;
		}
		if (invertVerticalInput)
		{
			f *= -1f;
		}
		return f;
	}
}
