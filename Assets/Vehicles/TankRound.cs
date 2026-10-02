using UnityEngine;

public class TankRound : MonoBehaviour
{

    [Header("ROUND STATS")]
    [SerializeField] private float speed = 120f;
    [SerializeField] private float lifetime = 120f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Move
        transform.Translate(Vector3.up * (speed * Time.deltaTime));

        //Timeout just incase
        lifetime = lifetime - Time.deltaTime * 60;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
