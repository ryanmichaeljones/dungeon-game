using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private int _attackDamage = 20;
        [SerializeField] private float _attackRange = 1.2f;
        [SerializeField] private float _attackCooldown = 0.4f;
        [SerializeField] private LayerMask _enemyLayer;

        private float _lastAttackTime;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= _lastAttackTime + _attackCooldown)
                Attack();
        }

        private void Attack()
        {
            _lastAttackTime = Time.time;

            Vector3 attackCenter = transform.position + transform.forward * _attackRange * 0.5f;
            float attackRadius = _attackRange * 0.5f;

            Collider[] hits = Physics.OverlapSphere(attackCenter, attackRadius, _enemyLayer);

            foreach (Collider hit in hits)
                hit.GetComponent<Health>()?.TakeDamage(_attackDamage);
        }
    }
}
