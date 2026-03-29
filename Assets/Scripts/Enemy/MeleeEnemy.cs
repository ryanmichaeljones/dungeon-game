namespace Assets.Scripts
{
    public class MeleeEnemy : Enemy
    {
        protected override void Awake()
        {
            _moveSpeed = 3.5f;
            _detectionRange = 8f;
            _attackRange = 1.5f;
            _attackDamage = 10;
            _attackCooldown = 1f;
            base.Awake();
        }
    }
}
