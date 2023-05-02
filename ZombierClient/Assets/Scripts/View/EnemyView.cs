using UnityEngine;
using Zenject;

namespace Prototype.View
{
    public class EnemyView : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyView>
        {
        }
    }

}