using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	public float Speed = 10f;
	public float Damage;

	private Vector2 _dir;
	private RabitController _rabittController;
	private float _lifeTime = 0f;


	public void Start()
	{
		_rabittController = FindObjectOfType<RabitController>();
	}

	public void Update()
	{
		_lifeTime += Time.deltaTime;
		if (_lifeTime > 8.0f)
		{
			Destroy(this.gameObject);
		}
	}


	public void OnBecameInvisible()
	{
		Destroy(this.gameObject);
	}

	public void OnCollisionEnter2D(Collision2D coll)
	{

		Debug.Log(_rabittController == null);


		if (coll.gameObject.tag == "Rabitt")
		{
			_rabittController.ApplyDamage(Damage);
			Destroy(this.gameObject);
		}
	}
}
