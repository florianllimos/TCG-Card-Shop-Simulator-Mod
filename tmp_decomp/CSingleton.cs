using System;
using UnityEngine;

[Serializable]
public class CSingleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	private static object padlock = new object();

	private static bool isSet = false;

	private static bool isQuitting = false;

	public static bool IsQuitting => isQuitting;

	public static T Instance
	{
		get
		{
			if (isQuitting)
			{
				Debug.LogWarning("[Singleton] Cancel creating '" + typeof(T)?.ToString() + "' , application is closing.");
				return null;
			}
			if (!isSet)
			{
				instance = UnityEngine.Object.FindObjectOfType<T>();
				if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
				{
					isSet = true;
					return instance;
				}
				if (instance == null)
				{
					GameObject obj = new GameObject
					{
						name = "(singleton)" + typeof(T).ToString()
					};
					instance = obj.AddComponent<T>();
					UnityEngine.Object.DontDestroyOnLoad(obj);
				}
				isSet = true;
			}
			return instance;
		}
	}

	private void OnDestroy()
	{
		isSet = false;
	}

	public virtual void OnApplicationQuit()
	{
		isQuitting = true;
	}

	public static bool InstanceExist()
	{
		return isSet;
	}

	public static int InstanceID()
	{
		if (isSet)
		{
			return instance.gameObject.GetInstanceID();
		}
		return 0;
	}
}
