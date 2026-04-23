using UnityEngine;

namespace CMF;

public class CharacterKeyboardInput : CharacterInput
{
	public string horizontalInputAxis = "Horizontal";

	public string verticalInputAxis = "Vertical";

	public KeyCode jumpKey = KeyCode.Space;

	public KeyCode jumpKey2 = KeyCode.JoystickButton0;

	public bool useRawInput = true;

	public override float GetHorizontalMovementInput()
	{
		if (InputManager.GetKeyHoldAction(EGameAction.MoveLeft) || InputManager.GetKeyHoldAction(EGameAction.MoveLeftAlt))
		{
			return -1f;
		}
		if (InputManager.GetKeyHoldAction(EGameAction.MoveRight) || InputManager.GetKeyHoldAction(EGameAction.MoveRightAlt))
		{
			return 1f;
		}
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			_ = useRawInput;
			return CSingleton<InputManager>.Instance.m_CurrentGamepad.leftStick.x.value;
		}
		return 0f;
	}

	public override float GetVerticalMovementInput()
	{
		if (InputManager.GetKeyHoldAction(EGameAction.MoveForward) || InputManager.GetKeyHoldAction(EGameAction.MoveForwardAlt))
		{
			return 1f;
		}
		if (InputManager.GetKeyHoldAction(EGameAction.MoveBackward) || InputManager.GetKeyHoldAction(EGameAction.MoveBackwardAlt))
		{
			return -1f;
		}
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			_ = useRawInput;
			return CSingleton<InputManager>.Instance.m_CurrentGamepad.leftStick.y.value;
		}
		return 0f;
	}

	public override bool IsJumpKeyPressed()
	{
		if (CSingleton<InteractionPlayerController>.Instance.CanJump())
		{
			return InputManager.GetKeyHoldAction(EGameAction.Jump);
		}
		return false;
	}
}
