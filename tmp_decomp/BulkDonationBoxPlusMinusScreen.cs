using I2.Loc;
using TMPro;
using UnityEngine;

public class BulkDonationBoxPlusMinusScreen : UIScreenBase
{
	public BulkDonationBoxUIScreen m_BulkDonationBoxUIScreen;

	public CardUI m_CardUI;

	public TextMeshProUGUI m_TotalCardCountText;

	public TextMeshProUGUI m_InAlbumCountText;

	public TMP_InputField m_CardAmountInput;

	public TextMeshProUGUI m_CardAmountInputDisplay;

	private int m_AlbumCount;

	private int m_StackCardCount;

	private int m_BoxTotalCardCount;

	private CardData m_CardData;

	private bool m_IsUpdateDeckEditUI;

	protected override void OnOpenScreen()
	{
		base.OnOpenScreen();
		m_CardAmountInputDisplay.gameObject.SetActive(value: false);
		m_TotalCardCountText.gameObject.SetActive(value: true);
	}

	public void OpenPlusMinusScreen(CardData cardData, int stackCardCount, int totalDeckCardCount)
	{
		m_CardData = cardData;
		m_StackCardCount = stackCardCount;
		m_BoxTotalCardCount = totalDeckCardCount;
		m_CardUI.SetCardUI(m_CardData);
		m_AlbumCount = CPlayerData.GetCardAmount(m_CardData);
		if (m_CardData.cardGrade > 0)
		{
			m_AlbumCount = 0;
			m_InAlbumCountText.enabled = false;
		}
		else
		{
			m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
			m_InAlbumCountText.enabled = true;
		}
		m_TotalCardCountText.text = m_StackCardCount.ToString();
		OpenScreen();
	}

	public void OnPressAddBtn()
	{
		if (m_BoxTotalCardCount >= m_BulkDonationBoxUIScreen.GetBoxTotalCardCountMax())
		{
			SoundManager.GenericMenuClose();
			NotEnoughResourceTextPopup.ShowText(ENotEnoughResourceText.MaxDeckCardLimitReached);
		}
		else if (m_AlbumCount > 0)
		{
			if (m_CardData.cardGrade > 0)
			{
				CPlayerData.RemoveGradedCard(m_CardData);
			}
			else
			{
				CPlayerData.ReduceCard(m_CardData, 1);
			}
			m_BulkDonationBoxUIScreen.UpdateCompactData(1);
			m_AlbumCount--;
			m_StackCardCount++;
			m_BoxTotalCardCount++;
			m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
			m_TotalCardCountText.text = m_StackCardCount.ToString();
			m_IsUpdateDeckEditUI = true;
			SoundManager.GenericConfirm();
		}
	}

	public void OnPressMinusBtn()
	{
		if (m_StackCardCount <= 1)
		{
			OnPressRemoveAllBtn();
			return;
		}
		CPlayerData.AddCard(m_CardData, 1);
		if (m_CardData.cardGrade > 0)
		{
			CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
			m_BulkDonationBoxUIScreen.RemoveCompactData();
		}
		else
		{
			m_BulkDonationBoxUIScreen.UpdateCompactData(-1);
		}
		m_AlbumCount++;
		m_StackCardCount--;
		m_BoxTotalCardCount--;
		m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
		m_TotalCardCountText.text = m_StackCardCount.ToString();
		m_IsUpdateDeckEditUI = true;
		SoundManager.GenericConfirm();
	}

	public void OnPressDoneBtn()
	{
		OnPressBack();
		if (m_IsUpdateDeckEditUI)
		{
			m_IsUpdateDeckEditUI = false;
			m_BulkDonationBoxUIScreen.OnCloseCardPlusMinusScreen();
			SoundManager.GenericConfirm();
		}
	}

	public void OnPressRemoveAllBtn()
	{
		if (m_StackCardCount > 0)
		{
			int stackCardCount = m_StackCardCount;
			CPlayerData.AddCard(m_CardData, stackCardCount);
			if (m_CardData.cardGrade > 0)
			{
				CSingleton<InteractionPlayerController>.Instance.m_CollectionBinderFlipAnimCtrl.SetCanUpdateSort(canSort: true);
				m_BulkDonationBoxUIScreen.RemoveCompactData();
			}
			else
			{
				m_BulkDonationBoxUIScreen.UpdateCompactData(-stackCardCount);
			}
			m_AlbumCount += stackCardCount;
			m_StackCardCount = 0;
			m_BoxTotalCardCount -= stackCardCount;
			m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
			m_TotalCardCountText.text = m_StackCardCount.ToString();
			OnPressBack();
			m_BulkDonationBoxUIScreen.OnCloseCardPlusMinusScreen();
			SoundManager.GenericConfirm();
		}
	}

	public void OnInputChanged(string text)
	{
		int num = Mathf.FloorToInt(GameInstance.GetInvariantCultureDecimal(text));
		m_CardAmountInputDisplay.text = num.ToString();
		m_CardAmountInputDisplay.gameObject.SetActive(value: true);
		m_TotalCardCountText.gameObject.SetActive(value: false);
	}

	public void OnInputTextSelected(string text)
	{
		m_CardAmountInputDisplay.gameObject.SetActive(value: true);
		m_CardAmountInput.text = m_StackCardCount.ToString();
		m_TotalCardCountText.gameObject.SetActive(value: false);
	}

	public void OnInputTextUpdated(string text)
	{
		int stackCardCount = m_StackCardCount;
		int num = Mathf.FloorToInt(GameInstance.GetInvariantCultureDecimal(text));
		if (num <= 0)
		{
			OnPressRemoveAllBtn();
			return;
		}
		int num2 = num - stackCardCount;
		if (num2 > 0)
		{
			int num3 = m_BulkDonationBoxUIScreen.GetBoxTotalCardCountMax() - m_BoxTotalCardCount;
			if (num2 > num3)
			{
				num2 = num3;
			}
			if (num2 > m_AlbumCount)
			{
				num2 = m_AlbumCount;
			}
			if (num2 > 0)
			{
				CPlayerData.ReduceCard(m_CardData, num2);
				m_BulkDonationBoxUIScreen.UpdateCompactData(num2);
				m_AlbumCount -= num2;
				m_StackCardCount += num2;
				m_BoxTotalCardCount += num2;
				m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
				m_TotalCardCountText.text = m_StackCardCount.ToString();
				m_IsUpdateDeckEditUI = true;
			}
		}
		else
		{
			int num4 = Mathf.Abs(num2);
			CPlayerData.AddCard(m_CardData, num4);
			m_BulkDonationBoxUIScreen.UpdateCompactData(-num4);
			m_AlbumCount += num4;
			m_StackCardCount -= num4;
			m_BoxTotalCardCount -= num4;
			m_InAlbumCountText.text = LocalizationManager.GetTranslation("In Album") + " : " + m_AlbumCount;
			m_TotalCardCountText.text = m_StackCardCount.ToString();
			m_IsUpdateDeckEditUI = true;
		}
		m_CardAmountInputDisplay.text = m_StackCardCount.ToString();
		m_TotalCardCountText.text = m_StackCardCount.ToString();
		m_CardAmountInputDisplay.gameObject.SetActive(value: false);
		m_TotalCardCountText.gameObject.SetActive(value: true);
	}
}
