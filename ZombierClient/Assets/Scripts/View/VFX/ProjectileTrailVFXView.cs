using Prototype.Data;
using UnityEngine;

namespace Prototype.View
{
    public class ProjectileTrailVFXView : MonoBehaviour
    {
        public ProjectileTrailData ProjectileTrailData;

        private TrailRenderer _trail;

        private void Awake()
        {
            _trail = GetComponentInChildren<TrailRenderer>();
        }

        private void Start()
        {
            ConfigureTrail();
        }

        private void ConfigureTrail()
        {
            if (_trail != null && ProjectileTrailData != null)
            {
                ProjectileTrailData.SetupTrail(_trail);
            }
        }

        // This will be used when object pooling is implemented

        //[SerializeField] private Renderer _renderer;
        //private bool IsDisabling = false;
        //protected const string DISABLE_METHOD_NAME = "Disable";
        //protected const string DO_DISABLE_METHOD_NAME = "DoDisable";

        //private void Awake()
        //{
        //    _trail = GetComponent<TrailRenderer>();
        //}

        //private void OnEnable()
        //{
        //    _renderer.enabled = true;
        //    IsDisabling = false;
        //    // CancelInfoke(DISABLE_METHOD_NAME);
        //    ConfigureTrail();
        //    // Invoke(DISABLE_METHOD_NAME, AutoDestroyTime);
        //}

        //private void OnDisable()
        //{

        //}

        //// This will be needed when object pooling for projectiles is implemented
        //protected void Disable()
        //{
        //    CancelInvoke(DISABLE_METHOD_NAME);
        //    CancelInvoke(DO_DISABLE_METHOD_NAME);
        //    _renderer.enabled = false;

        //    if (_trail != null && ProjectileTrailSO != null)
        //    {
        //        IsDisabling = true;
        //        Invoke(DO_DISABLE_METHOD_NAME, ProjectileTrailSO.Time);
        //    }
        //    else
        //    {
        //        DoDisable();
        //    }

        //    gameObject.SetActive(false);
        //}

        //protected void DoDisable()
        //{
        //    if (_trail != null && ProjectileTrailSO != null)
        //    {
        //        _trail.Clear();
        //    }

        //    gameObject.SetActive(false);
        //}

        /////////////////////////////////////////////////////////////////////
    }
}
