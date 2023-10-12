using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif


public class ColorTabHandler : MonoBehaviour
{
    [System.Serializable]
    public class MaterialEntry
    {
        public Material Material;
        public Texture2D Preview;
    }
    
    public MaterialEntry[] AvailableColors;
    public Toggle TogglePrefab;

    public Action<Material> OnColorSwitched
    {
        get => m_OnColorSwitched;
        set => m_OnColorSwitched = value;
    }
    
    private Action<Material> m_OnColorSwitched;
    
    // Start is called before the first frame update
    void Start()
    {
        var toggleGroup = GetComponent<ToggleGroup>();
        
        foreach (var entry in AvailableColors)
        {
            var t = Instantiate(TogglePrefab, transform);
            t.group = toggleGroup;

            t.transform.GetChild(0).GetComponent<RawImage>().texture = entry.Preview;

            Material m = entry.Material;
            t.onValueChanged.AddListener(state =>
            {
                if (state)
                {
                    m_OnColorSwitched.Invoke(m);
                }
            });
        }
    }
    
    
    #if UNITY_EDITOR
    
    Queue<MaterialEntry> m_LeftEntry = new Queue<MaterialEntry>();
    private int m_EntryCount;
    private Texture2D previewLoading;

    [ContextMenu("Generate Preview texture")]
    void GeneratePreview()
    {
        //one is already running
        if(m_LeftEntry.Count > 0)
            return;
        
        foreach (var availableColor in AvailableColors)
        {
            m_LeftEntry.Enqueue(availableColor);
        }

        m_EntryCount = m_LeftEntry.Count;
        EditorUtility.DisplayProgressBar("Creating Preview", "Creating material preview", 0.0f);
        EditorApplication.update += EditorUpdate;
    }

    void EditorUpdate()
    {
        if (m_LeftEntry.Count == 0)
        {
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= EditorUpdate;
            AssetDatabase.SaveAssets();
            return;
        }

        if (previewLoading != null)
        {
            //asset loaded
            var entry = m_LeftEntry.Dequeue();
            
            var assetPath = "";
            Texture2D target;
            if (entry.Preview == null)
            {
                target = new Texture2D(previewLoading.width, previewLoading.height, TextureFormat.ARGB32, false);
                AssetDatabase.CreateAsset(target, "Assets/CarMaterialPreview/"+entry.Material.name+"_Preview.asset");
                entry.Preview = target;
            }
            else
            {
                target = entry.Preview;
            }
            
            target.SetPixels32(previewLoading.GetPixels32());
            EditorUtility.SetDirty(gameObject);
            EditorUtility.SetDirty(target);

            previewLoading = null;
            
            EditorUtility.DisplayProgressBar("Creating Preview", "Creating material preview", 1.0f - (float)m_LeftEntry.Count/m_EntryCount);
        }
        else
        {
            var entry = m_LeftEntry.Peek();
            previewLoading = AssetPreview.GetAssetPreview(entry.Material);
        }
    }
    
    #endif
}
