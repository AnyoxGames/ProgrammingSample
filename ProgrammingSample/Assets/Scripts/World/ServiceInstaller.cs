using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldCore : MonoBehaviour
{
    protected void Awake()
    {
        SceneManager.LoadScene("Scenes/Services", LoadSceneMode.Additive);
        Destroy(gameObject);
    }
}