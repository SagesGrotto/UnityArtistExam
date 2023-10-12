using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraControl : MonoBehaviour
{
    public Transform CameraTarget;
    public Vector2 DistanceRange;
    public float ZoomSpeed;
    public float VerticalSpeed = 45.0f;
    public float HorizontalSpeed = 45.0f;
    
    private float m_CurrentDistance;
    public Vector2 m_CurrentRotation;
    private Vector3 m_PreviousMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentDistance = 0.5f;
        m_CurrentRotation = new Vector2(0.37f, 0.16f);
        UpdateRotation();
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentDistance = Mathf.Clamp01(m_CurrentDistance - Input.mouseScrollDelta.y * ZoomSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
            m_PreviousMousePosition = Input.mousePosition;
        
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = m_PreviousMousePosition - Input.mousePosition;
            //transform delta into screen size ratio for coherency across screen size
            delta.x /= Screen.width;
            delta.y /= Screen.height;

            m_CurrentRotation.x = Helpers.Wrap01(m_CurrentRotation.x - delta.x * HorizontalSpeed * Time.deltaTime);
            m_CurrentRotation.y = Mathf.Clamp01(m_CurrentRotation.y + delta.y * VerticalSpeed * Time.deltaTime);

            UpdateRotation();
            
            m_PreviousMousePosition = Input.mousePosition;
        }

        Vector3 pos, forward;
        CurrentTarget(out pos, out forward);
        transform.forward = forward;
        transform.position = pos;
    }

    void UpdateRotation()
    {
        CameraTarget.rotation = Quaternion.Euler(
            Mathf.Lerp(0, 89, m_CurrentRotation.y),
            Mathf.Lerp(0, 360, m_CurrentRotation.x),
            0);
    }

    public void CurrentTarget(out Vector3 position, out Vector3 forward)
    {
        forward = CameraTarget.transform.forward;
        position = CameraTarget.transform.position - forward * Mathf.Lerp(DistanceRange.x, DistanceRange.y, m_CurrentDistance);
    }
}
