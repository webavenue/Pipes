using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScenes : MonoBehaviour
{

    public static GameScenes instance;

    [HideInInspector]
    public short levelIndex { get; private set; } = 1;

    public int splashSceneIndex;
    public int menuSceneIndex;
    public int gameSceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

    }

    public void LoadSpecificLevel(short levelIndex)
    {
        Currencies.instance.StopHeartTimer();
        this.levelIndex = levelIndex;
        Music.instance.StartMusic(Music.PipesMusics.Gameplay, true);
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void LoadMenu(int manuallyShowChapterX = -1, int manuallyShowLevelY = -1)
    {
        Music.instance.StartMusic(Music.PipesMusics.Menu, true);
        Menu.manuallyShowChapterX = manuallyShowChapterX;
        Menu.manuallyShowLevelY = manuallyShowLevelY;
        SceneManager.LoadScene(menuSceneIndex);
    }

    public bool AreWeInSplash()
    {
        return (SceneManager.GetActiveScene().buildIndex == splashSceneIndex);
    }

    public bool AreWeInMenu()
    {
        return (SceneManager.GetActiveScene().buildIndex == menuSceneIndex);
    }

    public bool AreWeInGame()
    {
        return (SceneManager.GetActiveScene().buildIndex == gameSceneIndex);
    }
}
