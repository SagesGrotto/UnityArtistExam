using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GoToDriveButton : MonoBehaviour
{
    public Camera MainCamera;
    public Transform DriverSeatPosition;
    public CanvasGroup Group;

    private RectTransform m_RectTransform;

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 pos = MainCamera.WorldToScreenPoint(DriverSeatPosition.position);
        m_RectTransform.position = pos;
        
        float dist = Vector2.Distance(Input.mousePosition , m_RectTransform.position);
        float distRatio = dist / Screen.width;

        float alpha = Mathf.Lerp(0.2f, 1, 1.0f - Mathf.Clamp01(distRatio / 0.2f));

        Group.alpha = alpha;
    }
}
