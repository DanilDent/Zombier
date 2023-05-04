using Prototype.Model;
using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Player Data", menuName = "Data/Player Data")]
    public class PlayerData : ScriptableObject
    {
        public PlayerModel PlayerPrefab;
        public int Health;
        public int MaxHealth;
        public float Speed;
        public WeaponData Weapon;
    }
}