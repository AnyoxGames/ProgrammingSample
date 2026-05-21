using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnyoxGames
{
    public class ServiceInstaller : MonoBehaviour
    {
        protected void Awake()
        {
            SceneManager.LoadScene("Scenes/Services", LoadSceneMode.Additive);
            Destroy(gameObject);
        }
    }
}