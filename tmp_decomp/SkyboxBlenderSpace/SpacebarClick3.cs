using UnityEngine;

namespace SkyboxBlenderSpace;

public class SpacebarClick3 : MonoBehaviour
{
	public SkyboxBlender skyboxScript;

	private bool isStopped;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			skyboxScript.Blend(2);
			isStopped = false;
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (isStopped)
			{
				skyboxScript.Blend(2);
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
