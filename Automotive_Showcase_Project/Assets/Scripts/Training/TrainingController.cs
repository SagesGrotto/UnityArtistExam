using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrainingController : MonoBehaviour
{
    [System.Serializable]
    public class QuestionEntry
    {
        [Multiline] public string Question;
        public SelectableInfoPart Answer;
    }

    enum State
    {
        Learning,
        Answering,
        Answered
    }

    public Camera MainCamera;
    
    public Transform RotatingTransform;
    public float RotatingSpeed;
    public float SelectionClickTime = 0.1f;

    public QuestionEntry[] Questions;
    public bool RandomizeQuestion;
    [Tooltip("-1 mean all question.")]
    public int QuestionCount = -1;

    public Button SwitchModeButton;
    
    [Header("Info Panel")] 
    public GameObject InfoPanel;
    public Text InfoTitle;
    public Text InfoText;

    [Header("Question Panel")] 
    public GameObject QuestionPanel;
    public Text QuestionText;
    public Button NextQuestionButton;
    
    private float m_PressTime;
    private Vector3 m_LastPosition;

    private State m_CurrentState;
    private QuestionEntry m_CurrentEntry;
    private int m_RightAnswer;
    private int m_MaxScore;

    private float m_XAngle = 0, m_YAngle = 0;

    private Text m_SwitchModeButtonText;
    
    private SelectableInfoPart m_Selected = null;
    private int m_OldLayer;
    private List<QuestionEntry> m_LeftOverQuestion = new List<QuestionEntry>();
    
    // Start is called before the first frame update
    void Start()
    {
        InfoPanel.SetActive(false);
        QuestionPanel.SetActive(false);

        m_SwitchModeButtonText = SwitchModeButton.GetComponentInChildren<Text>();
        m_SwitchModeButtonText.text = "Switch to Question";
        
        SwitchModeButton.onClick.AddListener(SwitchToQuestion);

        m_CurrentState = State.Learning;

        UpdateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        //check for selectable
        var ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f,
            (1 << Helpers.SelectableInfoLayer | 1 << Helpers.HighlightingLayer)))
        {
            var info = hit.transform.GetComponentInParent<SelectableInfoPart>();

            if (m_Selected != null)
            {
                Helpers.RecursiveLayerSet(m_Selected.transform, m_OldLayer);
            }
            
            m_Selected = info;
            m_OldLayer = m_Selected.gameObject.layer;
            Helpers.RecursiveLayerSet(m_Selected.transform, Helpers.HighlightingLayer);
        }
        else
        {
            if (m_Selected != null)
            {
                Helpers.RecursiveLayerSet(m_Selected.transform, m_OldLayer);
                m_Selected = null;
            }
        }


        if (Input.GetMouseButtonDown(0))
        {
            m_PressTime = 0.0f;
            m_LastPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            m_PressTime += Time.deltaTime;

            Vector3 delta = Input.mousePosition - m_LastPosition;

            m_XAngle = m_XAngle + delta.y * RotatingSpeed * Time.deltaTime;
            if (m_XAngle > 50) m_XAngle = 50;
            else if (m_XAngle < -50) m_XAngle = -50;

            m_YAngle = m_YAngle + delta.x * RotatingSpeed * Time.deltaTime;
            if (m_YAngle > 360) m_YAngle -= 360;
            else if (m_YAngle < 0) m_YAngle += 360;

            RotatingTransform.localEulerAngles = new Vector3(m_XAngle, m_YAngle, 0);

            UpdateCamera();
            
            m_LastPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && m_Selected != null)
        {
            if (m_PressTime < SelectionClickTime)
            {
                if (m_CurrentState == State.Answering)
                {
                    Helpers.RecursiveLayerSet(m_Selected.transform, m_OldLayer);
                    CheckAnswer();
                }
                else if(m_CurrentState == State.Learning)
                {
                    Helpers.RecursiveLayerSet(m_Selected.transform, m_OldLayer);
                    InfoPanel.SetActive(true);
                    InfoTitle.text = m_Selected.Title;
                    InfoText.text = m_Selected.Description;
                }
            }
        }
    }

    void UpdateCamera()
    {
        MainCamera.transform.position = RotatingTransform.position + RotatingTransform.forward * 1.0f;
        MainCamera.transform.LookAt(RotatingTransform);
    }

    public void SwitchToQuestion()
    {
        m_CurrentState = State.Answering;
        m_RightAnswer = 0;
        m_Selected = null;

        m_LeftOverQuestion.Clear();
        m_LeftOverQuestion.AddRange(Questions);
        
        if(RandomizeQuestion)
            Shuffle(ref m_LeftOverQuestion);

        if (QuestionCount != -1 && QuestionCount < m_LeftOverQuestion.Count)
        {
            int toRemove = m_LeftOverQuestion.Count - QuestionCount;
            m_LeftOverQuestion.RemoveRange(m_LeftOverQuestion.Count - toRemove, toRemove);
        }

        m_MaxScore = m_LeftOverQuestion.Count;
        InfoPanel.gameObject.SetActive(false);
        QuestionPanel.gameObject.SetActive(true);
        
        DisplayNextQuestion();
        m_SwitchModeButtonText.text = "Switch to Learning";
        SwitchModeButton.onClick.AddListener(ExitQuestion);
    }

    public void ExitQuestion()
    {
        m_CurrentState = State.Learning;
        m_Selected = null;
        QuestionPanel.SetActive(false);
        m_SwitchModeButtonText.text = "Switch to Question";
        SwitchModeButton.onClick.AddListener(SwitchToQuestion);
    }

    public void DisplayNextQuestion()
    {
        m_CurrentState = State.Answering;
        NextQuestionButton.gameObject.SetActive(false);
        if (m_LeftOverQuestion.Count == 0)
        {
            m_CurrentState = State.Answered;
            QuestionText.text = $"Congratulations, you finished with a score of {m_RightAnswer}/{m_MaxScore}!\n" +
                                $"Exit the scene now or click on the Learning button to go back to Learning mode";
        }
        else
        {
            m_CurrentEntry = m_LeftOverQuestion[0];
            m_LeftOverQuestion.RemoveAt(0);
            QuestionText.text = m_CurrentEntry.Question;
        }
    }

    public void CheckAnswer()
    {
        m_CurrentState = State.Answered;
        NextQuestionButton.gameObject.SetActive(true);
        
        if (m_Selected == m_CurrentEntry.Answer)
        {
            QuestionText.text = "Right!";
            m_RightAnswer++;
        }
        else
        {
            QuestionText.text = "Wrong!";
        }

        m_Selected = null;
    }
    
    public void Shuffle(ref List<QuestionEntry> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0, n + 1);
            var value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
