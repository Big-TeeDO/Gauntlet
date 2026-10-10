using UnityEngine;
using UnityEngine.SceneManagement; // Essential for scene switching

public class MenuController : MonoBehaviour
{
    // You can call this function for your "Play" button
    public void LoadGame()
    {
        // Replace "YourSceneName" with the actual name of your next scene
        SceneManager.LoadScene("GameScene");
    }

    // Optional: A function to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }
}