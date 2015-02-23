using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IAEnemy : MonoBehaviour {


	public enum EnemyState {Looking, Moving, RabittDetected}

	public float Speed = 10f;

	public float VisionDistance = 25f;
	public LayerMask DetectablesLayers;


	public Transform BulletSpawner;
	public GameObject Bullet;
	public float TimeToShoot = 0.5f;
	private float _timeLastShoot = 0.0f;

	private bool _raycastDone;


	private float _direction;
	private EnemyState _state;
	private EnemyState _lastState;

	//Sounds
	public AudioClip ShootSound;
	public AudioClip ExplosionSound;



	//Health
	public Slider SliderHealth;
	public float _health = 100f;

	//Effects
	public GameObject DeadEffect;

	private Animator _animator;

	private bool _facingRight;
	
	void Start () {
		_direction = -1;
		_state = EnemyState.Moving;
		_facingRight = false;
		_animator = GetComponent<Animator>();
		_raycastDone = false;
	}


	public void FixedUpdate()
	{
		if (_state == EnemyState.Moving)
		{
			StopCoroutine("WaitLookTime");
			rigidbody2D.velocity = new Vector2(Speed * _direction, rigidbody2D.velocity.y);
		}
		else if(_state == EnemyState.Looking)
		{
			StartCoroutine("WaitLookTime");
			rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
		}
		else if (_state == EnemyState.RabittDetected)
		{
			StopCoroutine("WaitLookTime");
			rigidbody2D.velocity = new Vector2(0, rigidbody2D.velocity.y);
			_timeLastShoot += Time.deltaTime;
			if(_timeLastShoot >= TimeToShoot){
				Shoot();
				_timeLastShoot = 0;
			}
		}
	}


	private void Shoot()
	{
		audio.PlayOneShot(ShootSound);
		var projectile = (GameObject)Instantiate(Bullet, BulletSpawner.transform.position, BulletSpawner.rotation);
		projectile.rigidbody2D.AddForce(Vector2.right * _direction * 800f);
	}

	private IEnumerator WaitLookTime()
	{
		for (float timer = 3.0f; timer > 0; timer -= Time.deltaTime)
		{
			yield return 0;
		}
		SetState(EnemyState.Moving);
		_direction = -_direction;
	}

	private void SetState(EnemyState newState)
	{
		if (_lastState != _state && _lastState != EnemyState.RabittDetected)
			_lastState = _state;
		_state = newState;

	}

	void Update () {


		if (_direction > 0 && !_facingRight)
		{
			Flip();
		}
		else if(_direction < 0  && _facingRight){
			Flip();
		}

		//Raycasting for detect rabbit
		Ray2D ray = new Ray2D(transform.position, transform.right * _direction);
		RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, transform.right * _direction, VisionDistance, DetectablesLayers);
		Debug.DrawRay(ray.origin, transform.right * _direction * VisionDistance, Color.green);

		if (hit2D)
		{
			if (hit2D.collider.tag == "Rabitt")
			{
				SetState(EnemyState.RabittDetected);
				_raycastDone = true;
			}
		}
		else
		{

			if (_state == EnemyState.RabittDetected)
			{
				Ray2D bigRay = new Ray2D(transform.position, transform.right * _direction);
				RaycastHit2D bigHit2D = Physics2D.Raycast(bigRay.origin, transform.right * _direction, VisionDistance * 4, DetectablesLayers);
				Debug.DrawRay(bigRay.origin, transform.right * _direction * VisionDistance * 4, Color.blue);

				if (bigHit2D)
				{
					if (!(bigHit2D.collider.tag == "Rabitt"))
					{
						SetState(EnemyState.Moving);
					}
				}
				else
				{
					SetState(EnemyState.Moving);
				}
			}

			if (_raycastDone)
			{
				SetState(EnemyState.Moving);
				_raycastDone = false;
			}
		}

		//Destroy if dead
		if (_health <= 0)
		{
			Instantiate(DeadEffect, transform.position, Quaternion.identity);


			AudioSource.PlayClipAtPoint(ExplosionSound, transform.position);


			foreach (Transform item in transform)
			{
				if (item.name == "Head")
				{
					item.parent = null;
					var rigidbody2D = item.gameObject.AddComponent<Rigidbody2D>();
					item.gameObject.AddComponent<DestroyHead>();

					rigidbody2D.AddForce(new Vector2(350, 450));
					rigidbody2D.angularVelocity = Random.Range(0, 181);
					
					item.gameObject.AddComponent<CircleCollider2D>();
					item.gameObject.layer = 13;
					item.gameObject.tag = "Head";

					
					break;
				}
			}
			
			LevelManager.Instance.AddMurderer();
			Destroy(this.gameObject);
		}

		if (transform.position.y <= -60)
		{
			Destroy(this.gameObject);
		}


		_animator.SetBool("Looking", _state == EnemyState.Looking);
		_animator.SetBool("Moving", _state == EnemyState.Moving);
		_animator.SetBool("RabittDetected", _state == EnemyState.RabittDetected);

	}

	private void Flip()
	{
		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		_facingRight = !_facingRight;
	}


	public void ApplyDamage(float amount, Vector2 dirDamage)
	{
		_health -= amount;
		SliderHealth.value = _health;
		if (!(_state == EnemyState.RabittDetected))
		{
			_direction = Mathf.Sign(-dirDamage.normalized.x);
			SetState(EnemyState.RabittDetected);
		}
	}


	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Edge")
		{
			SetState(EnemyState.Looking);
		}
	}
}
