using TMPro;
using UnityEngine;

//Dervies from basic enemy class; BMP will as well
public class T90 : EnemyTank
{
    [SerializeField] private TextMeshPro ai_state_debug_text;

    [Header("TURRET")]
    [SerializeField] private GameObject turret;
    [SerializeField] float turret_rotation_speed = 20f;

    [SerializeField] float reload_rate = 180f;
    Quaternion turret_desired_rotation;
    private bool lineOfSight = false;

    void Start()
    {
        
    }

    public override void Update()
    {
        //GATHER PLAYER INFO
        GameObject player = GameObject.FindWithTag("Player");
        //if player dead ignore
        if (player.TryGetComponent(out Player playerTank))
        {
            if (playerTank.state == Player.STATUS.DEAD)
            {
                player = null;
            }
        }

        //GATHER LINE OF SIGHT ON PLAYEr
        lineOfSight = get_line_of_sight(player);

        //EXECUTE STATE MACHINE
        switch (ai_state) {
            case AI_STATE.IDLE:

                if (lineOfSight) {
                    ai_state = AI_STATE.ATTACK;
                }
                break;

            case AI_STATE.ATTACK:
                if (lineOfSight)
                {
                    set_turret_rotation(player.transform.position);
                }
                break;
        }

        process_turret_rotation();

        //Debug;
        ai_state_debug_text.text = ai_state.ToString();
    }
        

    public override void FixedUpdate()
    {

    }


    private void set_turret_rotation(Vector3 _position)
    {
        Vector3 aim_position = _position;
        Vector2 direction = aim_position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //provide correction angle offset and convertion to quaternion
        angle -= 90;
        Quaternion angle_quaternion = Quaternion.Euler(new Vector3(0, 0, angle));
        turret_desired_rotation = angle_quaternion;
    }

    private void process_turret_rotation()
    {
        turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation, turret_desired_rotation, turret_rotation_speed * Time.deltaTime);
    }

    //Override test: works
    public override void take_damage()
    {
        base.take_damage();
        Debug.Log("T90 DESTROYED!");
    }
}
