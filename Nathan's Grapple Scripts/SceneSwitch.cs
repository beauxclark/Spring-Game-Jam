using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitch : MonoBehaviour
{
    public PlayerCam cam;

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Obstacle"))
    {
    SceneManager.LoadScene("Game Over");
    cam.UnlockCursor(); 
    } 

    if (collision.gameObject.CompareTag("Ground"))
    {
    SceneManager.LoadScene("Game Over");
    cam.UnlockCursor();
    }

    if (collision.gameObject.CompareTag("Winner"))
    {
    SceneManager.LoadScene("Win Scene");
    cam.UnlockCursor(); 
    }
}

}
