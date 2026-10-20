using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MuzzleFlash : MonoBehaviour
{

    float lifetime = 9f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime = Mathf.Max(lifetime - Time.deltaTime * 60, 0f);
        if (lifetime == 0)
        {
            Destroy(gameObject);
        }
    }

    void OnAnimationEnd()
    {

    }
}
