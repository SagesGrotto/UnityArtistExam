using UnityEditor;
using UnityEngine;

public class Controller : MonoBehaviour
{
    enum State
    {
        Selection,
        Transition,
        Entry
    }

    public Camera MainCamera;
    public Transform OriginalPosition;

    public float TransitionTime = 2.0f;

    public InfoPanel Panel;
    public RectTransform ShortcutButtonPlace;
    public ShortcutButton ShortcutButtonPrefab;
    
    private State m_CurrentState;
    private SelectableObject m_CurrentObject;

    private SelectableObject m_HoveredObject;
    private int m_HoveredOldLayer;

    private float m_TransitionSpeed;
    private float m_TransitionValue;
    private Transform m_StartingSelectionTransform;
    private Transform m_EndingSelectionTransform;

    private void Start()
    {
        MainCamera.transform.SetPositionAndRotation(OriginalPosition.position, OriginalPosition.rotation);
        Panel.SetupCancelAction(ReturnToStart);
        
        //get all places to list them
        var objs = FindObjectsOfType<SelectableObject>();
        foreach (var obj in objs)
        {
            var newButton = Instantiate(ShortcutButtonPrefab, ShortcutButtonPlace);
            newButton.Name.text = obj.Title;
            newButton.Button.onClick.AddListener(() =>
            {
                m_CurrentObject = obj;
                GoToChoice();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_CurrentState)
        {
            case State.Selection:
                SelectionState();
                break;
            case State.Entry:
                EntryState();
                break;
            case State.Transition:
                TransitionState();
                break;
        }
    }

    void SelectionState()
    {
        RaycastHit hit;
        Ray r = MainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out hit, float.MaxValue, 
            (1 << Helpers.HubStationLayer) | (1 << Helpers.HighlightingLayer)))
        {
            var obj = hit.collider.gameObject.GetComponentInParent<SelectableObject>();

            if (obj != m_CurrentObject)
            {
                if (m_CurrentObject != null)
                {
                    Helpers.RecursiveLayerSet(m_CurrentObject.transform, m_HoveredOldLayer);
                }

                m_CurrentObject = obj;
                m_HoveredOldLayer = m_CurrentObject.gameObject.layer;
                Helpers.RecursiveLayerSet(m_CurrentObject.transform, 8);
            }
        }
        else
        {
            if (m_CurrentObject != null)
            {
                Helpers.RecursiveLayerSet(m_CurrentObject.transform, m_HoveredOldLayer);
                m_CurrentObject = null;
            }
        }
        
        if (m_CurrentObject != null && Input.GetMouseButtonDown(0))
        {
            GoToChoice();
        }
    }

    void TransitionState()
    {
        m_TransitionValue = Mathf.Clamp01(m_TransitionValue + Time.smoothDeltaTime * m_TransitionSpeed);
        Vector3 position = Vector3.Lerp(m_StartingSelectionTransform.position, m_EndingSelectionTransform.position,
            m_TransitionValue);
        Quaternion rotation = Quaternion.Lerp(m_StartingSelectionTransform.rotation,
            m_EndingSelectionTransform.rotation, m_TransitionValue);
        
        MainCamera.transform.SetPositionAndRotation(position, rotation);

        if (Mathf.Approximately(m_TransitionValue, 1.0f))
        {
            //we got back from a choice
            if (m_CurrentObject == null) m_CurrentState = State.Selection;
            else m_CurrentState = State.Entry;
        }
    }

    void EntryState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToStart();
        }
    }

    void ReturnToStart()
    {
        m_StartingSelectionTransform = m_CurrentObject.CameraPosition;
        m_EndingSelectionTransform = OriginalPosition;
        m_TransitionValue = 0.0f;
        m_TransitionSpeed = 1.0f / TransitionTime;
        m_CurrentState = State.Transition;
        m_CurrentObject = null;
        
        Panel.Close(m_TransitionSpeed);
        
        ShortcutButtonPlace.gameObject.SetActive(true);
    }
    

    void GoToChoice()
    {
        m_CurrentState = State.Transition;
        m_TransitionValue = 0.0f;
        m_TransitionSpeed = 1.0f / TransitionTime;
        m_StartingSelectionTransform = OriginalPosition;
        m_EndingSelectionTransform = m_CurrentObject.CameraPosition;
        
        Panel.Open(m_TransitionSpeed, m_CurrentObject.Title, m_CurrentObject.Description, m_CurrentObject.LevelToLoad);
        
        ShortcutButtonPlace.gameObject.SetActive(false);
        
        Helpers.RecursiveLayerSet(m_CurrentObject.transform, m_HoveredOldLayer);
    }
}
