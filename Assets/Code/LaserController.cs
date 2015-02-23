using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

	private LineRenderer _lineRenderer;
	public float SignDirectionLaser{get; set;}
	public LayerMask LaserColliders;

	public float LaserDamage = 1f;

	//Sounds
	public AudioClip LaserSound;

	//Effects
	public GameObject LaserSparkEffect;

	void Start () {
		_lineRenderer = GetComponent<LineRenderer>();
		_lineRenderer.enabled = false;
		SignDirectionLaser = 1.0f;
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftControl))
		{
			StopCoroutine("FireLaser");
			StartCoroutine("FireLaser");
		}
	}

	private IEnumerator FireLaser()
	{
		_lineRenderer.enabled = true;

		while (Input.GetKey(KeyCode.LeftControl))
		{
			//audio.PlayOneShot(LaserSound);
			if (!audio.isPlaying)
			{
				audio.volume = 0.5f;
				audio.PlayOneShot(LaserSound);
				audio.Play();
			}
			Ray2D ray = new Ray2D(transform.position, transform.right * SignDirectionLaser);
			RaycastHit2D hit;

			_lineRenderer.SetPosition(0, ray.origin);
			
			hit = Physics2D.Raycast(ray.origin, transform.right * SignDirectionLaser, 100, LaserColliders);
			if (hit)
			{

				Debug.DrawLine(Vector3.zero, hit.point, Color.blue);

				//print("Punto de choque: " + hit.point);
				_lineRenderer.SetPosition(1, hit.point);

				var particleSystem = LaserSparkEffect.GetComponent<ParticleSystem>();
				if (!particleSystem.isPlaying)
				{
					Instantiate(LaserSparkEffect, hit.point, Quaternion.identity);
				}

				if (hit.collider.tag == "Enemy")
				{
					hit.collider.gameObject.GetComponent<IAEnemy>().ApplyDamage(LaserDamage, hit.point - (Vector2)transform.position);
				}
			}
			else
			{
				_lineRenderer.SetPosition(1, ray.GetPoint(100));
			}
			yield return null;
		}
		audio.Stop();
		_lineRenderer.enabled = false;
	}
}