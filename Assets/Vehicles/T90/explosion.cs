using UnityEngine;

public class explosion : MonoBehaviour
{

    float lifetime = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifetime = Mathf.Max(lifetime - Time.deltaTime * 60, 0f);
        if (lifetime == 0.0)
        {
            Destroy(gameObject);
        }
    }
}
