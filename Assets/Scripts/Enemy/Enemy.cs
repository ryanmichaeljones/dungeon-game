using UnityEngine;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected float _moveSpeed = 3f;
        [SerializeField] protected float _detectionRange = 8f;
        [SerializeField] protected float _attackRange = 1.5f;
        [SerializeField] protected float _attackCooldown = 1f;
        [SerializeField] protected int _attackDamage = 10;

        protected Rigidbody _rb;
        protected Health _health;
        protected Transform _player;
        protected float _lastAttackTime;
        protected EnemyState _state = EnemyState.Idle;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _health = GetComponent<Health>();
            _health.OnDeath += OnDeath;
        }

        protected virtual void Start()
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
                _player = player.transform;
        }

        protected virtual void Update()
        {
            if (_state == EnemyState.Dead || _player == null) return;

            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(_player.position.x, 0f, _player.position.z));

            if (distance <= _attackRange)
                _state = EnemyState.Attack;
            else if (distance <= _detectionRange)
                _state = EnemyState.Chase;
            else
                _state = EnemyState.Idle;

            switch (_state)
            {
                case EnemyState.Idle:
                    ExecuteIdle();
                    break;
                case EnemyState.Chase:
                    ExecuteChase();
                    break;
                case EnemyState.Attack:
                    ExecuteAttack();
                    break;
            }
        }

        protected virtual void ExecuteIdle() { }

        protected virtual void ExecuteChase()
        {
            Vector3 direction = (_player.position - transform.position);
            direction.y = 0f;
            direction.Normalize();

            _rb.MovePosition(_rb.position + direction * _moveSpeed * Time.deltaTime);
            transform.forward = direction;
        }

        protected virtual void ExecuteAttack()
        {
            if (Time.time < _lastAttackTime + _attackCooldown) return;

            _lastAttackTime = Time.time;

            Health playerHealth = _player.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(_attackDamage);
        }

        private void OnDeath()
        {
            _state = EnemyState.Dead;
            Destroy(gameObject, 0.1f);
        }
    }
}
