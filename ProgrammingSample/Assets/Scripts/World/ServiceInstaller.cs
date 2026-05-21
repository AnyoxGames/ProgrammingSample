using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceInstaller : MonoBehaviour
{
    protected void Awake()
    {
        SceneManager.LoadScene("Scenes/Services", LoadSceneMode.Additive);
        Destroy(gameObject);
    }
}