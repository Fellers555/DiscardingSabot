using UnityEngine;

public class EnemyTank : MonoBehaviour
{

    [Header("COMPONENTS")]
    [SerializeField] private Rigidbody2D rigidBody;

    [Header("HULL")]
    [SerializeField] float hull_rotation_speed = 120f;
    [SerializeField] float move_speed = .75f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //prevent sliding
        rigidBody.linearVelocity = Vector2.zero;

        Vector2 targetNextPosition = transform.position + (transform.up.normalized * move_speed * Time.fixedDeltaTime);
        rigidBody.MovePosition(targetNextPosition);
    }

    //damage function; add logic for ammo types here
    public void take_damage()
    {
        Destroy(gameObject);
        Debug.Log("TARGET DESTROYED!");
    }
}
