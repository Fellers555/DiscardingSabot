using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("OBJECT SETUP")]
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private GameObject turret;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tankRound;

    [Header("TURRET")]
    [SerializeField] float turret_rotation_speed = 20f;
    [SerializeField] float reload_rate = 180f;
    float reload_timer = 0;

    [Header("HULL")]
    [SerializeField] float hull_rotation_speed = 120f;
    [SerializeField] float move_speed = .75f;
    [SerializeField] float hull_acceleration = .75f;
    [SerializeField] float hp = 3.0f;
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
        camera_actual_position = PlayerCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (state == STATUS.ALIVE)
        {
            process_mouseaim();
            fire_check();
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
            process_camera_shake();
        }
    }

    void movement()
    {

        //prevent sliding
        rigidBody.linearVelocity = Vector2.zero;


        float desired_rotation = 0f;

        //rotate hull
        if (Keyboard.current.dKey.IsPressed())
        {
            desired_rotation = - hull_rotation_speed * Time.fixedDeltaTime;
        }

        if (Keyboard.current.aKey.IsPressed())
        {
            desired_rotation = +hull_rotation_speed * Time.fixedDeltaTime;

        }
        hull_rotation_velocity = Mathf.Lerp(hull_rotation_velocity, desired_rotation, hull_acceleration * Time.fixedDeltaTime);
        rigidBody.MoveRotation(rigidBody.rotation + hull_rotation_velocity);

        Vector2 desired_velocity = Vector2.zero;

        //move forward/backwards
        if (Keyboard.current.wKey.IsPressed())
        {
            desired_velocity = (transform.up.normalized * move_speed * Time.fixedDeltaTime);
        }

        if (Keyboard.current.sKey.IsPressed())
        {
            desired_velocity = (transform.up.normalized * -move_speed * Time.fixedDeltaTime);
        }

        hull_velocity = Vector2.Lerp(hull_velocity,desired_velocity,Time.fixedDeltaTime * hull_acceleration);
        rigidBody.MovePosition(new Vector2(transform.position.x, transform.position.y) + hull_velocity);
    }

    void fire_check()
    {

        //Process reload
        reload_timer = Mathf.Max(0f, reload_timer - (60 * Time.deltaTime));

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (reload_timer == 0f)
            {
                start_camera_shake(6.5f);
                GameObject clone = GameObject.Instantiate(tankRound);
                clone.transform.parent = null;
                clone.transform.rotation = turret.transform.rotation;
                clone.transform.position = turret.transform.position;
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), clone.GetComponent<Collider2D>());

                reload_timer = reload_rate;
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
        hp -= 1;
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
}
