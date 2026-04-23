using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : CSingleton<SoundManager>
{
	private int m_CurrentFrame;

	private int m_FrameLoopLimit;

	private int m_CheckFrameIndex;

	private float m_MusicVolume01;

	private float m_MusicVolume02;

	private float m_MusicBlendSpeed = 1f;

	private float m_LinearBlendAmount;

	private float m_MusicLerpClipLower = 0.05f;

	private float m_MusicVolumeMultiplier = 0.3f;

	private float m_SFXVolumeMultiplier = 2f;

	private bool m_IsMusic02;

	private bool m_IsBlendingMusic;

	private bool m_IsLinearBlend;

	private bool m_StopEvaluateMusicEnd;

	private AudioClip m_VoiceRecordClip1;

	private string m_VoiceRecordFilePath;

	private bool m_IsRecording;

	public AudioSource m_MusicSrc01;

	public AudioSource m_MusicSrc02;

	public List<AudioSource> m_AudioSrcList;

	public List<AudioSource> m_ExternalAudioSrcList;

	private Dictionary<string, AudioClip> m_LoadedAudioClip = new Dictionary<string, AudioClip>();

	private AudioClip m_QueuedMusic;

	public AudioSource m_ExpIncrease;

	public AudioSource m_CoinIncrease;

	public AudioSource m_GemIncrease;

	public AudioSource m_TPIncrease;

	public AudioSource m_LevelUpExpLoop;

	public AudioSource m_SprayLoop;

	private static int m_MusicIndex = 7;

	private static int m_SFXIndex = 7;

	private static float m_MusicVolume = 1f;

	private static float m_SFXVolume = 1f;

	private static bool m_IsMute;

	public static int MusicIndex
	{
		get
		{
			return m_MusicIndex;
		}
		set
		{
			m_MusicIndex = value;
		}
	}

	public static int SFXIndex
	{
		get
		{
			return m_SFXIndex;
		}
		set
		{
			m_SFXIndex = value;
		}
	}

	public static float MusicVolume
	{
		get
		{
			return m_MusicVolume;
		}
		set
		{
			m_MusicVolume = value;
			SaveData();
		}
	}

	public static float SFXVolume
	{
		get
		{
			return m_SFXVolume;
		}
		set
		{
			m_SFXVolume = value;
			SaveData();
		}
	}

	protected SoundManager()
	{
	}

	private void Awake()
	{
		if (CSingleton<SoundManager>.Instance != null && CSingleton<SoundManager>.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		m_FrameLoopLimit = m_AudioSrcList.Count;
		m_MusicVolume01 = 1f;
		m_MusicVolume02 = 0f;
	}

	public static void MuteAllSound()
	{
		m_IsMute = true;
		CSingleton<SoundManager>.Instance.EvaluateSoundVolume();
	}

	public static void UnMuteAllSound()
	{
		m_IsMute = false;
		CSingleton<SoundManager>.Instance.EvaluateSoundVolume();
	}

	private IEnumerator DelayLoadData()
	{
		m_MusicVolume = 0f;
		m_SFXVolume = 0f;
		yield return new WaitForSeconds(0.1f);
		LoadData();
	}

	private void Update()
	{
		EvaluateAudioSourceOver();
		EvaluateFrame();
		EvaluateLerpMusic();
	}

	private void EvaluateMusicEnding()
	{
	}

	private void EvaluateFrame()
	{
		m_CurrentFrame++;
		m_CheckFrameIndex = m_CurrentFrame % m_FrameLoopLimit;
	}

	public static void PlayAudio(string audioName, float volume = 1f, float pitch = 1f)
	{
		CSingleton<SoundManager>.Instance.PlayAudioPrivate(audioName, volume, pitch);
	}

	public static void PlayAudioDelayed(string audioName, float delayTime, float volume = 1f, float pitch = 1f)
	{
		CSingleton<SoundManager>.Instance.StartCoroutine(CSingleton<SoundManager>.Instance.DelayPlayAudio(audioName, delayTime, volume, pitch));
	}

	private IEnumerator DelayPlayAudio(string audioName, float delayTime, float volume = 1f, float pitch = 1f)
	{
		yield return new WaitForSeconds(delayTime);
		CSingleton<SoundManager>.Instance.PlayAudioPrivate(audioName, volume, pitch);
	}

	private void PlayAudioPrivate(string audioName, float volume = 1f, float pitch = 1f)
	{
		if (GameInstance.m_HasFinishHideLoadingScreen)
		{
			AudioSource emptyAudioSource = GetEmptyAudioSource();
			if (emptyAudioSource != null)
			{
				emptyAudioSource.clip = GetAudioClip(audioName);
				emptyAudioSource.pitch = pitch;
				emptyAudioSource.volume = volume * m_SFXVolume * m_SFXVolume * m_SFXVolumeMultiplier;
				emptyAudioSource.Play();
			}
		}
	}

	public static void PlayAudioForceChannel(string audioName, float volume = 1f, float pitch = 1f, int channel = 0)
	{
		CSingleton<SoundManager>.Instance.PlayAudioForceChannelPrivate(audioName, volume, pitch, channel);
	}

	private void PlayAudioForceChannelPrivate(string audioName, float volume = 1f, float pitch = 1f, int channel = 0)
	{
		if (!GameInstance.m_HasFinishHideLoadingScreen)
		{
			return;
		}
		AudioSource audioSource = m_AudioSrcList[channel];
		if (audioSource != null)
		{
			AudioClip audioClip = GetAudioClip(audioName);
			if (!(audioClip == audioSource.clip))
			{
				audioSource.clip = audioClip;
				audioSource.pitch = pitch;
				audioSource.volume = volume * m_SFXVolume * m_SFXVolume * m_SFXVolumeMultiplier;
				audioSource.Play();
			}
		}
	}

	private AudioClip GetAudioClip(string audioName)
	{
		if (!m_LoadedAudioClip.TryGetValue(audioName, out var value))
		{
			value = Resources.Load("SFX/" + audioName) as AudioClip;
			m_LoadedAudioClip.Add(audioName, value);
		}
		if ((bool)value)
		{
			return value;
		}
		return null;
	}

	private AudioSource GetEmptyAudioSource()
	{
		for (int i = 0; i < m_AudioSrcList.Count; i++)
		{
			if (m_AudioSrcList[i].clip == null)
			{
				return m_AudioSrcList[i];
			}
		}
		return null;
	}

	private void EvaluateAudioSourceOver()
	{
		if ((bool)m_AudioSrcList[m_CheckFrameIndex].clip && !m_AudioSrcList[m_CheckFrameIndex].isPlaying)
		{
			m_AudioSrcList[m_CheckFrameIndex].clip = null;
		}
	}

	public static void SetAndPlayMusic(string audioName, int index)
	{
		CSingleton<SoundManager>.Instance.SetAndPlayMusicPrivate(audioName, index);
	}

	private void SetAndPlayMusicPrivate(string audioName, int index)
	{
		if (index == 0)
		{
			m_MusicSrc01.clip = GetAudioClip(audioName);
			m_MusicSrc01.Play();
		}
		else
		{
			m_MusicSrc02.clip = GetAudioClip(audioName);
			m_MusicSrc02.Play();
		}
	}

	public static void PlayMusic()
	{
		CSingleton<SoundManager>.Instance.PlayMusicPrivate();
	}

	private void PlayMusicPrivate()
	{
		if (m_IsMusic02)
		{
			m_MusicSrc02.Play();
		}
		else
		{
			m_MusicSrc01.Play();
		}
	}

	public static void PauseMusic()
	{
		CSingleton<SoundManager>.Instance.PauseMusicPrivate();
	}

	private void PauseMusicPrivate()
	{
		m_MusicSrc01.Pause();
		m_MusicSrc02.Pause();
	}

	public static void StopMusic()
	{
		CSingleton<SoundManager>.Instance.StopMusicPrivate();
	}

	private void StopMusicPrivate()
	{
		m_MusicSrc01.Stop();
		m_MusicSrc02.Stop();
	}

	public static void QueueMusic(string startAudioName, string audioName, float blendSpeed)
	{
		CSingleton<SoundManager>.Instance.QueueMusicPrivate(startAudioName, audioName, blendSpeed);
	}

	private void QueueMusicPrivate(string startAudioName, string audioName, float blendSpeed)
	{
		double dspTime = AudioSettings.dspTime;
		AudioClip audioClip = GetAudioClip(startAudioName);
		double num = (double)audioClip.samples / (double)audioClip.frequency;
		float smoothDeltaTime = Time.smoothDeltaTime;
		m_QueuedMusic = GetAudioClip(audioName);
		if (m_IsMusic02)
		{
			m_MusicSrc01.Stop();
			m_MusicSrc01.volume = m_MusicSrc02.volume;
			m_MusicSrc01.clip = m_QueuedMusic;
			m_MusicSrc01.PlayScheduled(dspTime + num + (double)smoothDeltaTime);
			m_MusicSrc02.SetScheduledEndTime(dspTime + num + (double)smoothDeltaTime);
		}
		else
		{
			m_MusicSrc02.Stop();
			m_MusicSrc02.volume = m_MusicSrc01.volume;
			m_MusicSrc02.clip = m_QueuedMusic;
			m_MusicSrc02.PlayScheduled(dspTime + num + (double)smoothDeltaTime);
			m_MusicSrc01.SetScheduledEndTime(dspTime + num + (double)smoothDeltaTime);
		}
	}

	public static void StopQueueMusic()
	{
		CSingleton<SoundManager>.Instance.m_QueuedMusic = null;
		if (CSingleton<SoundManager>.Instance.m_IsMusic02)
		{
			CSingleton<SoundManager>.Instance.m_MusicSrc01.Stop();
			CSingleton<SoundManager>.Instance.m_MusicSrc01.clip = null;
		}
		else
		{
			CSingleton<SoundManager>.Instance.m_MusicSrc02.Stop();
			CSingleton<SoundManager>.Instance.m_MusicSrc02.clip = null;
		}
	}

	public static void BlendToMusic(string audioName, float blendSpeed, bool isLinearBlend)
	{
		CSingleton<SoundManager>.Instance.BlendToMusicPrivate(audioName, blendSpeed, isLinearBlend);
	}

	private void BlendToMusicPrivate(string audioName, float blendSpeed, bool isLinearBlend)
	{
		if (m_IsMusic02)
		{
			if (m_MusicSrc02.clip == GetAudioClip(audioName))
			{
				return;
			}
		}
		else if (m_MusicSrc01.clip == GetAudioClip(audioName))
		{
			return;
		}
		m_QueuedMusic = null;
		m_MusicBlendSpeed = blendSpeed;
		m_IsLinearBlend = isLinearBlend;
		if (m_MusicSrc02.clip == null)
		{
			m_MusicSrc02.clip = GetAudioClip(audioName);
			m_IsMusic02 = true;
			m_IsBlendingMusic = true;
			m_MusicSrc02.Play();
		}
		else if (m_MusicSrc01.clip == null)
		{
			m_MusicSrc01.clip = GetAudioClip(audioName);
			m_IsMusic02 = false;
			m_IsBlendingMusic = true;
			m_MusicSrc01.Play();
		}
		else
		{
			m_MusicSrc01.clip = GetAudioClip(audioName);
			m_IsMusic02 = false;
			m_IsBlendingMusic = true;
			m_MusicSrc01.Play();
		}
		m_StopEvaluateMusicEnd = false;
	}

	private void EvaluateLerpMusic()
	{
		if (!m_IsBlendingMusic)
		{
			return;
		}
		if (m_IsMusic02)
		{
			if (m_IsLinearBlend)
			{
				m_LinearBlendAmount += Time.deltaTime * m_MusicBlendSpeed;
				m_MusicVolume01 = Mathf.Lerp(1f, 0f, m_LinearBlendAmount);
				m_MusicVolume02 = Mathf.Lerp(0f, 1f, m_LinearBlendAmount);
			}
			else
			{
				m_MusicVolume01 = Mathf.Lerp(m_MusicVolume01, 0f, Time.deltaTime * m_MusicBlendSpeed);
				m_MusicVolume02 = Mathf.Lerp(m_MusicVolume02, 1f, Time.deltaTime * m_MusicBlendSpeed);
			}
			if (m_MusicVolume01 < m_MusicLerpClipLower)
			{
				m_LinearBlendAmount = 0f;
				m_MusicVolume01 = 0f;
				m_MusicVolume02 = 1f;
				m_IsBlendingMusic = false;
				if (m_QueuedMusic == null)
				{
					m_MusicSrc01.clip = null;
					m_MusicSrc01.Stop();
				}
			}
		}
		else if (!m_IsMusic02)
		{
			if (m_IsLinearBlend)
			{
				m_LinearBlendAmount += Time.deltaTime * m_MusicBlendSpeed;
				m_MusicVolume01 = Mathf.Lerp(0f, 1f, m_LinearBlendAmount);
				m_MusicVolume02 = Mathf.Lerp(1f, 0f, m_LinearBlendAmount);
			}
			else
			{
				m_MusicVolume02 = Mathf.Lerp(m_MusicVolume02, 0f, Time.deltaTime * m_MusicBlendSpeed);
				m_MusicVolume01 = Mathf.Lerp(m_MusicVolume01, 1f, Time.deltaTime * m_MusicBlendSpeed);
			}
			if (m_MusicVolume02 < m_MusicLerpClipLower)
			{
				m_LinearBlendAmount = 0f;
				m_MusicVolume02 = 0f;
				m_MusicVolume01 = 1f;
				m_IsBlendingMusic = false;
				if (m_QueuedMusic == null)
				{
					m_MusicSrc02.clip = null;
					m_MusicSrc02.Stop();
				}
			}
		}
		if ((bool)m_QueuedMusic)
		{
			m_MusicVolume02 = 1f;
			m_MusicVolume01 = 1f;
		}
		m_MusicSrc01.volume = m_MusicVolume01 * m_MusicVolume * m_MusicVolumeMultiplier;
		m_MusicSrc02.volume = m_MusicVolume02 * m_MusicVolume * m_MusicVolumeMultiplier;
	}

	private void LoadData()
	{
		EvaluateSoundVolume();
	}

	private static void SaveData()
	{
	}

	private void OnEnable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.AddListener<CEventPlayer_VolumeChanged>(CPlayer_OnVolumeChanged);
		}
	}

	private void OnDisable()
	{
		if (Application.isPlaying || Application.isMobilePlatform)
		{
			CEventManager.RemoveListener<CEventPlayer_VolumeChanged>(CPlayer_OnVolumeChanged);
		}
	}

	private void CPlayer_OnVolumeChanged(CEventPlayer_VolumeChanged evt)
	{
	}

	public void EvaluateSoundVolume()
	{
		float musicVolume = m_MusicVolume;
		float volume = m_SFXVolume;
		if (m_IsMute)
		{
			volume = 0f;
		}
		for (int i = 0; i < m_AudioSrcList.Count; i++)
		{
			m_AudioSrcList[i].volume = volume;
		}
		m_MusicSrc01.volume = m_MusicVolume01 * musicVolume * m_MusicVolumeMultiplier;
		m_MusicSrc02.volume = m_MusicVolume02 * musicVolume * m_MusicVolumeMultiplier;
		m_ExpIncrease.volume = volume;
		m_CoinIncrease.volume = volume;
		m_GemIncrease.volume = volume;
		m_TPIncrease.volume = volume;
		for (int j = 0; j < m_ExternalAudioSrcList.Count; j++)
		{
			m_ExternalAudioSrcList[j].volume = volume;
		}
	}

	public static void GenericLightTap(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_ButtonLightTap", 0.4f, pitch);
	}

	public static void GenericPop(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_Popup_Low", volume, pitch);
	}

	public static void GenericConfirm(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_Popup2", 0.6f, 0.5f);
	}

	public static void GenericCancel(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_Popup2", 0.6f, 0.4f);
	}

	public static void GenericMenuOpen(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_Popup2", 0.6f, 0.5f);
	}

	public static void GenericMenuClose(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_Popup2", 0.6f, 0.4f);
	}

	public static void GenericPurchase(float volume = 1f, float pitch = 1f)
	{
		PlayAudio("SFX_CustomerBuy", volume, pitch);
	}

	public static void SetEnableSound_ExpIncrease(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_ExpIncrease.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_ExpIncrease.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_ExpIncrease.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_ExpIncrease.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_ExpIncrease.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_ExpIncrease.Stop();
			}
		}
	}

	public static void SetEnableSound_CoinIncrease(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_CoinIncrease.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_CoinIncrease.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_CoinIncrease.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_CoinIncrease.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_CoinIncrease.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_CoinIncrease.Stop();
			}
		}
	}

	public static void SetEnableSound_GemIncrease(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_GemIncrease.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_GemIncrease.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_GemIncrease.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_GemIncrease.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_GemIncrease.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_GemIncrease.Stop();
			}
		}
	}

	public static void SetEnableSound_TamerPointIncrease(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_TPIncrease.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_TPIncrease.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_TPIncrease.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_TPIncrease.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_TPIncrease.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_TPIncrease.Stop();
			}
		}
	}

	public static void SetEnableSound_LevelupExpLoop(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_LevelUpExpLoop.Stop();
			}
		}
	}

	public static void SetEnableSound_SprayLoop(bool isEnable)
	{
		if (m_SFXVolume == 0f)
		{
			CSingleton<SoundManager>.Instance.m_SprayLoop.Stop();
		}
		else if (CSingleton<SoundManager>.Instance.m_SprayLoop.enabled != isEnable)
		{
			CSingleton<SoundManager>.Instance.m_SprayLoop.enabled = isEnable;
			if (isEnable)
			{
				CSingleton<SoundManager>.Instance.m_SprayLoop.volume = m_SFXVolume;
				CSingleton<SoundManager>.Instance.m_SprayLoop.Play();
			}
			else
			{
				CSingleton<SoundManager>.Instance.m_SprayLoop.Stop();
			}
		}
	}
}
