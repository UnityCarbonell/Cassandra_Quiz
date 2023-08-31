using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game");
        //AudioManager.Instance.SonidoPlay("UISFX");
    }
}
