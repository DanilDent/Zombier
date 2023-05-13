using UnityEngine;

namespace Prototype.UnityComponent
{
    public class BindTransforms : MonoBehaviour
    {
        public Transform VirtualParentTransform;


        private void Awake()
        {
            if (VirtualParentTransform != null)
            {
                transform.position = VirtualParentTransform.position;
                transform.rotation = VirtualParentTransform.rotation;
                transform.localScale = VirtualParentTransform.localScale;
            }
        }

        private void Update()
        {
            if (VirtualParentTransform != null)
            {
                transform.position = VirtualParentTransform.position;
                transform.rotation = VirtualParentTransform.rotation;
                transform.localScale = VirtualParentTransform.localScale;
            }
        }
    }
}
