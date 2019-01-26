using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadGAme : MonoBehaviour
{
    public string gameScene;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(gameScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
