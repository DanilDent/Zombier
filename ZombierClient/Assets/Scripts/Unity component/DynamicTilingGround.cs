using UnityEngine;

[ExecuteInEditMode]
public class DynamicTilingGround : MonoBehaviour
{
    private Material _sharedMaterial;
    private Material _material;
    private MeshRenderer _renderer;

    void OnEnable()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        _sharedMaterial = _renderer.sharedMaterial;
        _material = new Material(_sharedMaterial);
        UpdateTextureScale();
    }

    void Update()
    {
        UpdateTextureScale();
    }

    private void UpdateTextureScale()
    {
        _material.mainTextureScale = new Vector2(
            _sharedMaterial.mainTextureScale.x * transform.localScale.x,
            _sharedMaterial.mainTextureScale.y * transform.localScale.z);
        _renderer.sharedMaterial = _material;
    }

    void OnDestroy()
    {
        DestroyImmediate(_material);
    }
}
