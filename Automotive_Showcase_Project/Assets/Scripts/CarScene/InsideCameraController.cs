using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsideCameraController : MonoBehaviour
{
    enum State
    {
        Off,
        LerpIn,
        LerpOut,
        Active
    }
    
    public Camera TargetCamera;
    public CarCameraControl CarCameraControl;
    public float LerpSpeed = 1.0f;
    public float VerticalSpeed = 45.0f;
    public float HorizontalSpeed = 45.0f;
    public Button AccessButton;
    public Button ExitButton;
    
    private Vector3 m_LerpStart;
    private Vector3 m_LerpEnd;
    private Vector3 m_ForwardLerpStart;
    private Vector3 m_ForwardLerpEnd;
    
    private float m_LerpValue = 0.0f;
    private State m_State = State.Off;
    
    public Vector2 m_CurrentRotation;
    private Vector3 m_PreviousMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        AccessButton.onClick.AddListener(() =>
        {
            AccessButton.gameObject.SetActive(false);
            GoTo();
        });
        
        ExitButton.onClick.AddListener(() =>
        {
            ExitButton.gameObject.SetActive(false);
            Exit();
        });

        ExitButton.gameObject.SetActive(false);
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case State.LerpIn:
            case State.LerpOut:
                m_LerpValue = Mathf.Clamp01(m_LerpValue + Time.deltaTime * LerpSpeed);
                TargetCamera.transform.position = Vector3.Lerp(m_LerpStart, m_LerpEnd, m_LerpValue);
                TargetCamera.transform.forward = Vector3.Lerp(m_ForwardLerpStart, m_ForwardLerpEnd, m_LerpValue);
                if (Mathf.Approximately(m_LerpValue, 1.0f))
                {
                    if (m_State == State.LerpOut)
                    {
                        enabled = false;
                        CarCameraControl.enabled = true;
                        AccessButton.gameObject.SetActive(true);
                        m_State = State.Off;
                    }
                    else
                    {
                        m_State = State.Active;
                        ExitButton.gameObject.SetActive(true);
                    }
                }

                break;
            case State.Active :
                HandleInput();
                TargetCamera.transform.position = transform.position;
                TargetCamera.transform.forward = transform.forward;
                break;
        }
    }

    void HandleInput()
    {
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
    }
    
    void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(
            Mathf.Lerp(-89, 89, m_CurrentRotation.y),
            Mathf.Lerp(0, 360, m_CurrentRotation.x),
            0);
    }

    public void GoTo()
    {
        m_State = State.LerpIn;
     
        m_CurrentRotation = new Vector2(0, 0.5f);
        UpdateRotation();
        
        m_LerpValue = 0.0f;
        enabled = true;
        CarCameraControl.enabled = false;

        m_LerpStart = TargetCamera.transform.position;
        m_LerpEnd = transform.position;

        m_ForwardLerpStart = TargetCamera.transform.forward;
        m_ForwardLerpEnd = transform.forward;
    }

    public void Exit()
    {
        m_State = State.LerpOut;
        m_LerpValue = 0.0f;
        
        m_LerpStart = transform.position;
        m_ForwardLerpStart = transform.forward;
        
        CarCameraControl.CurrentTarget(out m_LerpEnd, out m_ForwardLerpEnd);
    }
}
