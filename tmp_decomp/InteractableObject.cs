using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InteractableObject : MonoBehaviour
{
	public EObjectType m_ObjectType = EObjectType.None;

	public EDecoObject m_DecoObjectType;

	public GameObject m_HighlightGameObj;

	public GameObject m_NavMeshCut;

	public MeshRenderer m_Mesh;

	public MeshRenderer m_CullingCheckMesh;

	public SkinnedMeshRenderer m_SkinMesh;

	public float m_HighlightOutlineWidth = 5f;

	public bool m_IsGenericObject;

	public bool m_CanPickupMoveObject;

	public bool m_CanBoxUpObject;

	public bool m_CanScanByCounter;

	public bool m_PlaceObjectInShopOnly = true;

	public bool m_PlaceObjectInWarehouseOnly;

	public bool m_IsDecorationVertical;

	public bool m_CanFlip;

	public MeshFilter m_PickupObjectMesh;

	public Transform m_MoveStateValidArea;

	public ShelfMoveStateValidArea m_ShelfMoveStateValidArea;

	public BoxCollider m_BoxCollider;

	public List<BoxCollider> m_BoxColliderList;

	public List<EGameAction> m_GameActionInputDisplayList;

	public List<EGameAction> m_ControllerOnlyGameActionInputDisplayList;

	protected bool m_IsRaycasted;

	protected bool m_IsLerpingToPos;

	protected bool m_IsHideAfterFinishLerp;

	protected bool m_IsMovingObject;

	protected bool m_IsBeingHold;

	protected bool m_IsBoxedUp;

	protected bool m_HasInit;

	protected bool m_IsSnappingPos;

	protected bool m_IsVerticalSnapToWarehouseWall;

	protected bool m_IsFlipped;

	protected int m_VerticalSnapWallIndex = -1;

	protected int m_OriginalLayer;

	protected Vector3 m_OriginalScale;

	protected Vector3 m_TargetSnapPos;

	protected Transform m_OriginalParent;

	protected Material m_Material;

	protected Transform m_Shelf_WorldUIGrp;

	protected InteractablePackagingBox_Shelf m_InteractablePackagingBox_Shelf;

	protected List<ShelfCompartment> m_ItemCompartmentList = new List<ShelfCompartment>();

	protected List<Transform> m_GamepadQuickSelectTransformList = new List<Transform>();

	protected List<Transform> m_GamepadQuickSelectPriceTagTransformList = new List<Transform>();

	protected bool m_IsMovingObjectValidState;

	protected float m_LerpPosTimer;

	protected float m_LerpPosSpeed = 3f;

	private Vector3 m_StartLerpPos;

	private Vector3 m_StartLerpScale;

	protected Vector3 m_LerpPosOffset = Vector3.zero;

	protected Vector3 m_TargetMoveObjectPosition;

	protected Vector3 m_HitNormal;

	protected Transform m_ObjHit;

	private Quaternion m_StartLerpRot;

	private Transform m_TargetLerpTransform;

	private Transform m_LastHitTransform;

	private LockedRoomBlocker m_DecoPlaccedLockedRoomBlocker;

	private float m_MoveObjectLerpSpeed = 0.002f;

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		Init();
	}

	public virtual void Init()
	{
		if (!m_HasInit)
		{
			m_HasInit = true;
			if ((bool)m_Mesh)
			{
				m_Material = m_Mesh.material;
				m_Material.SetFloat("_Outline", 0f);
			}
			else if ((bool)m_SkinMesh)
			{
				m_Material = m_SkinMesh.material;
				m_Material.SetFloat("_Outline", 0f);
			}
			else if ((bool)m_HighlightGameObj)
			{
				m_HighlightGameObj.SetActive(value: false);
			}
			m_OriginalLayer = base.gameObject.layer;
			m_TargetMoveObjectPosition = base.transform.position;
			if (m_IsGenericObject && m_ObjectType != EObjectType.None)
			{
				ShelfManager.InitInteractableObject(this);
			}
			else if (m_IsGenericObject && m_DecoObjectType != EDecoObject.None)
			{
				ShelfManager.InitDecoObject(this);
			}
		}
	}

	private void EvaluateSnapping()
	{
		if (CSingleton<InteractionPlayerController>.Instance.IsStopSnapWhenMoving())
		{
			m_IsSnappingPos = false;
			return;
		}
		if (m_IsSnappingPos)
		{
			if ((m_TargetMoveObjectPosition - m_TargetSnapPos).magnitude > 0.5f)
			{
				m_IsSnappingPos = false;
				if (m_TargetMoveObjectPosition.y < 0f)
				{
					m_TargetMoveObjectPosition.y = 0f;
				}
				base.transform.position = m_TargetMoveObjectPosition;
			}
			else
			{
				base.transform.position = m_TargetSnapPos;
			}
		}
		if (m_IsSnappingPos || !m_MoveStateValidArea || !m_ShelfMoveStateValidArea || !m_ShelfMoveStateValidArea.m_CanSnap)
		{
			return;
		}
		int mask = LayerMask.GetMask("MoveStateBlockedArea");
		Collider[] array = Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f * 1.01f, m_MoveStateValidArea.rotation, mask);
		if (array.Length == 0)
		{
			return;
		}
		ShelfMoveStateValidArea shelfMoveStateValidArea = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].name == "MoveStateValidArea" && array[i].transform.parent != m_MoveStateValidArea && (array[i].transform.position - m_TargetMoveObjectPosition).magnitude <= 5f)
			{
				shelfMoveStateValidArea = array[i].transform.parent.GetComponent<ShelfMoveStateValidArea>();
				if ((bool)shelfMoveStateValidArea && shelfMoveStateValidArea.m_CanSnap)
				{
					break;
				}
				shelfMoveStateValidArea = null;
			}
		}
		if (!shelfMoveStateValidArea)
		{
			return;
		}
		Transform transform = null;
		float num = 0.3f;
		for (int j = 0; j < m_ShelfMoveStateValidArea.m_SnapLocList.Count; j++)
		{
			for (int k = 0; k < shelfMoveStateValidArea.m_SnapLocList.Count; k++)
			{
				float magnitude = (shelfMoveStateValidArea.m_SnapLocList[k].position - m_ShelfMoveStateValidArea.m_SnapLocList[j].position).magnitude;
				if (magnitude <= num)
				{
					num = magnitude;
					transform = shelfMoveStateValidArea.m_SnapLocList[k];
				}
			}
		}
		if (!transform)
		{
			return;
		}
		Transform transform2 = null;
		num = 0.3f;
		for (int l = 0; l < m_ShelfMoveStateValidArea.m_SnapLocList.Count; l++)
		{
			float magnitude2 = (transform.position - m_ShelfMoveStateValidArea.m_SnapLocList[l].position).magnitude;
			if (magnitude2 <= num)
			{
				num = magnitude2;
				transform2 = m_ShelfMoveStateValidArea.m_SnapLocList[l];
			}
		}
		Vector3 position = m_MoveStateValidArea.position;
		position.y = 0f;
		Vector3 normalized = (position - transform2.position).normalized;
		float magnitude3 = (m_ShelfMoveStateValidArea.transform.position - transform2.position).magnitude;
		Vector3 vector = transform.position + normalized * 0.005f + normalized * magnitude3;
		vector.y = 0f;
		base.transform.position = vector;
		m_IsSnappingPos = true;
		m_TargetSnapPos = vector;
	}

	protected virtual void Update()
	{
		EvaluateOcclusionCulling();
		if (m_IsLerpingToPos)
		{
			m_LerpPosTimer += Time.deltaTime * m_LerpPosSpeed;
			base.transform.position = Vector3.Lerp(m_StartLerpPos, m_TargetLerpTransform.position, m_LerpPosTimer);
			EvaluateLerpPosOffset();
			base.transform.position += m_LerpPosOffset;
			base.transform.rotation = Quaternion.Lerp(m_StartLerpRot, m_TargetLerpTransform.rotation, m_LerpPosTimer);
			base.transform.localScale = Vector3.Lerp(m_StartLerpScale, m_TargetLerpTransform.localScale, m_LerpPosTimer);
			if (m_LerpPosTimer >= 1f)
			{
				m_LerpPosTimer = 0f;
				m_IsLerpingToPos = false;
				OnFinishLerp();
				if (m_IsHideAfterFinishLerp)
				{
					m_IsHideAfterFinishLerp = false;
					base.gameObject.SetActive(value: false);
				}
			}
		}
		else if (m_IsDecorationVertical)
		{
			if (!m_IsMovingObject)
			{
				return;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, m_TargetMoveObjectPosition, Time.deltaTime * 7.5f);
			int mask = LayerMask.GetMask("DecorationBlocker");
			Collider[] array = Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask);
			bool flag = true;
			if (array.Length == 0 || base.transform.position.y < 0.1f)
			{
				flag = false;
			}
			if (flag)
			{
				int mask2 = LayerMask.GetMask("DecorationMoveStateBlockedArea");
				if (Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask2).Length != 0)
				{
					flag = false;
				}
			}
			if (flag)
			{
				if (m_LastHitTransform != array[0].transform)
				{
					m_LastHitTransform = array[0].transform;
				}
				float z = base.transform.rotation.eulerAngles.z;
				Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, m_HitNormal);
				Vector3 eulerAngles = rotation.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = z;
				if (m_IsFlipped)
				{
					eulerAngles.y += 180f;
				}
				rotation.eulerAngles = eulerAngles;
				base.transform.rotation = rotation;
			}
			else
			{
				m_LastHitTransform = null;
			}
			if ((m_TargetMoveObjectPosition - base.transform.position).magnitude > 0.02f)
			{
				flag = false;
			}
			if (m_IsMovingObjectValidState != flag)
			{
				m_IsMovingObjectValidState = flag;
				ShelfManager.SetMoveObjectPreviewModelValidState(m_IsMovingObjectValidState);
			}
		}
		else
		{
			if (!m_IsMovingObject)
			{
				return;
			}
			if (!m_IsSnappingPos)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetMoveObjectPosition, Time.deltaTime * 7.5f);
			}
			EvaluateSnapping();
			int mask3 = LayerMask.GetMask("MoveStateBlockedArea", "Customer");
			Collider[] array2 = Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask3);
			bool flag2 = true;
			if (m_PlaceObjectInShopOnly)
			{
				int mask4 = LayerMask.GetMask("MoveStateValidArea");
				if (Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask4).Length == 0)
				{
					flag2 = false;
				}
			}
			else if (m_PlaceObjectInWarehouseOnly)
			{
				int mask5 = LayerMask.GetMask("MoveStateValidWarehouseArea");
				if (Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask5).Length == 0)
				{
					flag2 = false;
				}
			}
			if (array2.Length != 0 || base.transform.position.y > 0.1f)
			{
				flag2 = false;
			}
			if (m_DecoObjectType != EDecoObject.None && !flag2)
			{
				int mask6 = LayerMask.GetMask("DecorationMoveStateValidArea");
				if (Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask6).Length != 0 && base.transform.position.y <= 0.1f)
				{
					int mask7 = LayerMask.GetMask("DecorationMoveStateBlockedAreaB", "MoveStateBlockedArea");
					flag2 = Physics.OverlapBox(m_MoveStateValidArea.position, m_MoveStateValidArea.lossyScale / 2f, m_MoveStateValidArea.rotation, mask7).Length == 0;
				}
				else
				{
					flag2 = false;
				}
			}
			if (m_IsMovingObjectValidState != flag2)
			{
				m_IsMovingObjectValidState = flag2;
				ShelfManager.SetMoveObjectPreviewModelValidState(m_IsMovingObjectValidState);
			}
		}
	}

	protected virtual void LateUpdate()
	{
	}

	protected virtual void EvaluateLerpPosOffset()
	{
	}

	public virtual void StartMoveObject()
	{
		if (m_CanPickupMoveObject)
		{
			CSingleton<InteractionPlayerController>.Instance.OnEnterMoveObjectMode(m_IsDecorationVertical);
			OnRaycastEnded();
			m_IsMovingObject = true;
			m_IsMovingObjectValidState = false;
			m_OriginalLayer = base.gameObject.layer;
			m_LastHitTransform = null;
			if ((bool)m_DecoPlaccedLockedRoomBlocker)
			{
				m_IsVerticalSnapToWarehouseWall = false;
				m_VerticalSnapWallIndex = -1;
				m_DecoPlaccedLockedRoomBlocker.RemoveFromVerticalDecoObjectList(this);
			}
			m_DecoPlaccedLockedRoomBlocker = null;
			base.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			m_BoxCollider.enabled = false;
			for (int i = 0; i < m_BoxColliderList.Count; i++)
			{
				m_BoxColliderList[i].enabled = false;
			}
			m_MoveStateValidArea.gameObject.SetActive(value: false);
			ShelfManager.ActivateMoveObjectPreviewMode(base.transform, m_PickupObjectMesh, m_MoveStateValidArea);
			ShelfManager.SetMoveObjectPreviewModelValidState(isValid: false);
			if ((bool)m_BoxCollider)
			{
				CSingleton<ShelfManager>.Instance.m_MoveObjectCustomerBlocker.transform.position = base.transform.position;
				CSingleton<ShelfManager>.Instance.m_MoveObjectCustomerBlocker.transform.rotation = base.transform.rotation;
				CSingleton<ShelfManager>.Instance.m_MoveObjectCustomerBlocker.transform.localScale = m_MoveStateValidArea.transform.lossyScale + Vector3.one * 0.1f;
				CSingleton<ShelfManager>.Instance.m_MoveObjectCustomerBlocker.gameObject.SetActive(value: true);
			}
			if (m_CanFlip)
			{
				InteractionPlayerController.AddToolTip(EGameAction.Flip);
			}
			OnStartMoveObject();
			SoundManager.PlayAudio("SFX_WhipSoft", 0.6f);
		}
	}

	protected virtual void OnStartMoveObject()
	{
	}

	public void PlaceMovedObject()
	{
		if (m_IsMovingObjectValidState)
		{
			m_IsBoxedUp = false;
			OnPlacedMovedObject();
			PlayPlaceMoveObjectSFX();
			if (m_CanFlip)
			{
				InteractionPlayerController.RemoveToolTip(EGameAction.Flip);
			}
		}
	}

	public void Flip()
	{
		if (m_CanFlip)
		{
			SoundManager.GenericConfirm();
			m_IsFlipped = !m_IsFlipped;
			if (!m_IsDecorationVertical)
			{
				base.transform.Rotate(0f, 180f, 0f);
			}
		}
	}

	protected virtual void PlayPlaceMoveObjectSFX()
	{
		SoundManager.PlayAudio("SFX_PlaceShelf", 0.5f);
	}

	protected virtual void OnPlacedMovedObject()
	{
		m_IsSnappingPos = false;
		CSingleton<InteractionPlayerController>.Instance.OnExitMoveObjectMode();
		m_IsMovingObject = false;
		base.gameObject.layer = m_OriginalLayer;
		m_BoxCollider.enabled = true;
		for (int i = 0; i < m_BoxColliderList.Count; i++)
		{
			m_BoxColliderList[i].enabled = true;
		}
		if (!m_IsDecorationVertical)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			base.transform.position = position;
		}
		else
		{
			m_DecoPlaccedLockedRoomBlocker = null;
			if ((bool)m_LastHitTransform)
			{
				DecoBlocker component = m_LastHitTransform.GetComponent<DecoBlocker>();
				if ((bool)component)
				{
					m_DecoPlaccedLockedRoomBlocker = component.m_LockedRoomBlocker;
					m_DecoPlaccedLockedRoomBlocker.AddToVerticalDecoObjectList(this);
					if (m_DecoPlaccedLockedRoomBlocker.m_MainRoomIndex != -1)
					{
						m_IsVerticalSnapToWarehouseWall = false;
						m_VerticalSnapWallIndex = m_DecoPlaccedLockedRoomBlocker.m_MainRoomIndex;
					}
					else if (m_DecoPlaccedLockedRoomBlocker.m_StoreRoomIndex != -1)
					{
						m_IsVerticalSnapToWarehouseWall = true;
						m_VerticalSnapWallIndex = m_DecoPlaccedLockedRoomBlocker.m_StoreRoomIndex;
					}
				}
			}
		}
		m_MoveStateValidArea.gameObject.SetActive(value: true);
		ShelfManager.DisableMoveObjectPreviewMode();
		if ((bool)m_BoxCollider)
		{
			CSingleton<ShelfManager>.Instance.m_MoveObjectCustomerBlocker.gameObject.SetActive(value: false);
		}
		if (!m_IsBoxedUp && (bool)m_InteractablePackagingBox_Shelf)
		{
			if ((bool)m_Shelf_WorldUIGrp)
			{
				m_Shelf_WorldUIGrp.transform.position = base.transform.position;
			}
			m_InteractablePackagingBox_Shelf.EmptyBoxShelf();
			m_InteractablePackagingBox_Shelf.OnDestroyed();
		}
		if ((bool)m_Shelf_WorldUIGrp && !m_IsBoxedUp)
		{
			m_Shelf_WorldUIGrp.transform.position = base.transform.position;
			m_Shelf_WorldUIGrp.transform.rotation = base.transform.rotation;
		}
		if ((bool)m_NavMeshCut)
		{
			m_NavMeshCut.SetActive(value: false);
			m_NavMeshCut.SetActive(value: true);
		}
	}

	public virtual void BoxUpObject(bool holdBox)
	{
		if (m_DecoObjectType != EDecoObject.None)
		{
			OnPlacedMovedObject();
			CPlayerData.AddDecoItemToInventory(m_DecoObjectType, 1);
			SoundManager.PlayAudio("SFX_Throw", 0.3f);
			base.gameObject.SetActive(value: false);
			OnDestroyed();
			return;
		}
		OnPlacedMovedObject();
		m_IsBoxedUp = true;
		if (!m_InteractablePackagingBox_Shelf)
		{
			m_InteractablePackagingBox_Shelf = RestockManager.SpawnPackageBoxShelf(this, holdBox);
		}
		else
		{
			m_InteractablePackagingBox_Shelf.ExecuteBoxUpObject(this, holdBox);
		}
		if ((bool)m_Shelf_WorldUIGrp)
		{
			m_Shelf_WorldUIGrp.transform.position = Vector3.one * -10f;
		}
		if (holdBox)
		{
			SoundManager.PlayAudio("SFX_BoxClose", 0.5f);
		}
	}

	public void SetTargetMovePosition(Vector3 pos, Vector3 normal, Transform objHit)
	{
		m_TargetMoveObjectPosition = pos;
		m_HitNormal = normal;
		m_ObjHit = objHit;
		if (m_TargetMoveObjectPosition.y < 0f)
		{
			m_TargetMoveObjectPosition.y = 0f;
		}
	}

	public void SetMovePositionToCamera(float forwardDistnace = 3f)
	{
		m_TargetMoveObjectPosition = CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.position + CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.forward * forwardDistnace;
		m_TargetMoveObjectPosition += CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.up * -0.1f;
		if (m_TargetMoveObjectPosition.y < 0f)
		{
			m_TargetMoveObjectPosition.y = 0f;
		}
	}

	public void AddObjectRotation(float rotY, float snapAmount = 0f)
	{
		if (m_IsDecorationVertical)
		{
			base.transform.Rotate(0f, 0f, rotY);
			return;
		}
		base.transform.Rotate(0f, rotY, 0f);
		if (snapAmount > 0f)
		{
			Quaternion rotation = base.transform.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			float y = (float)Mathf.RoundToInt(rotation.eulerAngles.y / snapAmount) * snapAmount;
			eulerAngles.y = y;
			rotation.eulerAngles = eulerAngles;
			base.transform.rotation = rotation;
		}
	}

	public virtual void OnRaycasted()
	{
		if (!m_IsRaycasted)
		{
			if ((bool)m_Material)
			{
				m_Material.SetFloat("_Outline", m_HighlightOutlineWidth);
			}
			if ((bool)m_HighlightGameObj)
			{
				m_HighlightGameObj.SetActive(value: true);
			}
			m_IsRaycasted = true;
			ShowToolTip();
		}
	}

	protected virtual void ShowToolTip()
	{
		for (int i = 0; i < m_GameActionInputDisplayList.Count; i++)
		{
			InteractionPlayerController.AddToolTip(m_GameActionInputDisplayList[i]);
		}
		for (int j = 0; j < m_ControllerOnlyGameActionInputDisplayList.Count; j++)
		{
			InteractionPlayerController.RemoveToolTip(m_ControllerOnlyGameActionInputDisplayList[j]);
		}
		if (CSingleton<InputManager>.Instance.m_IsControllerActive)
		{
			for (int k = 0; k < m_ControllerOnlyGameActionInputDisplayList.Count; k++)
			{
				InteractionPlayerController.AddToolTip(m_ControllerOnlyGameActionInputDisplayList[k]);
			}
		}
	}

	public virtual void OnRaycastEnded()
	{
		if (m_IsRaycasted)
		{
			if ((bool)m_Material)
			{
				m_Material.SetFloat("_Outline", 0f);
			}
			if ((bool)m_HighlightGameObj)
			{
				m_HighlightGameObj.SetActive(value: false);
			}
			m_IsRaycasted = false;
		}
		for (int i = 0; i < m_GameActionInputDisplayList.Count; i++)
		{
			InteractionPlayerController.RemoveToolTip(m_GameActionInputDisplayList[i]);
		}
		for (int j = 0; j < m_ControllerOnlyGameActionInputDisplayList.Count; j++)
		{
			InteractionPlayerController.RemoveToolTip(m_ControllerOnlyGameActionInputDisplayList[j]);
		}
	}

	public virtual void OnDestroyed()
	{
		for (int i = 0; i < m_ItemCompartmentList.Count; i++)
		{
			if ((bool)m_ItemCompartmentList[i])
			{
				m_ItemCompartmentList[i].DisableAllItem();
			}
		}
		if (m_IsGenericObject && m_ObjectType != EObjectType.None)
		{
			ShelfManager.RemoveInteractableObject(this);
		}
		else if (m_IsGenericObject && m_DecoObjectType != EDecoObject.None)
		{
			ShelfManager.RemoveDecoObject(this);
		}
		if ((bool)m_DecoPlaccedLockedRoomBlocker)
		{
			m_IsVerticalSnapToWarehouseWall = false;
			m_VerticalSnapWallIndex = -1;
			m_DecoPlaccedLockedRoomBlocker.RemoveFromVerticalDecoObjectList(this);
		}
		if ((bool)m_Shelf_WorldUIGrp)
		{
			Object.Destroy(m_Shelf_WorldUIGrp.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	public virtual void OnMouseButtonUp()
	{
	}

	public virtual void OnRightMouseButtonUp()
	{
	}

	public virtual void OnPressEsc()
	{
	}

	public virtual void OnPressSpaceBar()
	{
	}

	public void LerpToTransform(Transform targetTransform, Transform targetParent, float lerpSpeed = 3f)
	{
		m_LerpPosSpeed = lerpSpeed;
		m_LerpPosTimer = 0f;
		base.transform.parent = targetParent;
		m_StartLerpPos = base.transform.position;
		m_StartLerpRot = base.transform.rotation;
		m_StartLerpScale = base.transform.localScale;
		m_TargetLerpTransform = targetTransform;
		m_IsLerpingToPos = true;
		OnStartLerp();
		if (!base.gameObject.activeInHierarchy)
		{
			OnDisable();
		}
	}

	public void StopLerpToTransform()
	{
		m_LerpPosTimer = 0f;
		m_IsLerpingToPos = false;
		m_IsHideAfterFinishLerp = false;
		m_LerpPosOffset = Vector3.zero;
		OnStopLerp();
	}

	public void SetHideItemAfterFinishLerp()
	{
		m_IsHideAfterFinishLerp = true;
	}

	protected virtual void OnStartLerp()
	{
	}

	protected virtual void OnStopLerp()
	{
	}

	protected virtual void OnFinishLerp()
	{
		m_LerpPosOffset = Vector3.zero;
	}

	protected void OnDisable()
	{
		if (m_IsLerpingToPos)
		{
			m_LerpPosTimer = 0f;
			m_IsLerpingToPos = false;
			base.transform.position = m_TargetLerpTransform.position;
			base.transform.rotation = m_TargetLerpTransform.rotation;
			base.transform.localScale = m_TargetLerpTransform.localScale;
			OnFinishLerp();
			if (m_IsHideAfterFinishLerp)
			{
				m_IsHideAfterFinishLerp = false;
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void OnLockedRoomHidden()
	{
		if (m_ObjectType != EObjectType.None)
		{
			BoxUpObject(holdBox: false);
			m_LastHitTransform = null;
			m_IsVerticalSnapToWarehouseWall = false;
			m_VerticalSnapWallIndex = -1;
		}
		else
		{
			CPlayerData.AddDecoItemToInventory(m_DecoObjectType, 1);
			m_LastHitTransform = null;
			m_IsVerticalSnapToWarehouseWall = false;
			m_VerticalSnapWallIndex = -1;
			base.gameObject.SetActive(value: false);
			OnDestroyed();
		}
	}

	private void EvaluateOcclusionCulling()
	{
		if (GameInstance.m_HasFinishHideLoadingScreen && (bool)m_CullingCheckMesh)
		{
			float num = 25f;
			if (CSingleton<CGameManager>.Instance.m_QualitySettingIndex == 0)
			{
				num = 27.5f;
			}
			else if (CSingleton<CGameManager>.Instance.m_QualitySettingIndex == 1)
			{
				num = 22.5f;
			}
			else if (CSingleton<CGameManager>.Instance.m_QualitySettingIndex == 2)
			{
				num = 17.5f;
			}
			float magnitude = (m_CullingCheckMesh.transform.position - CSingleton<InteractionPlayerController>.Instance.m_Cam.transform.position).magnitude;
			bool flag = false;
			if (magnitude > num)
			{
				flag = true;
			}
			m_CullingCheckMesh.shadowCastingMode = ShadowCastingMode.Off;
			for (int i = 0; i < m_ItemCompartmentList.Count; i++)
			{
				m_ItemCompartmentList[i].m_StoredItemListGrp.gameObject.SetActive(m_CullingCheckMesh.isVisible && !flag);
			}
		}
	}

	public bool CanPickup()
	{
		return !m_IsLerpingToPos;
	}

	public virtual bool IsValidObject()
	{
		if (!m_IsMovingObject && !m_IsBoxedUp)
		{
			return !m_IsBeingHold;
		}
		return false;
	}

	public bool GetIsBoxedUp()
	{
		return m_IsBoxedUp;
	}

	public bool GetIsMovingObject()
	{
		return m_IsMovingObject;
	}

	public InteractablePackagingBox_Shelf GetPackagingBoxShelf()
	{
		return m_InteractablePackagingBox_Shelf;
	}

	public bool GetIsVerticalSnapToWarehouseWall()
	{
		return m_IsVerticalSnapToWarehouseWall;
	}

	public int GetVerticalSnapWallIndex()
	{
		return m_VerticalSnapWallIndex;
	}

	public void SetVerticalSnapToWarehouseWall(bool isVerticalSnapToWarehouseWall, int verticalSnapWallIndex)
	{
		m_IsVerticalSnapToWarehouseWall = isVerticalSnapToWarehouseWall;
		m_VerticalSnapWallIndex = verticalSnapWallIndex;
		if (m_VerticalSnapWallIndex != -1)
		{
			if (!m_IsVerticalSnapToWarehouseWall)
			{
				m_DecoPlaccedLockedRoomBlocker = CSingleton<UnlockRoomManager>.Instance.m_LockedRoomBlockerList[m_VerticalSnapWallIndex];
			}
			else if (m_IsVerticalSnapToWarehouseWall)
			{
				m_DecoPlaccedLockedRoomBlocker = CSingleton<UnlockRoomManager>.Instance.m_LockedWarehouseRoomBlockerList[m_VerticalSnapWallIndex];
			}
			m_DecoPlaccedLockedRoomBlocker.AddToVerticalDecoObjectList(this);
		}
	}

	public List<Transform> GetGamepadQuickSelectTransformList(bool isPriceTagOnly = false)
	{
		if (isPriceTagOnly)
		{
			return m_GamepadQuickSelectPriceTagTransformList;
		}
		return m_GamepadQuickSelectTransformList;
	}
}
