﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatingJoystick))]
public class FloatingJoystickEditor : JoystickEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (background != null)
        {
            RectTransform backgroundRect = (RectTransform)background.objectReferenceValue;
            backgroundRect.anchorMax = new Vector2(0.5f, 0f);
            backgroundRect.anchorMin = new Vector2(0.5f, 0f);
            backgroundRect.pivot = center;
        }
    }
}