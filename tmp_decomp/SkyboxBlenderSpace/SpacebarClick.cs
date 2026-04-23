using UnityEngine;

namespace SkyboxBlenderSpace;

public class SpacebarClick : MonoBehaviour
{
	public SkyboxBlender skyboxScript;

	private bool isStopped;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			skyboxScript.Blend();
			isStopped = false;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (isStopped)
			{
				skyboxScript.Resume();
				isStopped = false;
			}
			else
			{
				skyboxScript.Stop();
				isStopped = true;
			}
		}
	}
}
