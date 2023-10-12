using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabHandler : MonoBehaviour
{
    [System.Serializable]
    public class TabEntry
    {
        public Button TabButton;
        public Transform Tab;
    }

    public TabEntry[] Tabs;
    
    private int m_CurrentTab;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentTab = -1;

        for (int i = 0; i < Tabs.Length; ++i)
        {
            int ii = i;
            Tabs[i].TabButton.onClick.AddListener(() =>
            {
                SwitchTab(ii);
            });
            Tabs[i].Tab.gameObject.SetActive(false);
        }
        
        SwitchTab(0);
    }

    void SwitchTab(int tab)
    {
        if (m_CurrentTab != -1)
        {
            Tabs[m_CurrentTab].TabButton.enabled = true;
            Tabs[m_CurrentTab].Tab.gameObject.SetActive(false);
        }

        m_CurrentTab = tab;
        Tabs[m_CurrentTab].TabButton.enabled = false;
        Tabs[m_CurrentTab].Tab.gameObject.SetActive(true);
    }
}
