using UnityEngine;

public class FrontMover : MonoBehaviour
{
	public Transform pivot;

	public ParticleSystem effect;

	public float speed = 15f;

	public float drug = 1f;

	public float repeatingTime = 1f;

	private float startSpeed;

	private void Start()
	{
		InvokeRepeating("StartAgain", 0f, repeatingTime);
		effect.Play();
		startSpeed = speed;
	}

	private void StartAgain()
	{
		startSpeed = speed;
		base.transform.position = pivot.position;
	}

	private void Update()
	{
		startSpeed *= drug;
		base.transform.position += base.transform.forward * (startSpeed * Time.deltaTime);
	}
}
