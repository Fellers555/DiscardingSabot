using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Typewriter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt;
    private string text;
    private float max_delay = 2f;
    private float delay = 0f;
    private float inital_delay = 15f;
    private int position = 0;

    private bool complete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = txt.text;
        txt.text = "";
        delay = inital_delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!complete)
        {
            delay = Mathf.Max(delay - Time.deltaTime * 60, 0f);
            if (delay == 0f)
            {
                if (txt.text == text)
                {
                    complete = true;
                }
                else
                {
                    txt.text = txt.text + text[position];
                    position++;
                    delay = max_delay;
                }
            }
        }
    }
}
