using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public MeshFilter m_MeshFilter;

	public MeshRenderer m_Mesh;

	public MeshFilter m_MeshFilterSecondary;

	public MeshRenderer m_MeshSecondary;

	public MeshFilter m_OutlineMeshFilter;

	public BoxCollider m_Collider;

	public Rigidbody m_Rigidbody;

	public InteractableScanItem m_InteractableScanItem;

	private bool m_IsLerpingToPos;

	private bool m_IsSmoothLerpingToPos;

	private bool m_IsHideAfterFinishLerp;

	private bool m_IsIgnoreUpForce;

	private float m_ItemVolume;

	private float m_Timer;

	private float m_UpTimer;

	private float m_LerpSpeed = 3f;

	private float m_Accelration;

	private float m_UpLerpSpeed = 5f;

	private float m_UpLerpHeight = 0.1f;

	private float m_CurrentPrice;

	private float m_ItemContentFill = 1f;

	private Vector3 m_StartPos;

	private Quaternion m_StartRot;

	private Vector3 m_StartScale;

	private Transform m_TargetTransform;

	private EItemType m_ItemType;

	private void Awake()
	{
		base.enabled = false;
	}

	public void SetMesh(Mesh mesh, Material material, EItemType itemType, Mesh meshSecondary = null, Material materialSecondary = null, List<Material> materialList = null)
	{
		m_ItemType = itemType;
		m_MeshFilter.mesh = mesh;
		switch (itemType)
		{
		case EItemType.BasicCardPack:
		case EItemType.RareCardPack:
		case EItemType.EpicCardPack:
		case EItemType.LegendaryCardPack:
		case EItemType.DestinyBasicCardPack:
		case EItemType.DestinyRareCardPack:
		case EItemType.DestinyEpicCardPack:
		case EItemType.DestinyLegendaryCardPack:
			m_MeshFilterSecondary.mesh = meshSecondary;
			m_Mesh.sharedMaterial = material;
			m_MeshSecondary.material = materialSecondary;
			break;
		default:
			if (materialList != null && materialList.Count > 0)
			{
				Material[] array = new Material[materialList.Count];
				for (int i = 0; i < materialList.Count; i++)
				{
					array[i] = materialList[i];
				}
				m_Mesh.materials = array;
			}
			else if (meshSecondary == null && materialSecondary == null && m_Mesh.materials.Length > 1)
			{
				Material[] materials = new Material[1] { material };
				m_Mesh.materials = materials;
			}
			else if (meshSecondary != null && m_Mesh.materials.Length > 1)
			{
				Material[] materials2 = new Material[1] { material };
				m_Mesh.materials = materials2;
			}
			else
			{
				m_Mesh.material = material;
			}
			m_MeshFilterSecondary.gameObject.SetActive(value: false);
			if (meshSecondary != null)
			{
				m_MeshFilterSecondary.mesh = meshSecondary;
				m_MeshSecondary.material = materialSecondary;
				m_MeshFilterSecondary.gameObject.SetActive(value: true);
			}
			else if (meshSecondary == null && materialSecondary != null)
			{
				Material[] materials3 = new Material[2] { material, materialSecondary };
				m_Mesh.materials = materials3;
			}
			break;
		}
		m_OutlineMeshFilter.mesh = mesh;
		m_Collider.size = InventoryBase.GetItemData(itemType).colliderScale;
		m_Collider.center = InventoryBase.GetItemData(itemType).colliderPosOffset;
		m_ItemVolume = InventoryBase.GetItemData(itemType).GetItemVolume();
		m_InteractableScanItem.enabled = false;
		m_ItemContentFill = 1f;
	}

	public void LerpToTransform(Transform targetTransform, Transform targetParent, bool ignoreUpForce = false)
	{
		m_Timer = 0f;
		m_UpTimer = 0f;
		m_Accelration = 0f;
		base.transform.parent = targetParent;
		m_StartPos = base.transform.position;
		m_StartRot = base.transform.rotation;
		m_StartScale = base.transform.localScale;
		m_TargetTransform = targetTransform;
		m_IsLerpingToPos = true;
		m_IsSmoothLerpingToPos = false;
		m_IsIgnoreUpForce = ignoreUpForce;
		m_Mesh.enabled = true;
		base.enabled = true;
	}

	public void SmoothLerpToTransform(Transform targetTransform, Transform targetParent, bool ignoreUpForce = false)
	{
		m_Timer = 0f;
		m_UpTimer = 0f;
		m_Accelration = 0f;
		base.transform.parent = targetParent;
		m_StartPos = base.transform.position;
		m_StartRot = base.transform.rotation;
		m_StartScale = base.transform.localScale;
		m_TargetTransform = targetTransform;
		m_IsLerpingToPos = false;
		m_IsSmoothLerpingToPos = true;
		m_IsIgnoreUpForce = ignoreUpForce;
		m_Mesh.enabled = true;
		base.enabled = true;
	}

	private void Update()
	{
		if (m_IsLerpingToPos)
		{
			m_UpTimer += Time.deltaTime * m_UpLerpSpeed;
			if (m_UpTimer > 0.2f)
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * (1f + m_Accelration);
				m_Accelration += Time.deltaTime;
			}
			else
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * 0.1f;
			}
			Vector3 vector = Vector3.up * (Mathf.PingPong(Mathf.Clamp(m_UpTimer, 0f, 2f), 1f) * m_UpLerpHeight);
			if (m_IsIgnoreUpForce)
			{
				vector = Vector3.zero;
				m_UpTimer = 2f;
			}
			base.transform.position = Vector3.Lerp(m_StartPos, m_TargetTransform.position + vector, m_Timer) + vector;
			base.transform.rotation = Quaternion.Lerp(m_StartRot, m_TargetTransform.rotation, m_Timer);
			base.transform.localScale = Vector3.Lerp(m_StartScale, m_TargetTransform.localScale, m_Timer);
			if (m_Timer >= 1f && m_UpTimer >= 2f)
			{
				m_Timer = 0f;
				m_UpTimer = 0f;
				m_Accelration = 0f;
				m_IsLerpingToPos = false;
				base.enabled = false;
				if (m_IsHideAfterFinishLerp)
				{
					m_IsHideAfterFinishLerp = false;
					base.gameObject.SetActive(value: false);
				}
			}
		}
		else if (m_IsSmoothLerpingToPos)
		{
			m_UpTimer += Time.deltaTime * m_UpLerpSpeed * 0.75f;
			Vector3 vector2 = Vector3.up * (Mathf.PingPong(Mathf.Clamp(m_UpTimer, 0f, 2f), 1f) * m_UpLerpHeight);
			if (m_UpTimer > 0.2f)
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * (1f + m_Accelration);
				m_Accelration += Time.deltaTime;
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetTransform.position + vector2, Time.deltaTime * 10f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetTransform.rotation, Time.deltaTime * 10f);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, m_TargetTransform.localScale, Time.deltaTime * 10f);
			}
			else
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * 0.1f;
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetTransform.position + vector2, Time.deltaTime * 2f) + vector2;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetTransform.rotation, Time.deltaTime * 2f);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, m_TargetTransform.localScale, Time.deltaTime * 2f);
			}
			if (m_IsIgnoreUpForce)
			{
				vector2 = Vector3.zero;
				m_UpTimer = 2f;
			}
		}
	}

	public void DepleteContent(float amount)
	{
		m_ItemContentFill -= amount;
		if (m_ItemContentFill <= 0f)
		{
			m_ItemContentFill = 0f;
		}
	}

	public float GetContentFill()
	{
		return m_ItemContentFill;
	}

	public void SetContentFill(float itemContentFill)
	{
		m_ItemContentFill = itemContentFill;
	}

	public void SetHideItemAfterFinishLerp()
	{
		m_IsHideAfterFinishLerp = true;
	}

	public void SetCurrentPrice(float price)
	{
		m_CurrentPrice = price;
	}

	public float GetCurrentPrice()
	{
		return m_CurrentPrice;
	}

	public float GetItemVolume()
	{
		return m_ItemVolume;
	}

	public EItemType GetItemType()
	{
		return m_ItemType;
	}

	public void DisableItem()
	{
		m_TargetTransform = null;
		m_IsLerpingToPos = false;
		m_IsSmoothLerpingToPos = false;
		base.enabled = false;
		ItemSpawnManager.DisableItem(this);
	}
}
