using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    private float timing = 1f;
    private bool isPaused;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused && pauseMenu != null)
        {
            isPaused = true;    
            pauseMenu.SetActive(true);
            player1.SetActive(false);
            player2.SetActive(false);
            timing = 0f;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused && pauseMenu != null)
        {
            isPaused = false;
            timing = 1f;
            pauseMenu.SetActive(false);
            player1.SetActive(true);
            player2.SetActive(true);
        }
    }

    public void ResumePressed()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        player1.SetActive(true);
        player2.SetActive(true);
        timing = 1;
    }

    public void PlayPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitPressed()
    {
        Application.Quit();
    }

    
}
