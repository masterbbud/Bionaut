using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyManualInteraction : InteractibleObject
{
    [SerializeField]
    private Sprite openBook;
    [SerializeField]
    private GameObject flashingLight, mouseHelper;

    public static bool broken = false;
    public override void Start() {
        base.Start();
        if (broken) {
            GetComponent<SpriteRenderer>().sprite = openBook;
            Destroy(flashingLight);
        }
    }
    public override void Interact()
    {
        // Give the planet 1 ship part
        if (broken) {
            MainMenuBehavior.ShowMenu();
        }
        else {
            broken = true;
            GetComponent<SpriteRenderer>().sprite = openBook;
            Destroy(flashingLight);
        }
    }

    protected override void OnPlayerNear()
    {
        StartCoroutine(FadeInMouse());
    }
    protected override void OnPlayerLeave()
    {
        StartCoroutine(FadeOutMouse());
    }

    IEnumerator FadeInMouse()
    {
        mouseHelper.SetActive(true);
        Color color = mouseHelper.GetComponent<SpriteRenderer>().color;
        for (float i = 0; i < 1; i += 0.1f) {
            color.a = i;
            mouseHelper.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeOutMouse()
    {
        Color color = mouseHelper.GetComponent<SpriteRenderer>().color;
        for (float i = 0; i < 1; i += 0.1f) {
            color.a = 1-i;
            mouseHelper.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.05f);
        }
        mouseHelper.SetActive(false);
    }

}
