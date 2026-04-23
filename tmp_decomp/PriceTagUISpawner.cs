using System.Collections.Generic;
using UnityEngine;

public class PriceTagUISpawner : CSingleton<PriceTagUISpawner>
{
	public static PriceTagUISpawner m_Instance;

	public Transform m_Shelf_WorldUIGrpPrefab;

	public UI_PriceTag m_UIPriceTagPrefab;

	public UI_PriceTag m_UIPriceTagItemBoxPrefab;

	public UI_PriceTag m_UIPriceTagPackageBoxPrefab;

	public UI_PriceTag m_UIPriceTagWarehouseRackPrefab;

	public UI_PriceTag m_UIPriceTagCardPrefab;

	private List<Transform> m_WorldUIGrpList = new List<Transform>();

	private List<UI_PriceTag> m_UI_PriceTagList = new List<UI_PriceTag>();

	private int m_CullIndex;

	private int m_CullLoopCount;

	private int m_CullLoopCountMaxPerFrame = 100;

	private float m_DotCullLimit = 0.65f;

	private float m_AngleCullLimit = 110f;

	private float m_CullTimer;

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
		}
		else if (m_Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		List<UI_PriceTag> uI_PriceTagList = m_UI_PriceTagList;
		int count = uI_PriceTagList.Count;
		InteractionPlayerController interactionPlayerController = CSingleton<InteractionPlayerController>.Instance;
		Transform transform = interactionPlayerController.m_Cam.transform;
		Vector3 position = transform.position;
		Vector3 forward = transform.forward;
		Vector3 position2 = interactionPlayerController.m_WalkerCtrl.transform.position;
		m_CullLoopCount = 0;
		for (int i = 0; i < count; i++)
		{
			UI_PriceTag uI_PriceTag = uI_PriceTagList[m_CullIndex];
			if ((bool)uI_PriceTag && !uI_PriceTag.m_IgnoreCull)
			{
				Transform uIGrp = uI_PriceTag.m_UIGrp;
				Vector3 position3 = uIGrp.position;
				float num = Vector3.Dot((position3 - position).normalized, forward);
				float magnitude = (position3 - position2).magnitude;
				float num2 = Vector3.Angle(uIGrp.TransformDirection(Vector3.forward), transform.TransformDirection(Vector3.forward));
				bool flag = magnitude > 6f || (magnitude > 1f && num < m_DotCullLimit) || num2 > m_AngleCullLimit;
				uIGrp.gameObject.SetActive(!flag);
			}
			m_CullIndex++;
			if (m_CullIndex >= count)
			{
				m_CullIndex = 0;
			}
			m_CullLoopCount++;
			if (m_CullLoopCount >= m_CullLoopCountMaxPerFrame)
			{
				m_CullLoopCount = 0;
				break;
			}
		}
	}

	public static Transform SpawnShelfWorldUIGrp(Transform shelf)
	{
		Transform transform = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_Shelf_WorldUIGrpPrefab, shelf.position, shelf.rotation, CSingleton<PriceTagUISpawner>.Instance.transform);
		CSingleton<PriceTagUISpawner>.Instance.m_WorldUIGrpList.Add(transform);
		return transform;
	}

	public static UI_PriceTag SpawnPriceTagWorldUIGrp(Transform parent, Transform priceTagLoc)
	{
		UI_PriceTag uI_PriceTag = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_UIPriceTagPrefab, priceTagLoc.position, priceTagLoc.rotation, parent);
		uI_PriceTag.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
		CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Add(uI_PriceTag);
		return uI_PriceTag;
	}

	public static UI_PriceTag SpawnPriceTagItemBoxWorldUIGrp(Transform parent, Transform priceTagLoc)
	{
		UI_PriceTag uI_PriceTag = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_UIPriceTagItemBoxPrefab, priceTagLoc.position, priceTagLoc.rotation, parent);
		uI_PriceTag.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
		CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Add(uI_PriceTag);
		return uI_PriceTag;
	}

	public static UI_PriceTag SpawnPriceTagPackageBoxWorldUIGrp(Transform parent, Transform priceTagLoc)
	{
		UI_PriceTag uI_PriceTag = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_UIPriceTagPackageBoxPrefab, priceTagLoc.position, priceTagLoc.rotation, parent);
		uI_PriceTag.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
		CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Add(uI_PriceTag);
		return uI_PriceTag;
	}

	public static UI_PriceTag SpawnPriceTagWarehouseRakWorldUIGrp(Transform parent, Transform priceTagLoc)
	{
		UI_PriceTag uI_PriceTag = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_UIPriceTagWarehouseRackPrefab, priceTagLoc.position, priceTagLoc.rotation, parent);
		uI_PriceTag.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
		CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Add(uI_PriceTag);
		return uI_PriceTag;
	}

	public static UI_PriceTag SpawnPriceTagCardWorldUIGrp(Transform parent, Transform priceTagLoc)
	{
		UI_PriceTag uI_PriceTag = Object.Instantiate(CSingleton<PriceTagUISpawner>.Instance.m_UIPriceTagCardPrefab, priceTagLoc.position, priceTagLoc.rotation, parent);
		uI_PriceTag.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
		CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Add(uI_PriceTag);
		return uI_PriceTag;
	}

	public static void SetAllPriceTagUIBrightness(float brightness)
	{
		for (int i = 0; i < CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList.Count; i++)
		{
			if ((bool)CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList[i])
			{
				CSingleton<PriceTagUISpawner>.Instance.m_UI_PriceTagList[i].SetBrightness(brightness);
			}
		}
	}

	protected virtual void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected virtual void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_OnSettingUpdated>(OnSettingUpdated);
		}
	}

	protected void OnSettingUpdated(CEventPlayer_OnSettingUpdated evt)
	{
		m_DotCullLimit = Mathf.Lerp(0.75f, 0.35f, CSingleton<CGameManager>.Instance.m_CameraFOVSlider);
		m_AngleCullLimit = 110f + CSingleton<CGameManager>.Instance.m_CameraFOVSlider * 40f;
	}
}
