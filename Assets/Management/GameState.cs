using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{

    [SerializeField] Player playersTank;
    [SerializeField] private string nextScene = "";
    bool enemiesLeft = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                if (nextScene != "")
                {
                    SceneManager.LoadScene(nextScene);
                }
                else
                {
                    Debug.Log("INVALID SCENE TO LOAD ON GAMESTATE");
                }

            }
        }
    }
}
