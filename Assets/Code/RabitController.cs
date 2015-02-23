using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RabitController : MonoBehaviour
{


	public float Speed;
	public float JumpForce;

	//Que se condidera ground
	public LayerMask WhatIsGround;
	public Transform GroundCheck;

	private bool _grounded;
	private bool _facingRight;
	private Animator _animator;
	private LaserController _laserController;


	//Stairs
	private bool _climbingStairs;
	private bool _inStairs;				//Si el personaje se encuentra ante una escalera o no
	private GameObject _stair;			//Posible escalera para escalar
	private Collider2D _onGroundPlatform;
	private bool _onBottomStairs = false;

	//Lives
	public Image healthImage;
	private float _health = 1.0f;

	//Sounds
	public AudioClip hurtSound;


	void Start()
	{
		_facingRight = true;
		_animator = GetComponent<Animator>();
		_grounded = true;
		_climbingStairs = false;
		_laserController = FindObjectOfType<LaserController>();
	}


	public void FixedUpdate()
	{
		if (!_climbingStairs)
		{
			var movement = Input.GetAxis("Horizontal");
			rigidbody2D.velocity = new Vector2(movement * Speed, rigidbody2D.velocity.y);


			//¿Is grounded?
			var groundedCollider = Physics2D.OverlapCircle(GroundCheck.position, 0.4f, WhatIsGround);
			_grounded = groundedCollider; //Se podría eliminar.
			if (groundedCollider)
			{
				_onGroundPlatform = groundedCollider.collider2D;
			}
			_animator.SetBool("grounded", _grounded);

			_animator.SetFloat("velocity", Mathf.Abs(movement * Time.deltaTime));

			if (movement > 0 && !_facingRight)
			{
				Flip();
			}
			else if (movement < 0 && _facingRight)
			{
				Flip();
			}
		}
		else //En caso de que scale!!
		{
			var movement = Input.GetAxis("Vertical");
			rigidbody2D.velocity = new Vector2(0, movement * Speed);
		}

		_animator.SetBool("climbing", _climbingStairs);
	}

	void Update()
	{
		if (_grounded && !_climbingStairs && Input.GetKeyDown(KeyCode.Space))
		{
			rigidbody2D.AddForce(new Vector2(0, JumpForce));
		}

		if (_health <= 0)
		{
			LevelManager.Instance.ChangeScene("GameOverScene");
		}

		if (_inStairs && ((Input.GetKeyDown(KeyCode.S) && !_onBottomStairs) || Input.GetKeyDown(KeyCode.W)))
		{
			_climbingStairs = true;
		}

		if (transform.position.y < -50f)
		{
			LevelManager.Instance.ChangeScene("GameOverScene");
		}


		//print("Velocidad del cuerpo rigido: " + rigidbody2D.velocity + " Soy trigger: " + rigidbody2D.collider2D.isTrigger 
		//	  + " Delante de una escalera: " + _inStairs + " Estoy escalando: " + _climbingStairs);

	}


	private void Flip()
	{

		var scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		_facingRight = !_facingRight;

		_laserController.SignDirectionLaser = Mathf.Sign(scale.x);
	}


	//En realidad sería TakeDamage
	public void ApplyDamage(float amount)
	{
		audio.PlayOneShot(hurtSound);

		_health -= amount;
		healthImage.fillAmount = _health;
	}

	public void OnCollisionEnter2D(Collision2D coll)
	{

		print(coll.gameObject.tag);

		if (coll.gameObject.tag == "Enemy")
		{
			//rigidbody2D.AddForce(new Vector2(-transform.right.x * 800f, 250f));

		}
	}


	public void OnCollisionStay2D(Collision2D coll)
	{
		if (coll.gameObject.tag == "Enemy")
		{
			//ApplyDamage(0.1f);
			//rigidbody2D.AddForce(new Vector2(-1000f, 400f));
		}
	}


	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Stairs")
		{
			_stair = other.gameObject;
			_inStairs = true;
		}

		if (other.gameObject.tag == "BottomStairs")
		{
			if (_climbingStairs)
			{

				print("NO DEBERIA DE SALIR!");

				_climbingStairs = false;
				_stair = null;
				_inStairs = false;

				foreach (Collider2D coll in gameObject.GetComponents<Collider2D>())
				{
					coll.isTrigger = false;
				}
			}
			else
			{
				_stair = other.gameObject;
				_inStairs = true;
			}
		}
	}


	public void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Stairs")
		{
			if (_climbingStairs)
			{
				print("NO DEBERIA DE SALIR!");
				foreach (Collider2D coll in gameObject.GetComponents<Collider2D>())
				{
					coll.isTrigger = true;
				}
				_stair = other.gameObject;
				_inStairs = true;
			}
		}


		if (other.gameObject.tag == "BottomStairs")
		{
			_onBottomStairs = true;
		}

	}


	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.tag == "Stairs")
		{
			_climbingStairs = false;
			_stair = null;
			_inStairs = false;

			foreach (Collider2D coll in gameObject.GetComponents<Collider2D>())
			{
				coll.isTrigger = false;
			}
		}

		if (other.gameObject.tag == "BottomStairs")
		{
			_onBottomStairs = false;
		}
	}
}