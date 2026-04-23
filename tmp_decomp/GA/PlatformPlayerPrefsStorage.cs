using System;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace GA;

public class PlatformPlayerPrefsStorage
{
	private PlayerPrefsData _data = new PlayerPrefsData();

	private bool _dirty;

	private readonly string _fileName;

	private readonly Func<byte[]> _loader;

	private readonly Action<byte[]> _saver;

	public bool IsLoaded { get; private set; }

	public PlayerPrefsData Data
	{
		get
		{
			if (!IsLoaded)
			{
				Load();
			}
			return _data;
		}
	}

	public PlatformPlayerPrefsStorage(string fileName, Func<byte[]> loader, Action<byte[]> saver)
	{
		_fileName = fileName;
		_loader = loader;
		_saver = saver;
	}

	public void Load()
	{
		if (IsLoaded)
		{
			return;
		}
		byte[] array = _loader?.Invoke();
		if (array != null && array.Length != 0)
		{
			try
			{
				string text = Encoding.UTF8.GetString(array);
				_data = JsonConvert.DeserializeObject<PlayerPrefsData>(text) ?? new PlayerPrefsData();
				Debug.Log($"[PlayerPrefsStorage] Loaded {_fileName} ({array.Length} bytes). json data: \n{text}\n");
			}
			catch (Exception arg)
			{
				Debug.LogError($"[PlayerPrefsStorage] Failed to parse {_fileName}: {arg}");
				_data = new PlayerPrefsData();
			}
		}
		else
		{
			Debug.Log("[PlayerPrefsStorage] No " + _fileName + " file found, starting empty");
		}
		IsLoaded = true;
	}

	public void MarkDirty()
	{
		_dirty = true;
	}

	public void Save()
	{
		if (_dirty)
		{
			try
			{
				string s = JsonConvert.SerializeObject(_data);
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				_saver?.Invoke(bytes);
			}
			catch (Exception arg)
			{
				Debug.LogError($"[PlayerPrefsStorage] Save failed: {arg}");
			}
			_dirty = false;
		}
	}
}
