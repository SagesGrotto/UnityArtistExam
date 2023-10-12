using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public Transform CameraPosition;

    public string Title;
    [Multiline]
    public string Description;

    public int LevelToLoad = -1;
    
    private void OnEnable()
    {
        RecursiveLayerSet(transform);
    }

    void RecursiveLayerSet(Transform root)
    {
        root.gameObject.layer = Helpers.HubStationLayer;
        foreach (Transform t in root)
        {
            RecursiveLayerSet(t);
        }
    }
}
