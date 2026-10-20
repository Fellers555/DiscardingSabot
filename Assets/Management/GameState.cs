using UnityEngine;

public class GameState : MonoBehaviour
{

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
        }
        
    }
}
