using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

//Dervies from basic enemy class; BMP will as well
public class T90 : EnemyTank
{
    public enum ATTACK_MOVEMENT_STATE { STILL, TURN, DRIVE };
    public ATTACK_MOVEMENT_STATE attack_movement_state = ATTACK_MOVEMENT_STATE.STILL;
    bool attack_state_rand_bool = false;
    float state_timer = 0f;
    float state_timer_min = 24f;
    float state_timer_max = 64f;

    [Header("ENGINE SMOKE")]
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject smokeEmitter;
    [SerializeField] private float smokeInterval = 5f;
    private float smokeIntervalTimer = 0f;

    [Header("TURRET")]
    [SerializeField] private GameObject tankRound;
    [SerializeField] private GameObject turret;
    [SerializeField] float turret_rotation_speed = 20f;
    [SerializeField] float firing_delay = 35f;

    [SerializeField] AudioSource audioPlayer;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject muzzle;

    float firing_delay_timer = 35f;

    [SerializeField] float reload_rate = 180f;
    float reload_timer = 0f;
    Quaternion turret_desired_rotation;

    void Start()
    {
        firing_delay_timer = firing_delay;
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
                ai_state = AI_STATE.IDLE;
            }
        }

        //GATHER LINE OF SIGHT ON PLAYEr
        lineOfSight = get_line_of_sight(player);

        //RELOAD WEAPON
        reload_timer = Mathf.Max(0f, reload_timer - (60 * Time.deltaTime));

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
                    firing_delay_timer = Mathf.Max(0f, firing_delay_timer - Time.deltaTime * 60);
                    if (firing_delay_timer == 0) {
                        attempt_fire();
                    }
                }
                else
                {
                    firing_delay_timer = firing_delay;
                }

                //Movement substate machine; actual movement code is loacted in fixed update
                state_timer = Mathf.Max(0f, state_timer - Time.deltaTime * 60);
                if (state_timer == 0) {
                    pick_random_attack_state();
                }
                break;
        }

        emit_smoke();

        process_turret_rotation();
    }
        

    public override void FixedUpdate()
    {

        //reset movement vars
        float desired_rotation = 0f;
        Vector2 desired_velocity = Vector2.zero;

        switch (ai_state)
        {
            case AI_STATE.IDLE:
                break;

            case AI_STATE.ATTACK:
                //execute movement based on state
                switch (attack_movement_state)
                {
                    case ATTACK_MOVEMENT_STATE.STILL:
                        break;

                    case ATTACK_MOVEMENT_STATE.TURN:
                        if (attack_state_rand_bool)
                        {
                            desired_rotation = hull_rotation_speed * Time.fixedDeltaTime;
                        }
                        else {
                            desired_rotation = -hull_rotation_speed * Time.fixedDeltaTime;
                        }
                        break;

                    case ATTACK_MOVEMENT_STATE.DRIVE:
                        if (attack_state_rand_bool) {
                            desired_velocity = (transform.up.normalized * move_speed * Time.fixedDeltaTime);
                        }
                        else
                        {
                            desired_velocity = (transform.up.normalized * -move_speed * Time.fixedDeltaTime);
                        }
                        break;
                }
                break;
        }

        //process turn and move
        hull_rotation_velocity = Mathf.Lerp(hull_rotation_velocity, desired_rotation, hull_acceleration * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation + hull_rotation_velocity);

        hull_velocity = Vector2.Lerp(hull_velocity, desired_velocity, Time.fixedDeltaTime * hull_acceleration);
        rigidBody.MovePosition(new Vector2(transform.position.x, transform.position.y) + hull_velocity);
    }

    private void attempt_fire()
    {

        float diff = Mathf.DeltaAngle(turret.transform.rotation.eulerAngles.z, turret_desired_rotation.eulerAngles.z);

        if ((reload_timer == 0f) && (Mathf.Abs(diff) < 10))
        {

            audioPlayer.Play();

            GameObject flash = GameObject.Instantiate(muzzleFlash, muzzle.transform);
            flash.transform.parent = muzzle.transform;
            flash.transform.rotation = turret.transform.rotation;
            flash.transform.position = muzzle.transform.position;

            GameObject clone = GameObject.Instantiate(tankRound);
            clone.transform.parent = null;
            clone.transform.rotation = turret.transform.rotation;
            clone.transform.position = turret.transform.position;
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), clone.GetComponent<Collider2D>());

            reload_timer = reload_rate;
        }
    }

    private void pick_random_attack_state()
    {
        state_timer = UnityEngine.Random.Range(state_timer_min, state_timer_max);
        attack_movement_state = (ATTACK_MOVEMENT_STATE)UnityEngine.Random.Range(0,3);
        attack_state_rand_bool = UnityEngine.Random.value > 0.5f;
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

    void emit_smoke()
    {
        smokeIntervalTimer = Mathf.Max(smokeIntervalTimer - 60 * Time.deltaTime, 0f);
        if (smokeIntervalTimer == 0)
        {
            smokeIntervalTimer = smokeInterval;
            GameObject smoke_part = GameObject.Instantiate(smoke, null);
            smoke_part.transform.parent = null;
            smoke_part.transform.position = smokeEmitter.transform.position;
            smoke_part.transform.rotation = Quaternion.Euler(turret.transform.rotation.eulerAngles + new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
        }
    }
}
