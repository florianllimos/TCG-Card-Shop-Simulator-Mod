using UnityEngine;

public class ShopCustomizationManager : CSingleton<ShopCustomizationManager>
{
	public static ShopCustomizationManager m_Instance;

	private bool m_FinishLoading;

	public Material m_FloorMat;

	public Material m_FloorMatB;

	public Material m_WallMat;

	public Material m_WallMatB;

	public Material m_WallBarMat;

	public Material m_WallBarMatB;

	public Material m_CeilingMat;

	public Material m_CeilingMatB;

	public GameObject m_ShopWall_WithBar;

	public GameObject m_ShopWall_NoBar;

	public GameObject m_ShopWall_WithBarB;

	public GameObject m_ShopWall_NoBarB;

	private void Init()
	{
		if (!m_FinishLoading)
		{
			m_FinishLoading = true;
			ChangeFloorMaterial(CPlayerData.m_EquippedFloorDecoIndex, isShopLotB: false);
			ChangeWallMaterial(CPlayerData.m_EquippedWallDecoIndex, isShopLotB: false);
			ChangeCeilingMaterial(CPlayerData.m_EquippedCeilingDecoIndex, isShopLotB: false);
			ChangeFloorMaterial(CPlayerData.m_EquippedFloorDecoIndexB, isShopLotB: true);
			ChangeWallMaterial(CPlayerData.m_EquippedWallDecoIndexB, isShopLotB: true);
			ChangeCeilingMaterial(CPlayerData.m_EquippedCeilingDecoIndexB, isShopLotB: true);
		}
	}

	private void Start()
	{
		ChangeFloorMaterial(0, isShopLotB: false);
		ChangeWallMaterial(0, isShopLotB: false);
		ChangeCeilingMaterial(0, isShopLotB: false);
		ChangeFloorMaterial(0, isShopLotB: true);
		ChangeWallMaterial(0, isShopLotB: true);
		ChangeCeilingMaterial(0, isShopLotB: true);
	}

	public static void ChangeFloorMaterial(int index, bool isShopLotB)
	{
		ShopDecoData floorDecoData = InventoryBase.GetFloorDecoData(index);
		if (isShopLotB)
		{
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMatB.SetTexture("_MainTex", floorDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMatB.SetTexture("_MetallicGlossMap", floorDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMatB.SetTexture("_BumpMap", floorDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMatB.SetFloat("_GlossMapScale", floorDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMatB.SetColor("_Color", floorDecoData.color);
		}
		else
		{
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMat.SetTexture("_MainTex", floorDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMat.SetTexture("_MetallicGlossMap", floorDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMat.SetTexture("_BumpMap", floorDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMat.SetFloat("_GlossMapScale", floorDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_FloorMat.SetColor("_Color", floorDecoData.color);
		}
	}

	public static void ChangeWallMaterial(int index, bool isShopLotB)
	{
		ShopDecoData wallDecoData = InventoryBase.GetWallDecoData(index);
		if (isShopLotB)
		{
			CSingleton<ShopCustomizationManager>.Instance.m_WallMatB.SetTexture("_MainTex", wallDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMatB.SetTexture("_MetallicGlossMap", wallDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMatB.SetTexture("_BumpMap", wallDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMatB.SetFloat("_GlossMapScale", wallDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMatB.SetColor("_Color", wallDecoData.color);
		}
		else
		{
			CSingleton<ShopCustomizationManager>.Instance.m_WallMat.SetTexture("_MainTex", wallDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMat.SetTexture("_MetallicGlossMap", wallDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMat.SetTexture("_BumpMap", wallDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMat.SetFloat("_GlossMapScale", wallDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_WallMat.SetColor("_Color", wallDecoData.color);
		}
		if (wallDecoData.showBar)
		{
			ShopDecoData wallBarDecoData = InventoryBase.GetWallBarDecoData(index);
			if (isShopLotB)
			{
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMatB.SetTexture("_MainTex", wallBarDecoData.mainTexture);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMatB.SetTexture("_MetallicGlossMap", wallBarDecoData.roughnessMap);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMatB.SetTexture("_BumpMap", wallBarDecoData.normalMap);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMatB.SetFloat("_GlossMapScale", wallBarDecoData.smoothness);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMatB.SetColor("_Color", wallBarDecoData.color);
			}
			else
			{
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMat.SetTexture("_MainTex", wallBarDecoData.mainTexture);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMat.SetTexture("_MetallicGlossMap", wallBarDecoData.roughnessMap);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMat.SetTexture("_BumpMap", wallBarDecoData.normalMap);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMat.SetFloat("_GlossMapScale", wallBarDecoData.smoothness);
				CSingleton<ShopCustomizationManager>.Instance.m_WallBarMat.SetColor("_Color", wallBarDecoData.color);
			}
			if (isShopLotB)
			{
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_WithBarB.SetActive(value: true);
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_NoBarB.SetActive(value: false);
			}
			else
			{
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_WithBar.SetActive(value: true);
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_NoBar.SetActive(value: false);
			}
			CSingleton<UnlockRoomManager>.Instance.SetWallBarVisibility(isVisible: true, isShopLotB);
		}
		else
		{
			if (isShopLotB)
			{
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_WithBarB.SetActive(value: false);
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_NoBarB.SetActive(value: true);
			}
			else
			{
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_WithBar.SetActive(value: false);
				CSingleton<ShopCustomizationManager>.Instance.m_ShopWall_NoBar.SetActive(value: true);
			}
			CSingleton<UnlockRoomManager>.Instance.SetWallBarVisibility(isVisible: false, isShopLotB);
		}
	}

	public static void ChangeCeilingMaterial(int index, bool isShopLotB)
	{
		ShopDecoData ceilingDecoData = InventoryBase.GetCeilingDecoData(index);
		if (isShopLotB)
		{
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMatB.SetTexture("_MainTex", ceilingDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMatB.SetTexture("_MetallicGlossMap", ceilingDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMatB.SetTexture("_BumpMap", ceilingDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMatB.SetFloat("_GlossMapScale", ceilingDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMatB.SetColor("_Color", ceilingDecoData.color);
		}
		else
		{
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMat.SetTexture("_MainTex", ceilingDecoData.mainTexture);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMat.SetTexture("_MetallicGlossMap", ceilingDecoData.roughnessMap);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMat.SetTexture("_BumpMap", ceilingDecoData.normalMap);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMat.SetFloat("_GlossMapScale", ceilingDecoData.smoothness);
			CSingleton<ShopCustomizationManager>.Instance.m_CeilingMat.SetColor("_Color", ceilingDecoData.color);
		}
	}

	protected void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_GameDataFinishLoaded>(OnGameDataFinishLoaded);
		}
	}

	protected void OnGameDataFinishLoaded(CEventPlayer_GameDataFinishLoaded evt)
	{
		Init();
	}
}
