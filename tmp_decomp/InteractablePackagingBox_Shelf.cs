using System.Collections;
using UnityEngine;

public class InteractablePackagingBox_Shelf : InteractablePackagingBox
{
	public ShelfCompartment m_ItemCompartment;

	public GameObject m_StaticMeshGrp;

	public GameObject m_RigMeshGrp;

	public GameObject m_ClosedBox;

	public GameObject m_OpenBox;

	private InteractableObject m_BoxedObject;

	private bool m_IsBoxOpened;

	private bool m_IsPriceTagSpawned;

	private bool m_IsOpeningBox;

	private Coroutine m_DelayResetOpenBox;

	protected override void Awake()
	{
		base.Awake();
		RestockManager.InitShelfPackageBox(this);
	}

	protected override void Update()
	{
		base.Update();
		if (m_IsOpeningBox)
		{
			base.transform.position += Vector3.up * Time.deltaTime * -1.2f;
		}
	}

	public void ExecuteBoxUpObject(InteractableObject obj, bool holdBox)
	{
		if (m_DelayResetOpenBox != null)
		{
			StopCoroutine(m_DelayResetOpenBox);
			m_IsOpeningBox = false;
			m_DelayResetOpenBox = null;
		}
		base.transform.position = obj.transform.position;
		base.transform.rotation = obj.transform.rotation;
		m_BoxAnim.Play("Close");
		base.gameObject.SetActive(value: true);
		m_BoxedObject = obj;
		m_BoxedObject.gameObject.SetActive(value: false);
		SpawnPriceTag();
		if (holdBox)
		{
			m_IsBeingHold = false;
			StartHoldBox(isPlayer: true, CSingleton<InteractionPlayerController>.Instance.m_HoldBigItemPos);
			SetPriceTagVisibility(isVisible: false);
			StartCoroutine(DelaySetPriceTagVisibility(0.85f, isVisible: true));
		}
	}

	public override void StartHoldBox(bool isPlayer, Transform holdItemPos)
	{
		if (!m_IsBeingHold)
		{
			base.StartHoldBox(isPlayer, CSingleton<InteractionPlayerController>.Instance.m_HoldBigItemPos);
			_ = m_IsBoxOpened;
			if (isPlayer)
			{
				InteractionPlayerController.RemoveToolTip(EGameAction.OpenBox);
				InteractionPlayerController.AddToolTip(EGameAction.OpenBox);
				InteractionPlayerController.RemoveToolTip(EGameAction.SellBoxedUpShelf);
				InteractionPlayerController.AddToolTip(EGameAction.SellBoxedUpShelf);
				m_ItemCompartment.SetIgnoreCull(ignoreCull: true);
			}
		}
	}

	public override void ThrowBox(bool isPlayer)
	{
		base.ThrowBox(isPlayer);
		m_ItemCompartment.SetIgnoreCull(ignoreCull: false);
	}

	public override void DropBox(bool isPlayer)
	{
		base.DropBox(isPlayer);
		m_ItemCompartment.SetIgnoreCull(ignoreCull: false);
	}

	protected override void SpawnPriceTag()
	{
		if (!m_IsPriceTagSpawned)
		{
			m_IsPriceTagSpawned = true;
			base.SpawnPriceTag();
			for (int i = 0; i < m_ItemCompartment.m_InteractablePriceTagList.Count; i++)
			{
				UI_PriceTag uI_PriceTag = PriceTagUISpawner.SpawnPriceTagPackageBoxWorldUIGrp(m_Shelf_WorldUIGrp, m_ItemCompartment.m_InteractablePriceTagList[i].transform);
				uI_PriceTag.SetObjectImage(m_BoxedObject.m_ObjectType);
				m_ItemCompartment.m_InteractablePriceTagList[i].SetPriceTagUI(uI_PriceTag);
				m_ItemCompartment.m_InteractablePriceTagList[i].SetVisibility(isVisible: true);
			}
		}
	}

	public override void OnPressOpenBox()
	{
		if (!m_IsOpeningBox)
		{
			m_IsOpeningBox = true;
			m_DelayResetOpenBox = StartCoroutine(DelayResetOpenBox());
			base.OnPressOpenBox();
			if ((bool)m_BoxedObject)
			{
				m_BoxAnim.Play("Open");
				CSingleton<InteractionPlayerController>.Instance.OnExitHoldBoxMode();
				InteractionPlayerController.SetCurrentInteractableObject(m_BoxedObject);
				m_BoxedObject.transform.position = base.transform.position;
				m_BoxedObject.gameObject.SetActive(value: true);
				m_BoxedObject.StartMoveObject();
				SetPriceTagVisibility(isVisible: false);
				SoundManager.PlayAudio("SFX_BoxOpen", 0.5f);
			}
		}
	}

	protected IEnumerator DelayResetOpenBox()
	{
		yield return new WaitForSeconds(0.85f);
		m_IsOpeningBox = false;
		base.gameObject.SetActive(value: false);
	}

	public void SetOpenCloseBox(bool isOpen)
	{
		m_IsBoxOpened = isOpen;
		if (!m_IsBoxOpened)
		{
			m_OpenBox.SetActive(value: false);
			m_ClosedBox.SetActive(value: true);
			SetPriceTagVisibility(isVisible: true);
		}
		else
		{
			m_OpenBox.SetActive(value: true);
			m_ClosedBox.SetActive(value: false);
			SetPriceTagVisibility(isVisible: false);
		}
	}

	public void EmptyBoxShelf()
	{
		m_BoxedObject = null;
	}

	public override void OnDestroyed()
	{
		if ((bool)m_BoxedObject)
		{
			m_BoxedObject.gameObject.SetActive(value: false);
			m_BoxedObject.OnDestroyed();
		}
		RestockManager.RemoveShelfPackageBox(this);
		base.OnDestroyed();
	}

	protected override void SetPriceTagVisibility(bool isVisible)
	{
		base.SetPriceTagVisibility(isVisible);
		for (int i = 0; i < m_ItemCompartment.m_InteractablePriceTagList.Count; i++)
		{
			m_ItemCompartment.m_InteractablePriceTagList[i].SetVisibility(isVisible);
		}
	}

	public EObjectType GetBoxedObjectType()
	{
		return m_BoxedObject.m_ObjectType;
	}
}
