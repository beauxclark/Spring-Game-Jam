using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public KeyCode pauseKey;
    public static bool isPaused;
    public PlayerCam cam;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // pauses or resumes game when escape is pressed.
        if(Input.GetKeyDown(pauseKey))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        cam.UnlockCursor();
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        cam.LockCursor(); 
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void GoToMainMenu()
    {
        //MESSED WITH THIS. CAN REMOVE BOOL IF DOESN'T WORK.
        Time.timeScale = 1f;
        cam.UnlockCursor();
        SceneManager.LoadScene("Main Menu");
        isPaused = true;
    }
    public void OuitGame()
    {
        Application.Quit();
    }
   
}
