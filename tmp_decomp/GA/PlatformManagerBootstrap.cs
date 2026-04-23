using UnityEngine;

namespace GA;

[DefaultExecutionOrder(-1000)]
public static class PlatformManagerBootstrap
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		if (!(PlatformManager.Instance != null))
		{
			GameObject gameObject = Resources.Load<GameObject>("PlatformManager");
			if (gameObject != null)
			{
				Object.DontDestroyOnLoad(Object.Instantiate(gameObject));
			}
		}
	}
}
