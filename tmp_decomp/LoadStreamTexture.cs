using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadStreamTexture : CSingleton<LoadStreamTexture>
{
	public static LoadStreamTexture m_Instance;

	public Sprite m_LoadingSprite;

	public List<Sprite> m_LoadedSpriteList;

	private Dictionary<string, Sprite> m_LoadedSpriteDict = new Dictionary<string, Sprite>();

	public string m_ImageURL = "https://www.opneon.com/streamTexture/";

	public Image m_Image;

	public List<string> m_CurrentLoadingFileNameList = new List<string>();

	public bool m_IsPreloading;

	public bool m_IsWaitingPreloadCallback;

	public int m_CurrentPreloadExpansionIndex;

	public int m_CurrentPreloadIndex;

	public List<ECardExpansionType> m_ExpasionTypeToPreLoadList;

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
		if (m_IsPreloading && !m_IsWaitingPreloadCallback && m_CurrentPreloadExpansionIndex >= m_ExpasionTypeToPreLoadList.Count && m_CurrentPreloadExpansionIndex >= m_ExpasionTypeToPreLoadList.Count)
		{
			m_IsPreloading = false;
			m_IsWaitingPreloadCallback = false;
		}
	}

	private void OnPreloadFinish()
	{
		m_CurrentPreloadIndex++;
		m_IsWaitingPreloadCallback = false;
	}

	public static Sprite GetImage(string fileName)
	{
		Sprite value = null;
		if (CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteDict.ContainsKey(fileName))
		{
			CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteDict.TryGetValue(fileName, out value);
			if (value != null)
			{
				CSingleton<LoadStreamTexture>.Instance.m_Image.sprite = value;
				return value;
			}
		}
		Texture2D texture2D = LoadPNG(fileName);
		if (texture2D != null)
		{
			value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			value.name = fileName;
			if (!CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteDict.ContainsKey(fileName))
			{
				CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteList.Add(value);
				CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteDict.TryAdd(fileName, value);
			}
			else
			{
				CSingleton<LoadStreamTexture>.Instance.m_LoadedSpriteDict[fileName] = value;
			}
			return value;
		}
		if (!CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Contains(fileName))
		{
			CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Add(fileName);
			CSingleton<LoadStreamTexture>.Instance.StartCoroutine(CSingleton<LoadStreamTexture>.Instance.LoadTextureFromWeb(fileName));
		}
		return null;
	}

	private IEnumerator LoadTextureFromWeb(string fileName)
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(m_ImageURL + fileName + ".png");
		yield return www.SendWebRequest();
		if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError || www.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.LogError("Error: " + www.error);
			CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Remove(fileName);
			if (m_IsPreloading)
			{
				OnPreloadFinish();
			}
			yield break;
		}
		Texture2D content = DownloadHandlerTexture.GetContent(www);
		content.ignoreMipmapLimit = true;
		Sprite sprite = Sprite.Create(content, new Rect(0f, 0f, content.width, content.height), Vector2.zero);
		sprite.name = fileName;
		if (!m_LoadedSpriteDict.ContainsKey(fileName))
		{
			m_LoadedSpriteList.Add(sprite);
			m_LoadedSpriteDict.TryAdd(fileName, sprite);
		}
		else
		{
			m_LoadedSpriteDict[fileName] = sprite;
		}
		CSingleton<LoadStreamTexture>.Instance.m_Image.sprite = sprite;
		WriteImageOnDisk(content, fileName);
		CSingleton<LoadStreamTexture>.Instance.m_CurrentLoadingFileNameList.Remove(fileName);
		if (m_IsPreloading)
		{
			OnPreloadFinish();
		}
	}

	private void WriteImageOnDisk(Texture2D texture, string fileName)
	{
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.persistentDataPath + "/" + fileName + ".png", bytes);
	}

	private static Texture2D LoadPNG(string fileName)
	{
		string path = Application.persistentDataPath + "/" + fileName + ".png";
		if (File.Exists(path))
		{
			byte[] data = File.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(2, 2, DefaultFormat.LDR, TextureCreationFlags.None);
			texture2D.LoadImage(data);
			return texture2D;
		}
		return null;
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			_ = Application.isMobilePlatform;
		}
	}

	private void OnDisable()
	{
		if (!Application.isPlaying)
		{
			_ = Application.isMobilePlatform;
		}
	}
}
