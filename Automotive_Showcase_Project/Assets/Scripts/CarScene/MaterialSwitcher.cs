using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class RendererEntry
    {
        public Renderer TargetRenderer;
        public int MaterialSlot;
    }

    public RendererEntry[] Entries;
    public ColorTabHandler ColorHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        ColorHandler.OnColorSwitched += material =>
        {
            foreach (var entry in Entries)
            {
                var mats = entry.TargetRenderer.materials;
                mats[entry.MaterialSlot] = material;
                entry.TargetRenderer.materials = mats;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
