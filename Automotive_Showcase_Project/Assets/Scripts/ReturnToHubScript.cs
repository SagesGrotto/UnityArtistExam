using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHubScript : MonoBehaviour
{
    public void ReturnToHub()
    {
        SceneManager.LoadScene(0);
    }
}
