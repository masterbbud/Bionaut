using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/*
 * Contains the UI for the Player's toolbelt.
 */
public class CatchCritterDialogBehavior : MonoBehaviour
{
    private UIDocument critterDialogUI;
    private VisualElement menuPanel;
    
    private VisualElement critterAnimation;
    private TextField nameField;
    private Button homeButton;
    private Button alongButton;

    // If true, the UI is being shown. This allows us to stop other events when the
    // tool selection UI is up
    public static bool showing;
    private CritterData selectedCritter;
    private int spriteIdx;

    [SerializeField] private int animationSpeedMs;

    private static CatchCritterDialogBehavior main;

    /// <summary>
    /// Grabs definitions from the UI document
    /// </summary>
    private void Awake()
    {
        critterDialogUI = GetComponent<UIDocument>();
        menuPanel = critterDialogUI.rootVisualElement.Q<VisualElement>("panel");

        critterAnimation = critterDialogUI.rootVisualElement.Q<VisualElement>("CritterAnim");
        nameField = critterDialogUI.rootVisualElement.Q<TextField>("NameField");
        homeButton = critterDialogUI.rootVisualElement.Q<Button>("SendHome");
        alongButton = critterDialogUI.rootVisualElement.Q<Button>("TakeAlong");

        homeButton.RegisterCallback<ClickEvent>(SendHome);
        alongButton.RegisterCallback<ClickEvent>(TakeAlong);

        spriteIdx = 0;

        if (main) {
            DestroyImmediate(gameObject);
            return;
        } else {
            DontDestroyOnLoad(gameObject);
            main = this;
        }

        // menuPanel.Add(radialButton);
    }

    /// <summary>
    /// Ensures that the UI is hidden on start
    /// </summary>
    private void Start()
    {
        critterDialogUI.rootVisualElement.style.display= DisplayStyle.None;
        critterAnimation.schedule.Execute(SwapAnimationTexture).Every(animationSpeedMs);
    }

    private void Update()
    {

    }

    private void OpenUI()
    {
        critterDialogUI.rootVisualElement.style.display= DisplayStyle.Flex;
        showing = true;
        Time.timeScale = 0;
    }

    private void CloseUI()
    {
        critterDialogUI.rootVisualElement.style.display= DisplayStyle.None;
        showing = false;
        Time.timeScale = 1;
    }

    /// <summary>
    /// On click event for the rifle button, will eventually be a call on all buttons differing by class name once more tools are introduced
    /// </summary>
    private void OnButtonClicked(ClickEvent evt)
    {
        
    }

    private void _Show(CritterData newCatch) {
        OpenUI();
        selectedCritter = newCatch;
        critterAnimation.style.backgroundImage = new StyleBackground(selectedCritter.sprites[0].texture);
        critterAnimation.style.backgroundColor = selectedCritter.planetColor;
    }

    public static void Show(CritterData newCatch) {
        main._Show(newCatch);
    }

    private void SwapAnimationTexture() {
        if (!selectedCritter) { return; }
        spriteIdx = (spriteIdx + 1) % selectedCritter.sprites.Count;
        critterAnimation.style.backgroundImage = new StyleBackground(selectedCritter.sprites[spriteIdx].texture);
    }

    private void SendHome(ClickEvent clickEvent) {
        Player.inventory.NameCritter(selectedCritter, nameField.value);
        CloseUI();
    }

    private void TakeAlong(ClickEvent clickEvent) {
        Player.inventory.NameCritter(selectedCritter, nameField.value);
        if (Companion.main) {
            Companion.main.GetComponent<Companion>().OnDeselect();
            DestroyImmediate(Companion.main);
        }
        GameObject companion = Instantiate(selectedCritter.prefab);
        companion.transform.position = Player.main.transform.position;
        CloseUI();
    }
}
