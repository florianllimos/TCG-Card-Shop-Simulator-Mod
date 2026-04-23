using UnityEngine;

public class InteractableCard3d : InteractableObject
{
	public bool m_IsCardAlbumCard;

	public CollectionBinderFlipAnimCtrl m_CollectionBinderFlipAnimCtrl;

	public Card3dUIGroup m_Card3dUI;

	public BoxCollider m_Collider;

	public Rigidbody m_Rigidbody;

	public bool m_IsCard3dUIFollow;

	private bool m_IsLerpingCardRot;

	private bool m_IsLerpingCardScale;

	private bool m_IsLerpingCardLocalPos;

	private bool m_IsDisplayedOnShelf;

	private bool m_PlayAnimAfterFinishLerp;

	private bool m_IsPlayAnimAfterTime;

	public bool m_IsEnableUpDownLerpMotion;

	public bool m_IsEnableLocalUpDownLerpMotion;

	private string m_FinishLerpPlayAnimName = "";

	private string m_TimePassedPlayAnimName = "";

	private float m_CurrentPrice;

	private float m_PlayAnimAfterTime;

	private float m_PlayAnimAfterTimer;

	private float m_MultiplierUpDownLerpMotion;

	private float m_MultiplierLocalUpDownLerpMotion;

	private Quaternion m_CardTargetLerpRot;

	private Vector3 m_CardTargetLerpScale;

	private Vector3 m_CardTargetLerpLocalPos;

	private Vector3 m_LerpUpDownOffset = Vector3.zero;

	private Vector3 m_LerpLocalUpDownOffset = Vector3.zero;

	private Customer m_CurrentCustomer;

	private Transform m_ScannedItemLerpPos;

	protected override void Start()
	{
		base.Start();
	}

	public override void OnMouseButtonUp()
	{
		base.OnMouseButtonUp();
		if ((bool)m_CurrentCustomer)
		{
			OnCardScanned();
		}
	}

	protected override void EvaluateLerpPosOffset()
	{
		if (m_IsEnableUpDownLerpMotion)
		{
			m_LerpUpDownOffset = Vector3.Lerp(Vector3.zero, Vector3.up * m_MultiplierUpDownLerpMotion, m_LerpPosTimer);
			if (m_LerpPosTimer > 0.5f)
			{
				m_LerpUpDownOffset = Vector3.Lerp(Vector3.zero, Vector3.up * m_MultiplierUpDownLerpMotion, 1f - m_LerpPosTimer);
			}
		}
		if (m_IsEnableLocalUpDownLerpMotion)
		{
			float t = 0f;
			if (m_LerpPosTimer > 0f)
			{
				t = Mathf.Abs(Mathf.Log(3f, m_LerpPosTimer));
			}
			m_LerpLocalUpDownOffset = Vector3.Lerp(Vector3.zero, base.transform.up * m_MultiplierLocalUpDownLerpMotion, t);
			if (m_LerpPosTimer > 0.5f)
			{
				m_LerpLocalUpDownOffset = Vector3.Lerp(t: (!(1f - m_LerpPosTimer <= 0f)) ? Mathf.Abs(Mathf.Log(3f, 1f - m_LerpPosTimer)) : 0f, a: Vector3.zero, b: base.transform.up * m_MultiplierLocalUpDownLerpMotion);
			}
		}
		m_LerpPosOffset = m_LerpUpDownOffset + m_LerpLocalUpDownOffset;
	}

	public void SetUpDownMotionToLerp(bool isLocal, float multiplier)
	{
		if (isLocal)
		{
			m_MultiplierLocalUpDownLerpMotion = multiplier;
			m_IsEnableLocalUpDownLerpMotion = true;
		}
		else
		{
			m_MultiplierUpDownLerpMotion = multiplier;
			m_IsEnableUpDownLerpMotion = true;
		}
	}

	public void SetCardUIFollow(Card3dUIGroup card3dUI)
	{
		if ((bool)card3dUI)
		{
			m_IsCard3dUIFollow = true;
			m_Card3dUI = card3dUI;
		}
		else
		{
			m_IsCard3dUIFollow = false;
			m_Card3dUI = null;
		}
	}

	public void SetTargetRotation(Quaternion targetRot)
	{
		m_IsLerpingCardRot = true;
		m_CardTargetLerpRot = targetRot;
	}

	public void SetTargetLocalScale(Vector3 targetScale)
	{
		m_IsLerpingCardScale = true;
		m_CardTargetLerpScale = targetScale;
	}

	public void SetTargetLocalPos(Vector3 targetPos)
	{
		m_IsLerpingCardLocalPos = true;
		m_CardTargetLerpLocalPos = targetPos;
	}

	public override void OnRaycasted()
	{
		if (!m_IsLerpingToPos)
		{
			base.OnRaycasted();
			if ((bool)m_CollectionBinderFlipAnimCtrl)
			{
				m_CollectionBinderFlipAnimCtrl.OnCardRaycasted(this);
			}
		}
	}

	public override void OnRaycastEnded()
	{
		if (!m_IsLerpingToPos)
		{
			base.OnRaycastEnded();
			if ((bool)m_CollectionBinderFlipAnimCtrl)
			{
				m_CollectionBinderFlipAnimCtrl.OnCardRaycastEnded();
			}
		}
	}

	public void SetEnableCollision(bool isEnable)
	{
		m_BoxCollider.enabled = isEnable;
	}

	protected override void LateUpdate()
	{
		if (m_IsCard3dUIFollow)
		{
			m_Card3dUI.transform.position = base.transform.position;
			m_Card3dUI.transform.rotation = base.transform.rotation;
		}
		if (m_IsLerpingCardRot)
		{
			m_Card3dUI.m_ScaleGrp.localRotation = Quaternion.Lerp(m_Card3dUI.m_ScaleGrp.localRotation, m_CardTargetLerpRot, Time.deltaTime * 5f);
		}
		if (m_IsLerpingCardScale)
		{
			m_Card3dUI.m_ScaleGrp.localScale = Vector3.Lerp(m_Card3dUI.m_ScaleGrp.localScale, m_CardTargetLerpScale, Time.deltaTime * 10f);
		}
		if (m_IsLerpingCardLocalPos)
		{
			m_Card3dUI.m_ScaleGrp.localPosition = Vector3.Lerp(m_Card3dUI.m_ScaleGrp.localPosition, m_CardTargetLerpLocalPos, Time.deltaTime * 10f);
		}
		if (m_IsPlayAnimAfterTime)
		{
			m_PlayAnimAfterTimer += Time.deltaTime;
			if (m_PlayAnimAfterTimer >= m_PlayAnimAfterTime)
			{
				m_PlayAnimAfterTimer = 0f;
				m_IsPlayAnimAfterTime = false;
				if (m_TimePassedPlayAnimName != "")
				{
					m_Card3dUI.m_Anim.Rewind();
					m_Card3dUI.m_Anim.Play(m_TimePassedPlayAnimName);
				}
				else
				{
					m_Card3dUI.m_Anim.Rewind();
					m_Card3dUI.m_Anim.Play();
				}
			}
		}
		base.LateUpdate();
	}

	protected override void OnStartLerp()
	{
		base.OnStartLerp();
		if (m_LerpPosTimer > 0f)
		{
			m_IsEnableUpDownLerpMotion = false;
			m_IsEnableLocalUpDownLerpMotion = false;
			m_LerpPosOffset = Vector3.zero;
		}
	}

	protected override void OnStopLerp()
	{
		base.OnStopLerp();
		m_IsEnableUpDownLerpMotion = false;
		m_IsEnableLocalUpDownLerpMotion = false;
		m_LerpPosOffset = Vector3.zero;
	}

	protected override void OnFinishLerp()
	{
		base.OnFinishLerp();
		m_IsEnableUpDownLerpMotion = false;
		m_IsEnableLocalUpDownLerpMotion = false;
		m_LerpPosOffset = Vector3.zero;
		if (m_IsHideAfterFinishLerp)
		{
			m_Card3dUI.gameObject.SetActive(value: false);
			base.gameObject.SetActive(value: false);
			if ((bool)m_CollectionBinderFlipAnimCtrl)
			{
				m_CollectionBinderFlipAnimCtrl.OnCardFinishLerpHide();
			}
		}
		if (m_PlayAnimAfterFinishLerp)
		{
			m_PlayAnimAfterFinishLerp = false;
			if (m_FinishLerpPlayAnimName != "")
			{
				m_Card3dUI.m_Anim.Rewind();
				m_Card3dUI.m_Anim.Play(m_FinishLerpPlayAnimName);
			}
			else
			{
				m_Card3dUI.m_Anim.Rewind();
				m_Card3dUI.m_Anim.Play();
			}
		}
	}

	public void SetFinishLerpPlayAnim(string animName)
	{
		if ((bool)m_Card3dUI.m_Anim)
		{
			m_PlayAnimAfterFinishLerp = true;
			m_FinishLerpPlayAnimName = animName;
		}
	}

	public void SetPlayAnimAfterTime(string animName, float playAfterTime = 0f)
	{
		if ((bool)m_Card3dUI.m_Anim)
		{
			m_IsPlayAnimAfterTime = true;
			m_PlayAnimAfterTime = playAfterTime;
			m_PlayAnimAfterTimer = 0f;
			m_TimePassedPlayAnimName = animName;
		}
	}

	public override void OnDestroyed()
	{
		if ((bool)m_Card3dUI && m_IsCard3dUIFollow)
		{
			m_Card3dUI.DisableCard();
		}
		m_CurrentCustomer = null;
		base.OnDestroyed();
	}

	public void OnCardScanned()
	{
		m_Collider.enabled = false;
		m_Rigidbody.isKinematic = true;
		m_CurrentCustomer.OnCardScanned(this);
		m_CurrentCustomer = null;
		SetHideItemAfterFinishLerp();
		LerpToTransform(m_ScannedItemLerpPos, m_ScannedItemLerpPos);
	}

	public void RegisterScanCard(Customer customer, Transform scannedItemLerpPos)
	{
		m_CurrentCustomer = customer;
		m_ScannedItemLerpPos = scannedItemLerpPos;
	}

	public void SetCurrentPrice(float price)
	{
		m_CurrentPrice = price;
	}

	public float GetCurrentPrice()
	{
		return m_CurrentPrice;
	}

	public bool IsNotScanned()
	{
		if ((bool)m_CurrentCustomer)
		{
			return true;
		}
		return false;
	}

	public bool IsDisplayedOnShelf()
	{
		return m_IsDisplayedOnShelf;
	}

	public void SetIsDisplayedOnShelf(bool isDisplayedOnShelf)
	{
		m_CollectionBinderFlipAnimCtrl = null;
		m_IsDisplayedOnShelf = isDisplayedOnShelf;
		m_Card3dUI.m_SlabTopLayerMesh.SetActive(!isDisplayedOnShelf);
	}
}
