using UnityEngine;

namespace Assets.Scripts
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 8f;
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _lifetime = 3f;

        private Vector3 _direction;

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        private void Start()
        {
            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (health != null && other.GetComponent<Player>() != null)
            {
                health.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }

            if (other.GetComponent<Enemy>() == null && other.GetComponent<Projectile>() == null)
                Destroy(gameObject);
        }
    }
}
