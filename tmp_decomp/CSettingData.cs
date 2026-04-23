using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CSettingData
{
	private static CSettingData m_Instance;

	[SerializeField]
	private List<KeybindBaseKey> m_KeybindBaseKeyList;

	[SerializeField]
	private List<KeybindBaseGamepadControl> m_KeybindBaseJoystickCtrlList;

	public static CSettingData instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new CSettingData();
			}
			return m_Instance;
		}
	}

	public void PropagateLoadSettingData(CSettingData settingData)
	{
		if (settingData.m_KeybindBaseKeyList != null && settingData.m_KeybindBaseKeyList.Count > 0)
		{
			InputManager.m_KeybindBaseKeyList = settingData.m_KeybindBaseKeyList;
			InputManager.m_KeybindBaseJoystickCtrlList = settingData.m_KeybindBaseJoystickCtrlList;
		}
	}

	public void SetLoadData<T>(ref T data, T loadData)
	{
		if (loadData != null)
		{
			_ = (object)data;
			data = loadData;
		}
	}

	public void SaveSettingData()
	{
		InputManager.OnKeybindSettingSaved();
		SetLoadData(ref m_KeybindBaseKeyList, InputManager.m_KeybindBaseKeyList);
		SetLoadData(ref m_KeybindBaseJoystickCtrlList, InputManager.m_KeybindBaseJoystickCtrlList);
		CSaveLoad.SaveSetting();
	}
}
