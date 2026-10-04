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

        //Destroy and do damage to enemy
        int vehicleLayer = LayerMask.NameToLayer("Vehicles");
        if (collision.gameObject.layer == vehicleLayer)
        {
            //if enemy run enemy damage code
            //we can expand this to classes; for later t90 and bmp will be different subclasses of this just inherit function
            if (collision.gameObject.TryGetComponent(out EnemyTank enemyTank))
            {
                enemyTank.take_damage();
            }
            Destroy(gameObject);
        }
    }
}
