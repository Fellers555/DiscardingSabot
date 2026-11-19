using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnemyTank;

public class GameState : MonoBehaviour
{

    [SerializeField] Player playersTank;
    [SerializeField] private string nextScene = "";
    [SerializeField] private Image image;
    bool enemiesLeft = true;
    bool fadingIn = true;
    bool fadeFinished = false;
    [SerializeField] float fadeRate = 2.5f;
    float fadeDelay = 20f;

    private enum FADE_FINISH_ACTION { NONE, RESTART, ADVANCE };
    private FADE_FINISH_ACTION on_fade_finish = FADE_FINISH_ACTION.NONE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //PROCESS FADING
        fadeDelay = Mathf.Max(fadeDelay - Time.deltaTime * 60, 0f);

        if (!fadeFinished && fadeDelay == 0f) {
            if (fadingIn)
            {
                image.color = new Color(0, 0, 0, Mathf.Max(0f, image.color.a - fadeRate * Time.deltaTime));
                if (image.color.a == 0.0f)
                {
                    fadeFinished = true;
                    finished_fade();
                }
            }
            else
            {
                image.color = new Color(0, 0, 0, Mathf.Min(1.0f, image.color.a + fadeRate * Time.deltaTime));
                if (image.color.a == 1.0f)
                {
                    fadeFinished = true;
                    finished_fade();
                }
            }
        }


        //HANDLE KILLING ENEMIES AND ADVANCING
        if (enemiesLeft) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
            if (enemies.Length == 0)
            {
                enemiesLeft = false;
                Debug.Log("ALL ENEMIES DESTROYED!");
            }
            if (playersTank.gameObject.TryGetComponent(out Player playerTank))
            {
                playerTank.updateEnemyCount(enemies.Length);
            }
        }

        //Process logic to switch scene
        else
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (fadeFinished)
                {
                    startFadeOut(FADE_FINISH_ACTION.ADVANCE);
                }
            }
        }

        //GLOBAL CHECK FOR LEVEL RESTART
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (fadeFinished)
            {
                startFadeOut(FADE_FINISH_ACTION.RESTART);
            }
        }
    }

    void finished_fade()
    {
        switch (on_fade_finish)
        {
            case FADE_FINISH_ACTION.ADVANCE:
                if (nextScene != "")
                {
                    SceneManager.LoadScene(nextScene);
                }
                else
                {
                    Debug.Log("INVALID SCENE TO LOAD ON GAMESTATE");
                }
                break;
            case FADE_FINISH_ACTION.RESTART:

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                break;
        }
    }

    void startFadeIn(FADE_FINISH_ACTION fadeAction)
    {
        fadingIn = true;

        fadeFinished = false;
        on_fade_finish = fadeAction;
    }

    void startFadeOut(FADE_FINISH_ACTION fadeAction)
    {
        fadingIn = false;
        fadeFinished = false;
        on_fade_finish = fadeAction;
    }
}
