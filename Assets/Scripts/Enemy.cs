using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
	private Vector2       _targetPosition;
	private Transform     _player;
	private Rigidbody2D   _rigidbody2D;
	private BoxCollider2D _collider2D;
	private Animator      _animator;

	[FormerlySerializedAs("Smoothing")] public float smoothing = 3;
	[FormerlySerializedAs("LossFood")]  public int   lossFood  = 10;
	[FormerlySerializedAs("Hp")]        public int   hp        = 2;

	[FormerlySerializedAs("AttackAudioClip")]
	public AudioClip attackAudioClip;

	private static readonly int Attack = Animator.StringToHash("Attack");

	void Start()
	{
		_player         = GameObject.FindGameObjectWithTag("Player").transform;
		_rigidbody2D    = GetComponent<Rigidbody2D>();
		_collider2D     = GetComponent<BoxCollider2D>();
		_animator       = GetComponent<Animator>();
		_targetPosition = transform.position;
		GameManager.Instance.enemiesList.Add(this);
	}

	void Update()
	{
		_rigidbody2D.MovePosition(Vector2.Lerp(transform.position, _targetPosition, smoothing * Time.deltaTime));
	}

	// 自身受到攻击的时候
	public void TakeDamage()
	{
		hp -= 1;

		if (hp <= 0)
		{
			GameManager.Instance.enemiesList.Remove(this);
			Destroy(gameObject);
		}
	}

	public void Move()
	{
		Vector2 offset = _player.position - transform.position;
		if (offset.magnitude < 1.1f)
		{
			// 攻击
			_animator.SetTrigger(Attack);
			AudioManager.Instance.RandomPlay(attackAudioClip);
			_player.SendMessage("TakeDamage", lossFood);
		}
		else
		{
			int x = 0, y = 0;
			// 追玩家
			if (Mathf.Abs(offset.y) > Mathf.Abs(offset.x))
			{
				// 按 Y 轴移动
				y = offset.y < 0 ? -1 : 1;
			}
			else
			{
				// 按 X 轴移动
				x = offset.x > 0 ? 1 : -1;
			}

			// 设置目标位置之前先做检测
			_collider2D.enabled = false;
			RaycastHit2D hit2D =
				Physics2D.Linecast(_targetPosition, _targetPosition + new Vector2(x + 0.2f, y + 0.2f), 1);
			_collider2D.enabled = true;
			if (hit2D.transform == null)
			{
				_targetPosition += new Vector2(x, y);
			}
			else
			{
				if (hit2D.collider.CompareTag("Food") || hit2D.collider.CompareTag("Soda"))
				{
					_targetPosition += new Vector2(x, y);
				}
			}
		}
	}
}