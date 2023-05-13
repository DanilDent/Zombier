using UnityEngine;
using Zenject;

namespace Prototype.UnityComponent
{
    public class LookAtCamera : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(Camera camera)
        {
            _camera = camera;
        }

        // Private

        private enum Mode
        {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted
        }

        //Injected
        private Camera _camera;
        //
        [SerializeField] private Mode _mode;

        private void LateUpdate()
        {
            switch (_mode)
            {
                case Mode.LookAt:
                    transform.LookAt(_camera.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCam = (transform.position - _camera.transform.position).normalized;
                    transform.LookAt(transform.position + dirFromCam);
                    break;
                case Mode.CameraForward:
                    transform.forward = _camera.transform.forward;
                    break;
                case Mode.CameraForwardInverted:
                    transform.forward = -_camera.transform.forward;
                    break;
            }
        }
    }
}
