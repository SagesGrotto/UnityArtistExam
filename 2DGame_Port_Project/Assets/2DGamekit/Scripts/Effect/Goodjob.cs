using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Goodjob : MonoBehaviour
{

    public TextMeshProUGUI Nice;
    public SpriteRenderer Goal;

    // Start is called before the first frame update
    void Start()
    {
        Nice.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            print("Yes");
            Nice.enabled = true;
            Goal.enabled = false;

            StartCoroutine(RestartLevel());
        }

    }
    IEnumerator RestartLevel()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);

        //After we have waited 5 seconds reload the scene.
        SceneManager.LoadScene("TileMap_Challenge_Scene");
    }


}
