using UnityEngine;
using UnityEngine.InputSystem;

public class crosshair : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Animator CrosshairAnimator;
    [SerializeField] private Animator HudAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = new Vector3(0, 0, 20);
        transform.parent = null;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = new Quaternion(0, 0, 0, 1);
        transform.position = PlayerCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        transform.position = new Vector3(transform.position.x, transform.position.y, 20);
    }

    public void start_reload()
    {
        CrosshairAnimator.SetBool("reloading", true);
        HudAnimator.SetBool("reloading", true);
    }
}
