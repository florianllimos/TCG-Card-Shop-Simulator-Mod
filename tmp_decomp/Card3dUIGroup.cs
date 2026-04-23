using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card3dUIGroup : MonoBehaviour
{
	public CardUI m_CardUI;

	public Animation m_Anim;

	public GameObject m_CardFrontMeshPos;

	public GameObject m_CardBackMesh;

	public GameObject m_NewCardIndicator;

	public GameObject m_GradedCardGrp;

	public GameObject m_GradedCardCullGrp;

	public GameObject m_SlabTopLayerMesh;

	public GameObject m_GradedCaseCullCardFrontMeshBlocker;

	public GameObject m_GradedCaseCullCardBackMeshBlocker;

	public Transform m_ScaleGrp;

	public Transform m_CardUIAnimGrp;

	public Transform m_BtmLayerParentGrp;

	public TextMeshProUGUI m_CardCountText;

	public TextMeshProUGUI m_GradeNumberText;

	public TextMeshProUGUI m_GradeDescriptionText;

	public TextMeshProUGUI m_GradeNameText;

	public TextMeshProUGUI m_GradeExpansionRarityText;

	public TextMeshProUGUI m_GradeSerialText;

	public Image m_GradedCardBrightnessControl;

	public bool m_IgnoreCulling;

	public bool m_AlwaysCulling;

	private bool m_IsActive;

	private bool m_IsSmoothLerpingToPos;

	private bool m_IsIgnoreUpForce;

	private float m_Timer;

	private float m_UpTimer;

	private float m_LerpSpeed = 3f;

	private float m_UpLerpSpeed = 5f;

	private float m_UpLerpHeight = 0.1f;

	private float m_Accelration;

	private Vector3 m_StartPos;

	private Quaternion m_StartRot;

	private Vector3 m_StartScale;

	private Transform m_TargetTransform;

	private void Awake()
	{
		m_NewCardIndicator.SetActive(value: false);
		m_CardUI.InitCard3dUIGroup(this);
	}

	private void Start()
	{
		Card3dUISpawner.AddCardToManager(this);
		m_CardUI.SetBrightness(CSingleton<LightManager>.Instance.GetBrightness());
	}

	public void SetSimplifyCardDistanceCull(bool isCull)
	{
		m_CardUI.ShowSimplifiedCullingGradedCardCase(isCull);
		m_GradedCardCullGrp.SetActive(!isCull);
	}

	public void SetVisibility(bool isVisible)
	{
		base.gameObject.SetActive(isVisible);
	}

	public void EvaluateCardGrade(CardData cardData)
	{
		if (cardData.cardGrade > 0)
		{
			if (m_IgnoreCulling)
			{
				m_GradedCardCullGrp.SetActive(value: true);
			}
			m_GradedCardGrp.SetActive(value: true);
			m_GradeNumberText.text = cardData.cardGrade.ToString();
			m_GradeDescriptionText.text = GameInstance.GetCardGradeString(cardData.cardGrade);
			m_GradeNameText.text = m_CardUI.m_MonsterNameText.text;
			m_GradeExpansionRarityText.text = LocalizationManager.GetTranslation(cardData.expansionType.ToString()) + " " + CPlayerData.GetFullCardTypeName(cardData);
		}
		else
		{
			m_GradedCardGrp.SetActive(value: false);
		}
	}

	public void SetCardCountText(int count, bool showDuplicate)
	{
		if (showDuplicate)
		{
			m_CardCountText.text = "X " + count + " (" + (count + 1) + ")";
		}
		else
		{
			m_CardCountText.text = "X " + count;
		}
	}

	public void SetCardCountTextVisibility(bool isVisible)
	{
		m_CardCountText.gameObject.SetActive(isVisible);
	}

	public void ActivateCard()
	{
		m_IsActive = true;
	}

	public void DisableCard()
	{
		m_IsActive = false;
		m_IgnoreCulling = false;
		SetAlwaysCulling(alwaysCulling: false);
		SetLocalScale(Vector3.one);
		m_CardUIAnimGrp.gameObject.SetActive(value: true);
		m_SlabTopLayerMesh.gameObject.SetActive(value: true);
		m_GradedCaseCullCardFrontMeshBlocker.SetActive(value: false);
		m_GradedCaseCullCardBackMeshBlocker.SetActive(value: false);
		Card3dUISpawner.DisableCard(this);
	}

	public void SetLocalScale(Vector3 localScale)
	{
		m_ScaleGrp.transform.localScale = localScale;
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
		m_IsSmoothLerpingToPos = true;
		m_IsIgnoreUpForce = ignoreUpForce;
	}

	private void Update()
	{
		if (m_IsSmoothLerpingToPos)
		{
			m_UpTimer += Time.deltaTime * m_UpLerpSpeed * 0.75f;
			Vector3 vector = Vector3.up * (Mathf.PingPong(Mathf.Clamp(m_UpTimer, 0f, 2f), 1f) * m_UpLerpHeight);
			if (m_UpTimer > 0.2f)
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * (1f + m_Accelration);
				m_Accelration += Time.deltaTime;
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetTransform.position + vector, Time.deltaTime * 10f);
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetTransform.rotation, Time.deltaTime * 10f);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, m_TargetTransform.localScale, Time.deltaTime * 10f);
			}
			else
			{
				m_Timer += Time.deltaTime * m_LerpSpeed * 0.1f;
				base.transform.position = Vector3.Lerp(base.transform.position, m_TargetTransform.position + vector, Time.deltaTime * 2f) + vector;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, m_TargetTransform.rotation, Time.deltaTime * 2f);
				base.transform.localScale = Vector3.Lerp(base.transform.localScale, m_TargetTransform.localScale, Time.deltaTime * 2f);
			}
			if (m_IsIgnoreUpForce)
			{
				vector = Vector3.zero;
				m_UpTimer = 2f;
			}
		}
	}

	public bool IsActive()
	{
		return m_IsActive;
	}

	public void CheckValidCard3dUI()
	{
		if (!m_CardUI.IsCard3dUIGroupSet())
		{
			m_CardUI.InitCard3dUIGroup(this);
		}
	}

	public void SetAlwaysCulling(bool alwaysCulling, bool setVisibilityInstant = true)
	{
		if (setVisibilityInstant)
		{
			m_CardUIAnimGrp.gameObject.SetActive(!alwaysCulling);
		}
		m_AlwaysCulling = alwaysCulling;
	}

	public bool GetAlwaysCulling()
	{
		return m_AlwaysCulling;
	}
}
