using UnityEngine;

public class TankRound : MonoBehaviour
{

    [Header("COMPONENTS")]
    [SerializeField] private Rigidbody2D rigidBody;

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
        //Timeout just incase
        lifetime = lifetime - Time.deltaTime * 60;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        //Move
        Vector2 targetNextPosition = transform.position + (transform.up.normalized * speed * Time.fixedDeltaTime);
        rigidBody.MovePosition(targetNextPosition);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy on wall
        int worldLayer = LayerMask.NameToLayer("WorldGeometry");
        if (collision.gameObject.layer == worldLayer)
        {
            Destroy(gameObject);
        }
    }
}
