using System.Collections;
using UnityEngine;

public class InteractablePackagingBox : InteractableObject
{
	public Collider m_Collider;

	public Rigidbody m_Rigidbody;

	public Animation m_BoxAnim;

	public Vector3 m_BoxPhysicsDimension;

	public Color m_MoveBoxPreviewColor;

	protected Vector3 m_OriginalScale_ShelfWorldUIGrp;

	protected override void Awake()
	{
		base.Awake();
		m_OriginalParent = base.transform.parent;
		m_OriginalScale = base.transform.localScale;
	}

	public override void OnMouseButtonUp()
	{
		StartHoldBox(isPlayer: true, CSingleton<InteractionPlayerController>.Instance.m_HoldItemPos);
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsMovingObject)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, m_TargetMoveObjectPosition, Time.deltaTime * 7.5f);
			if ((bool)m_MoveStateValidArea)
			{
				m_IsMovingObjectValidState = true;
			}
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if ((bool)m_Shelf_WorldUIGrp)
		{
			m_Shelf_WorldUIGrp.transform.position = base.transform.position;
			m_Shelf_WorldUIGrp.transform.rotation = base.transform.rotation;
			m_Shelf_WorldUIGrp.localScale = m_OriginalScale_ShelfWorldUIGrp * base.transform.lossyScale.x;
		}
	}

	public override void StartMoveObject()
	{
		CSingleton<InteractionPlayerController>.Instance.OnEnterMoveBoxMode(m_BoxPhysicsDimension);
		OnRaycastEnded();
		m_IsMovingObject = true;
		m_IsMovingObjectValidState = false;
		m_OriginalLayer = base.gameObject.layer;
		base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		m_BoxCollider.enabled = false;
		m_Rigidbody.isKinematic = true;
		OnStartMoveObject();
		SoundManager.PlayAudio("SFX_WhipSoft", 0.6f);
		Quaternion rotation = CSingleton<InteractionPlayerController>.Instance.m_CameraController.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		eulerAngles.x = 0f;
		rotation.eulerAngles = eulerAngles;
		base.transform.rotation = rotation;
	}

	protected override void OnPlacedMovedObject()
	{
		CSingleton<InteractionPlayerController>.Instance.OnExitMoveObjectMode();
		m_IsMovingObject = false;
		base.gameObject.layer = m_OriginalLayer;
		m_BoxCollider.enabled = true;
		m_Rigidbody.isKinematic = false;
	}

	protected override void PlayPlaceMoveObjectSFX()
	{
		SoundManager.PlayAudio("SFX_Throw", 0.3f);
	}

	public virtual void StartHoldBox(bool isPlayer, Transform holdItemPos)
	{
		if (!m_IsBeingHold)
		{
			if (isPlayer)
			{
				CSingleton<InteractionPlayerController>.Instance.OnEnterHoldBoxMode(this);
				SoundManager.PlayAudio("SFX_WhipSoft", 0.6f);
			}
			OnRaycastEnded();
			SetPhysicsEnabled(isEnable: false);
			LerpToTransform(holdItemPos, holdItemPos);
			m_MoveStateValidArea.gameObject.SetActive(value: true);
			m_IsBeingHold = true;
		}
	}

	public virtual void OnPressOpenBox()
	{
	}

	public virtual void OnHoldStateLeftMousePress(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
	}

	public virtual void OnHoldStateRightMousePress(bool isPlayer, ShelfCompartment targetItemCompartment)
	{
	}

	protected virtual void SpawnPriceTag()
	{
		m_Shelf_WorldUIGrp = PriceTagUISpawner.SpawnShelfWorldUIGrp(base.transform);
		m_OriginalScale_ShelfWorldUIGrp = m_Shelf_WorldUIGrp.localScale;
	}

	protected virtual void SetPriceTagVisibility(bool isVisible)
	{
	}

	protected IEnumerator DelaySetPriceTagVisibility(float delayTime, bool isVisible)
	{
		yield return new WaitForSeconds(delayTime);
		SetPriceTagVisibility(isVisible);
	}

	public virtual void ThrowBox(bool isPlayer)
	{
		if (isPlayer)
		{
			CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
			SoundManager.PlayAudio("SFX_Throw", 0.6f);
		}
		SetPhysicsEnabled(isEnable: true);
		base.transform.parent = m_OriginalParent.transform;
		base.transform.localScale = m_OriginalScale;
		m_Rigidbody.AddForce(base.transform.forward * 450f);
		m_IsBeingHold = false;
	}

	public virtual void DropBox(bool isPlayer)
	{
		if (isPlayer)
		{
			CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
		}
		SetPhysicsEnabled(isEnable: true);
		base.transform.parent = m_OriginalParent.transform;
		base.transform.localScale = m_OriginalScale;
		m_IsBeingHold = false;
	}

	public virtual bool IsBoxOpened()
	{
		return false;
	}

	public void SetPhysicsEnabled(bool isEnable)
	{
		if (isEnable)
		{
			m_Collider.enabled = true;
			m_Rigidbody.isKinematic = false;
		}
		else
		{
			m_Collider.enabled = false;
			m_Rigidbody.isKinematic = true;
		}
	}
}
