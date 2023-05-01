using Prototype.Data;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Prototype.Model
{
    public class WeaponModel : MonoBehaviour
    {
        private GameplaySessionData _session;

        [Inject]
        public void Construct(GameplaySessionData session)
        {
            _session = session;
        }

        [SerializeField] private IdData _id;
        public IdData Id
        {
            get => _id;
            private set { _id = value; }
        }

        public float AttackRange
        {
            get
            {
                var weapon = _session.Data.Weapons.FirstOrDefault(_ => _.IdSession.Equals(Id));
                return weapon.AttackRange;
            }
            set
            {
                var weapon = _session.Data.Weapons.FirstOrDefault(_ => _.IdSession.Equals(Id));
                weapon.AttackRange = value;
            }
        }
    }
}

