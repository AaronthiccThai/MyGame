using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void LoadSpecificScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
