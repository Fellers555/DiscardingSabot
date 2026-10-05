using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyTank : MonoBehaviour
{

    [Header("COMPONENTS")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask lineOfSightLayer;

    [Header("HULL")]
    [SerializeField] float hull_rotation_speed = 120f;
    [SerializeField] float move_speed = .75f;


    //Line of Sight
    private bool lineOfSight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        //show player for later los check
        GameObject player = GameObject.FindWithTag("Player");

        get_line_of_sight(player);
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

    void get_line_of_sight(GameObject _target)
    {
        lineOfSight = !Physics2D.Linecast(_target.transform.position, transform.position, lineOfSightLayer);
    }
}
