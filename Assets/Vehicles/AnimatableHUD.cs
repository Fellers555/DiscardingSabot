using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageCanvasAnimator : MonoBehaviour
{

    // set the controller you want to use in the inspector
    public RuntimeAnimatorController controller;

    // the UI/Image component
    [SerializeField] Image imageCanvas;
    // the fake SpriteRenderer
    [SerializeField] SpriteRenderer fakeRenderer;
    // the Animator
    [SerializeField] Animator animator;

    void Update()
    {
        imageCanvas.sprite = fakeRenderer.sprite;
    }

}