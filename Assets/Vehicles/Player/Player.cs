using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("OBJECT SETUP")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private GameObject turret;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject smokeEmitter;
    [SerializeField] private float smokeInterval = 5f;
    private float smokeIntervalTimer = 0f;
    [SerializeField] private GameObject tankRound;
    [SerializeField] private GameObject crosshair;

    [SerializeField] private Animator turretAnim;

    [SerializeField] private TextMeshProUGUI HUD_HP;
    [SerializeField] private TextMeshProUGUI HUD_RELOAD;
    [SerializeField] private TextMeshProUGUI HUD_ENEMYCOUNT;
    [SerializeField] public int enemyCount = 0;

    [Header("TURRET")]
    [SerializeField] float turret_rotation_speed = 20f;
    [SerializeField] float reload_rate = 180f;
    [SerializeField] AudioSource audioPlayer;
    [SerializeField] AudioClip firingSound;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] GameObject muzzle;
    float reload_timer = 0;

    [Header("HULL")]
    [SerializeField] float hull_rotation_speed = 120f;
    [SerializeField] float move_speed = .75f;
    [SerializeField] float hull_acceleration = .75f;
    [SerializeField] float starting_hp = 3.0f;
    [SerializeField] AudioSource engineSound;
    [SerializeField] AudioSource treadsSound;
    float maxTreadsVolume = .75f;
    float maxTreadsPitch = 1.3f;
    float treadsChangeRate = 3f;
    float minTreadsPitch = .25f;
    float hp = 0f;
    [SerializeField] AudioClip impactSound;
    Vector2 hull_velocity = Vector2.zero;
    float hull_rotation_velocity = 0f;

    public enum STATUS {ALIVE, DEAD};
    public STATUS state = STATUS.ALIVE;

    //camera
    float cam_shake_magnitude = 0f;
    float shake_decrease_rate = 1f;
    Vector3 camera_actual_position = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = starting_hp;
        camera_actual_position = PlayerCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (state == STATUS.ALIVE)
        {
            emit_smoke();
            process_mouseaim();
            fire_check();

            if (reload_timer == 0)
            {
                HUD_RELOAD.text = "GUN READY";
            }
            else
            {
                HUD_RELOAD.text = "RELOADING";
            }


            HUD_HP.text = "ARMOR STATUS: " + MathF.Round(hp/starting_hp*100).ToString() + "%";
        }
        else
        {
            HUD_RELOAD.text = "";
            HUD_HP.text = "MISSION KILL - PRESS R";
        }

        PlayerCamera.transform.rotation = Quaternion.Euler(0, 0, 0);
        // Restart test for debug
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void FixedUpdate()
    {
        if (state == STATUS.ALIVE)
        {
            movement();
            camera_actual_position = new Vector3(rigidBody.transform.position.x,rigidBody.transform.position.y,PlayerCamera.transform.position.z);
        }
        process_camera_shake();
    }

    void movement()
    {

        bool anyMovement = false;

        //prevent sliding
        rigidBody.linearVelocity = Vector2.zero;

        float desired_rotation = 0f;

        //rotate hull
        if (Keyboard.current.dKey.IsPressed())
        {
            anyMovement = true;
            desired_rotation = - hull_rotation_speed * Time.fixedDeltaTime;
        }

        if (Keyboard.current.aKey.IsPressed())
        {
            anyMovement = true;
            desired_rotation = +hull_rotation_speed * Time.fixedDeltaTime;

        }
        hull_rotation_velocity = Mathf.Lerp(hull_rotation_velocity, desired_rotation, hull_acceleration * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation + hull_rotation_velocity);

        Vector2 desired_velocity = Vector2.zero;

        //move forward/backwards
        if (Keyboard.current.wKey.IsPressed())
        {
            anyMovement = true;
            desired_velocity = (transform.up.normalized * move_speed * Time.fixedDeltaTime);
        }

        if (Keyboard.current.sKey.IsPressed())
        {
            anyMovement = true;
            desired_velocity = (transform.up.normalized * -move_speed * Time.fixedDeltaTime);
        }

        hull_velocity = Vector2.Lerp(hull_velocity,desired_velocity,Time.fixedDeltaTime * hull_acceleration);
        rigidBody.MovePosition(new Vector2(transform.position.x, transform.position.y) + hull_velocity);

        treads_sound_processing(anyMovement);
    }

    void fire_check()
    {

        //Process reload
        reload_timer = Mathf.Max(0f, reload_timer - (60 * Time.deltaTime));

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (reload_timer == 0f)
            {
                audioPlayer.pitch = UnityEngine.Random.Range(.95f, 1.05f);
                audioPlayer.PlayOneShot(firingSound);
                turretAnim.SetBool("firing", true);


                GameObject flash = GameObject.Instantiate(muzzleFlash,muzzle.transform);
                flash.transform.parent = muzzle.transform;
                flash.transform.rotation = turret.transform.rotation;
                flash.transform.position = muzzle.transform.position;

                start_camera_shake(9.0f);
                GameObject clone = GameObject.Instantiate(tankRound);
                clone.transform.parent = null;
                clone.transform.rotation = turret.transform.rotation;
                clone.transform.position = turret.transform.position;
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), clone.GetComponent<Collider2D>());

                reload_timer = reload_rate;

                //Start reload animation on crosshair
                if (crosshair.TryGetComponent(out crosshair crosshairObj))
                {
                    crosshairObj.start_reload();
                }
            }
            
        }
    }
    void process_mouseaim()
    {

        Vector3 save_cam_pos = PlayerCamera.transform.position;

        PlayerCamera.transform.position = new Vector3(rigidBody.transform.position.x, rigidBody.transform.position.y, PlayerCamera.transform.position.z);

        //get mouse screen pos to rotate towards
        Vector3 aim_position = PlayerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = aim_position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //provide correction angle offset and convertion to quaternion
        angle -= 90;
        Quaternion angle_quaternion = Quaternion.Euler(new Vector3(0, 0, angle));

        //execute rotation
        turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation,angle_quaternion,turret_rotation_speed * Time.deltaTime);

        PlayerCamera.transform.position = save_cam_pos;
    }

    public void take_damage()
    {

        audioPlayer.pitch = UnityEngine.Random.Range(.75f, .77f);
        audioPlayer.PlayOneShot(impactSound);

        hp -= 1;
        Debug.Log("PLAYER HIT!");
        start_camera_shake(11.5f);
        if (hp == 0)
        {
            engineSound.Stop();
            treadsSound.Stop();
            state = STATUS.DEAD;
        }
    }

    public void start_camera_shake(float _magnitude)
    {
        if (_magnitude >= cam_shake_magnitude)
        {
            cam_shake_magnitude = _magnitude;
        }
    }

    void process_camera_shake()
    {
        PlayerCamera.transform.position = camera_actual_position;
        cam_shake_magnitude = MathF.Max(cam_shake_magnitude - (Time.fixedDeltaTime * 60 * shake_decrease_rate), 0f);
        float to_shake = cam_shake_magnitude / 50f;
        PlayerCamera.transform.position += new Vector3(UnityEngine.Random.Range(-to_shake, to_shake),UnityEngine.Random.Range(-to_shake, to_shake),0);
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
            smoke_part.transform.rotation = Quaternion.Euler(turret.transform.rotation.eulerAngles + new Vector3(0,0,UnityEngine.Random.Range(0,360)));
        }
    }

    void treads_sound_processing(bool Active)
    {
        if (Active)
        {
            Debug.Log("INCREASING PITCH");
            treadsSound.pitch = Mathf.Min(treadsSound.pitch + (treadsChangeRate * maxTreadsPitch * Time.fixedDeltaTime), maxTreadsPitch);
            treadsSound.volume = Mathf.Min(treadsSound.volume + (treadsChangeRate * maxTreadsVolume * Time.fixedDeltaTime), maxTreadsVolume);
        }
        else
        {
            Debug.Log("DECREASING PITCH");
            treadsSound.volume = Mathf.Max(treadsSound.volume - (treadsChangeRate * maxTreadsVolume * Time.fixedDeltaTime), 0f);
            treadsSound.pitch = Mathf.Max(treadsSound.pitch - (treadsChangeRate * maxTreadsPitch * Time.fixedDeltaTime), minTreadsPitch);
        }
    }

    public void updateEnemyCount(int _count)
    {
        enemyCount = _count;
        if (enemyCount > 0) {
            HUD_ENEMYCOUNT.text = "TARGETS: " + enemyCount.ToString();
        }
        else
        {
            HUD_ENEMYCOUNT.text = "ALL TARGETS DESTROYED";
        }
    }
}
