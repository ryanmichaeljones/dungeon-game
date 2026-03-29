using UnityEngine;

namespace Assets.Scripts
{
    public class RangedEnemy : Enemy
    {
        [SerializeField] private Projectile _projectilePrefab;

        protected override void Awake()
        {
            _moveSpeed = 2f;
            _detectionRange = 10f;
            _attackRange = 6f;
            _attackDamage = 10;
            _attackCooldown = 2f;
            base.Awake();
        }

        protected override void ExecuteChase()
        {
            float distance = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(_player.position.x, 0f, _player.position.z));

            if (distance > _attackRange * 0.6f)
                base.ExecuteChase();
            else
            {
                Vector3 direction = (_player.position - transform.position);
                direction.y = 0f;
                if (direction.sqrMagnitude > 0f)
                    transform.forward = direction.normalized;
            }
        }

        protected override void ExecuteAttack()
        {
            if (Time.time < _lastAttackTime + _attackCooldown) return;

            _lastAttackTime = Time.time;

            Vector3 direction = (_player.position - transform.position);
            direction.y = 0f;
            direction.Normalize();

            if (direction.sqrMagnitude > 0f)
                transform.forward = direction;

            if (_projectilePrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.3f + direction * 0.5f;
                Projectile projectile = Instantiate(_projectilePrefab, spawnPos, Quaternion.identity);
                projectile.Initialize(direction);
            }
        }
    }
}
