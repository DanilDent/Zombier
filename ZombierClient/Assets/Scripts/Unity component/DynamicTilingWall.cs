using UnityEngine;

[ExecuteInEditMode]
public class DynamicTilingWall : MonoBehaviour
{
    private Material _sharedMaterial;
    private Material _material;
    private MeshRenderer _renderer;
    private Transform _gfx;

    void OnEnable()
    {
        _renderer = GetComponentInChildren<MeshRenderer>();
        _sharedMaterial = _renderer.sharedMaterial;
        _material = new Material(_sharedMaterial);
        _gfx = transform.GetChild(0);

        UpdateTextureScale();
    }

    void Update()
    {
        UpdateTextureScale();
    }

    private void UpdateTextureScale()
    {
        _material.mainTextureScale = new Vector2(
            _sharedMaterial.mainTextureScale.x * ((transform.localScale.z > 1f ? transform.localScale.z : transform.localScale.x)),
            _sharedMaterial.mainTextureScale.y * _gfx.localScale.y);
        _renderer.sharedMaterial = _material;
    }

    void OnDestroy()
    {
        DestroyImmediate(_material);
    }
}
