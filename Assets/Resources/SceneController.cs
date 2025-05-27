using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SceneChange(string sceneName) { 
        SceneManager.LoadScene(sceneName); 
        Time.timeScale = 1.0f;
    }

    public void SceneRestart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
