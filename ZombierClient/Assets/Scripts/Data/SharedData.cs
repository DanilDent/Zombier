using System;
using System.Collections.Generic;

namespace Prototype.Data
{
    [Serializable]
    public class SharedData
    {
        public PlayerData Player;
        public List<WeaponData> Weapons = new List<WeaponData>();
        public List<LocationData> Locations = new List<LocationData>();
        public List<EnemyData> Enemies = new List<EnemyData>();
    }
}
