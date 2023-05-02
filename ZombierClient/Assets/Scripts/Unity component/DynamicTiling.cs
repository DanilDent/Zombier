using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DynamicTiling : MonoBehaviour
{
    public Material SharedMaterial;
    private Material _material;
    private MeshRenderer _renderer;
    private string _textureName = "_MainTex";

    void OnEnable()
    { 
        _renderer = GetComponent<MeshRenderer>();
        _material = new Material(SharedMaterial);
        transform.hasChanged = false;
    }

    void Update()
    {
        _material.SetTextureScale(_textureName,
           new Vector2(
           SharedMaterial.mainTextureScale.x * transform.localScale.x,
           SharedMaterial.mainTextureScale.y * transform.localScale.y));
        _renderer.sharedMaterial = _material;
        transform.hasChanged = false;
    }

    void OnDestroy()
    {
        DestroyImmediate(_material);
    }
}
