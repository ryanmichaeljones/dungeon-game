using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private int _currentHealth;

        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= 0;

        public event Action<int> OnDamaged;
        public event Action OnDeath;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(int amount)
        {
            if (IsDead) return;

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);
            OnDamaged?.Invoke(amount);

            if (_currentHealth <= 0)
                OnDeath?.Invoke();
        }

        public void Heal(int amount)
        {
            _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
        }
    }
}
