using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableInfoPart : MonoBehaviour
{
    public string Title;
    [Multiline]
    public string Description;
    
    // Start is called before the first frame update
    void Start()
    {
        Helpers.RecursiveLayerSet(transform, Helpers.SelectableInfoLayer);
    }
}
