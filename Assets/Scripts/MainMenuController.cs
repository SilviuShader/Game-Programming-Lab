using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayButtonPressed() =>
        SceneManager.LoadScene("GameScene");

    public void OnQuitButtonPressed() =>
        Application.Quit();
}
