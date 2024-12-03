using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/*
 * Contains the UI for the Player's toolbelt.
 */
public class DialogBoxBehavior : MonoBehaviour
{
    private UIDocument dialogBoxUI;
    private VisualElement menuPanel;

    private VisualElement banner;
    private Label bannerText;

    // If true, the UI is being shown. This allows us to stop other events when the
    // tool selection UI is up
    public static bool showing;

    private static DialogBoxBehavior main;

    /// <summary>
    /// Grabs definitions from the UI document
    /// </summary>
    private void Awake()
    {
        dialogBoxUI = GetComponent<UIDocument>();
        menuPanel = dialogBoxUI.rootVisualElement.Q<VisualElement>("panel");

        banner = dialogBoxUI.rootVisualElement.Q<VisualElement>("Banner");
        bannerText = dialogBoxUI.rootVisualElement.Q<Label>("BannerText");

        if (main) {
            DestroyImmediate(gameObject);
            return;
        } else {
            DontDestroyOnLoad(gameObject);
            main = this;
        }

        // menuPanel.Add(radialButton);
        SceneManager.sceneLoaded += SetMenuActiveByScene;
    }
    void SetMenuActiveByScene(Scene scene, LoadSceneMode mode)
    {
        // Player should be inactive on the planet map scene and start scene
        if (scene.name == "PlanetMapScene" || scene.name == "MainMenu")
        {
            banner.style.display = DisplayStyle.None;
            main.gameObject.SetActive(false);
        }
        // Player should be active on all other scenes
        else
        {
            main.gameObject.SetActive(true);
            banner.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Ensures that the UI is hidden on start
    /// </summary>
    private void Start()
    {
        menuPanel.style.display = DisplayStyle.Flex;
        banner.style.display = DisplayStyle.None;
    }

    private void Update()
    {

    }

    /// <summary>
    /// On click event for the rifle button, will eventually be a call on all buttons differing by class name once more tools are introduced
    /// </summary>
    private void OnButtonClicked(ClickEvent evt)
    {
        
    }

    private void _ShowBanner(String text, float secs) {
        bannerText.text = text;
        StartCoroutine(FlashBanner(secs));
    }

    IEnumerator FlashBanner(float secs)
    {
        yield return new WaitForFixedUpdate();
        banner.style.display = DisplayStyle.Flex;
        yield return new WaitForSeconds(secs);
        banner.style.display = DisplayStyle.None;
    }

    public static void ShowBanner(String text, float secs) {
        main._ShowBanner(text, secs);
    }
}
