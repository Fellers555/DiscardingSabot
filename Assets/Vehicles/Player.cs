using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fire_check();
        PlayerCamera.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    private void FixedUpdate()
    {
        process_mouseaim();
        movement();
    }

    void movement()
    {

        //prevent sliding
        rigidBody.linearVelocity = Vector2.zero;

        //rotate hull
        if (Keyboard.current.dKey.IsPressed())
        {
            float newAngle = rigidBody.rotation - hull_rotation_speed * Time.fixedDeltaTime;
            rigidBody.MoveRotation(newAngle);
        }

        if (Keyboard.current.aKey.IsPressed())
        {
            float newAngle = rigidBody.rotation + hull_rotation_speed * Time.fixedDeltaTime;
            rigidBody.MoveRotation(newAngle);
        }

        //move forward/backwards
        if (Keyboard.current.wKey.IsPressed())
        {
            Vector2 targetNextPosition = transform.position + (transform.up.normalized * move_speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(targetNextPosition);
        }

        if (Keyboard.current.sKey.IsPressed())
        {
            Vector2 targetNextPosition = transform.position + (transform.up.normalized * -move_speed * Time.fixedDeltaTime);
            rigidBody.MovePosition(targetNextPosition);
        }
    }

    void fire_check()
    {

        //Process reload
        reload_timer = Mathf.Max(0f, reload_timer - (60 * Time.deltaTime));

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (reload_timer == 0f)
            {
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
        //get mouse screen pos to rotate towards
        Vector3 aim_position = PlayerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = aim_position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //provide correction angle offset and convertion to quaternion
        angle -= 90;
        Quaternion angle_quaternion = Quaternion.Euler(new Vector3(0, 0, angle));

        //execute rotation
        turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation,angle_quaternion,turret_rotation_speed * Time.fixedDeltaTime);
    }
}
