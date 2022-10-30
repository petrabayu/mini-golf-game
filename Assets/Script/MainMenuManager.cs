using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;

    private AudioSource bgm;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
    public void OnClickExit()
    {
        Debug.Log("Keluar Dari permainan");
        Application.Quit();
    }
}
