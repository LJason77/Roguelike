using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
	[FormerlySerializedAs("Smoothing")] public float smoothing = 1;
	[FormerlySerializedAs("RestTime")]  public float restTime  = 1;
	private                                    float _restTimer;

	[FormerlySerializedAs("ChopAudioClip")]
	public AudioClip[] chopAudioClip;

	[FormerlySerializedAs("StepAudioClip")]
	public AudioClip[] stepAudioClip;

	[FormerlySerializedAs("SodaAudioClip")]
	public AudioClip[] sodaAudioClip;

	[FormerlySerializedAs("FoodAudioClip")]
	public AudioClip[] foodAudioClip;

	[FormerlySerializedAs("TargetPos")] [HideInInspector]
	public Vector2 targetPos = new Vector2(1, 1);

	private                 Rigidbody2D   _rigidbody2D;
	private                 BoxCollider2D _collider2D;
	private                 Animator      _animator;
	private                 bool          _moved;
	private static readonly int           Attack = Animator.StringToHash("Attack");
	private static readonly int           Damage = Animator.StringToHash("Damage");

	void Start()
	{
		_rigidbody2D = GetComponent<Rigidbody2D>();
		_collider2D  = GetComponent<BoxCollider2D>();
		_animator    = GetComponent<Animator>();
	}

	void Update()
	{
		if (GameManager.Instance.food <= 0)
		{
			GameManager.Instance.Die();
		}

		if (GameManager.Instance.food <= 0 || GameManager.Instance.isEnd)
		{
			return;
		}

		_rigidbody2D.MovePosition(Vector2.Lerp(transform.position, targetPos, smoothing * Time.deltaTime));
		_restTimer += Time.deltaTime;
		if (_restTimer < restTime)
		{
			return;
		}

		int h = (int) Input.GetAxis("Horizontal");
		int v = (int) Input.GetAxis("Vertical");

		if (h > 0)
		{
			v = 0;
		}


		if (h != 0 || v != 0)
		{
			GameManager.Instance.ReduceFood(1);
			// 检测
			_collider2D.enabled = false;
			RaycastHit2D hit2D = Physics2D.Linecast(targetPos, targetPos + new Vector2(h, v));
			_collider2D.enabled = true;

			if (hit2D.transform == null)
			{
				targetPos += new Vector2(h, v);
				AudioManager.Instance.RandomPlay(stepAudioClip);
			}
			else
			{
				switch (hit2D.collider.tag)
				{
					case "OutWall":
						break;
					case "Wall":
						_animator.SetTrigger(Attack);
						AudioManager.Instance.RandomPlay(chopAudioClip);
						hit2D.collider.SendMessage("TakeDamage");
						break;
					case "Food":
						GameManager.Instance.AddFood(10);
						targetPos += new Vector2(h, v);
						AudioManager.Instance.RandomPlay(stepAudioClip);
						Destroy(hit2D.transform.gameObject);
						AudioManager.Instance.RandomPlay(foodAudioClip);
						break;
					case "Soda":
						GameManager.Instance.AddFood(15);
						targetPos += new Vector2(h, v);
						AudioManager.Instance.RandomPlay(stepAudioClip);
						Destroy(hit2D.transform.gameObject);
						AudioManager.Instance.RandomPlay(sodaAudioClip);
						break;
					case "Enemy":
						_animator.SetTrigger(Attack);
						AudioManager.Instance.RandomPlay(chopAudioClip);
						hit2D.collider.SendMessage("TakeDamage");
						break;
				}
			}

			_moved = true;

			// 不管攻击或移动，都需要休息
			_restTimer = 0;
		}
	}

	private void LateUpdate()
	{
		if (_moved)
		{
			GameManager.Instance.OnPlayerMove();
		}

		_moved = false;
	}

	public void TakeDamage(int lossFood)
	{
		GameManager.Instance.ReduceFood(lossFood);
		_animator.SetTrigger(Damage);
	}
}