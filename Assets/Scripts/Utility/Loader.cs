using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader{

    public enum Scene
    {
        TestScene,
        LoadingScene,
        MainMenu,
        LevelScene
    }

    private static Action onLoaderCallback;

    public static void Load(Scene scene)
    {

        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }


}
