using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyTank : MonoBehaviour
{

    //Line of Sight
    public bool lineOfSight = false;
    public enum AI_STATE { IDLE, ATTACK };
    public AI_STATE ai_state = AI_STATE.IDLE;

    //movement
    public Vector2 hull_velocity = Vector2.zero;
    public float hull_rotation_velocity = 0f;

    [Header("COMPONENTS")]
    [SerializeField] public Rigidbody2D rigidBody;
    [SerializeField] private LayerMask lineOfSightLayer;

    [Header("REACTION TIME")]
    [SerializeField] float reaction_time = 35f;
    float reaction_timer;

    [Header("HULL")]
    [SerializeField] public float hull_rotation_speed = 120f;
    [SerializeField] public float move_speed = .75f;
    [SerializeField] public float hull_acceleration = .75f;

    void Start()
    {
        reaction_timer = reaction_time;
    }

    public virtual void Update()
    {

    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        //prevent sliding
        rigidBody.linearVelocity = Vector2.zero;

        Vector2 targetNextPosition = transform.position + (transform.up.normalized * move_speed * Time.fixedDeltaTime);
        rigidBody.MovePosition(targetNextPosition);
        
    }

    //damage function; add logic for ammo types here
    public virtual void take_damage()
    {
        Destroy(gameObject);
        Debug.Log("TARGET DESTROYED!");
    }

    public bool get_line_of_sight(GameObject _target)
    {
        if (_target == null) {
            return false;
        }
        bool actual_los = !Physics2D.Linecast(_target.transform.position, transform.position, lineOfSightLayer);
        if (actual_los)
        {
            reaction_timer = Mathf.Max(0f, reaction_timer - (60 * Time.deltaTime));
            if (reaction_timer == 0)
            {
                return true;
            }
        }
        else
        {
            reaction_timer = reaction_time;
        }
        return false;
    }
}
