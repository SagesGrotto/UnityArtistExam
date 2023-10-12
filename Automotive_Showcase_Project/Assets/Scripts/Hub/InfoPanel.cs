using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    enum State
    {
        Opening,
        Closing,
        Visible,
        Hidden
    }
    
    public Text Title;
    public Text Description;
    public Button Load;
    public Button Cancel;

    private float m_CurrentScale;
    private float m_Speed;
    private State m_CurrentState;

    private Vector3 m_StartingPosition;

    private void Start()
    {
        m_CurrentState = State.Hidden;
        
        RectTransform t = transform as RectTransform;
        t.localPosition -= Vector3.right * t.rect.width;
        m_StartingPosition = t.localPosition;

        //transform.localScale = new Vector3(0, 1,1);
        m_CurrentScale = 0;
        gameObject.SetActive(false);

        Cancel.interactable = false;
        Load.interactable = false;
    }

    private void Update()
    {
        RectTransform t = transform as RectTransform;
        
        switch (m_CurrentState)
        {
            case State.Closing:
                m_CurrentScale = Mathf.Clamp01(m_CurrentScale - Time.smoothDeltaTime * m_Speed);
                
                t.localPosition = m_StartingPosition + Vector3.right * (t.rect.width * m_CurrentScale);
                
                if (Mathf.Approximately(m_CurrentScale, 0.0f))
                {
                    m_CurrentState = State.Hidden;
                    gameObject.SetActive(false);
                }
                break;
            case State.Opening:
                m_CurrentScale = Mathf.Clamp01(m_CurrentScale + Time.smoothDeltaTime * m_Speed);

                t.localPosition = m_StartingPosition + Vector3.right * (t.rect.width * m_CurrentScale);
                
                if (Mathf.Approximately(m_CurrentScale, 1.0f))
                {
                    m_CurrentState = State.Visible;
                    
                    Cancel.interactable = true;
                    Load.interactable = true;
                }
                break;
            case State.Hidden:
            case State.Visible: 
                break;
        }
    }

    public void SetupCancelAction(UnityAction canceled)
    {
        Cancel.onClick.RemoveAllListeners();
        Cancel.onClick.AddListener(canceled);
    }
    
    public void Open(float speed, string title, string description, int sceneLoaded)
    {
        m_CurrentState = State.Opening;
        m_Speed = speed;

        Title.text = title;
        Description.text = description;
        
        gameObject.SetActive(true);

        if (sceneLoaded == -1)
        {
            Load.gameObject.SetActive(false);
        }
        else
        {
            Load.gameObject.SetActive(true);
            Load.onClick.RemoveAllListeners();
            Load.onClick.AddListener(() => { SceneManager.LoadScene(sceneLoaded); });
        }
    }

    public void Close(float speed)
    {
        m_CurrentState = State.Closing;
        m_Speed = speed;
        
        Cancel.interactable = false;
        Load.interactable = false;
    }
}
