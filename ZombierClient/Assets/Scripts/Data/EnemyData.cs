using Prototype.Service;
using System;

namespace Prototype.Data
{
    [Serializable]
    public class EnemyData : BaseData
    {
        public int Health
        {
            get => _health;
            set
            {
                var old = _health;
                _health = value;
                if (_health < 0)
                {
                    _health = 0;
                }

                GameplayEventService.InvokeOnValueChanged(this, _health, MaxHealth);

                if (old > 0 && _health == 0)
                {
                    GameplayEventService.InvokeOnDeath(this);
                }
                else if (_health > 0 && old >= _health)
                {
                    GameplayEventService.InvokeOnEnemyDamage(old - _health, this);
                }
            }
        }
        public int MaxHealth { get; set; }
        public float Speed { get; set; }
        public float ChaseRange { get; set; }
        public float AttackRange { get; set; }
        public float AttackInterval { get; set; }
        public int Damage { get; set; }

        private int _health;
    }
}