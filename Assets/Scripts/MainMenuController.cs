using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private Scene scene;
    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)  && scene.name == "FPSGameScene")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("Main Scene");
        }
    }

    public void ChangeScene(string _newScene) {

        //If scene doesn't exist, it isn't loaded
        SceneManager.LoadScene(_newScene);

    }

    public void Exit() {
        Application.Quit();
    }

}
