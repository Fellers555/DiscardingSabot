using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("OBJECT SETUP")]
    [SerializeField] private GameObject turret;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private GameObject tankRound;

    [Header("TURRET")]
    [SerializeField] float turret_rotation_speed = 20f;

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
        movement();
        fire_check();
        process_mouseaim();
    }

    void movement()
    {
        //rotate hull
        if (Keyboard.current.dKey.IsPressed())
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - (hull_rotation_speed * Time.deltaTime));
        }

        if (Keyboard.current.aKey.IsPressed())
        {
            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (hull_rotation_speed * Time.deltaTime));
        }

        //move forward/backwards
        if (Keyboard.current.wKey.IsPressed())
        {
            transform.Translate(Vector3.up * (move_speed * Time.deltaTime));
        }

        if (Keyboard.current.sKey.IsPressed())
        {
            transform.Translate(Vector3.up * (-move_speed * Time.deltaTime));
        }

        PlayerCamera.transform.rotation = new Quaternion(0, 0, 0, 1);
    }

    void fire_check()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject clone = GameObject.Instantiate(tankRound);
            clone.transform.parent = null;
            clone.transform.rotation = turret.transform.rotation;
            clone.transform.position = turret.transform.position;
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
        turret.transform.rotation = Quaternion.RotateTowards(turret.transform.rotation,angle_quaternion,turret_rotation_speed * Time.deltaTime);
    }
}
