using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class CrushableObject : MonoBehaviour
{

    [SerializeField] private Sprite crushedSprite;
    [SerializeField] private BoxCollider2D collBox;
    [SerializeField] private SpriteRenderer render;

    float force = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (transform.right.normalized * force * Time.deltaTime);
        force = Mathf.Lerp(force, 0f, Time.deltaTime * .5f * 30);
    }

    public void crush(GameObject cause, float crushForce = 0f, bool useObjectRotation = false)
    {
        if (crushedSprite != null)
        {
            if (cause != null)
            {
                if (useObjectRotation)
                {
                    Debug.Log("ROTTING");
                    transform.rotation = cause.transform.rotation;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, 90));
                }
                else
                {
                    Vector2 direction = new Vector2(cause.transform.position.x, cause.transform.position.y) - new Vector2(transform.position.x, transform.position.y);
                    float angleRad = Mathf.Atan2(direction.y, direction.x);
                    float angleDeg = angleRad * Mathf.Rad2Deg;
                    angleDeg -= 180f;
                    transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
                }

                //slight random offset
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, UnityEngine.Random.Range(-5f,5f)));
            }

            force = crushForce;

            render.sprite = crushedSprite;
        }
        collBox.enabled = false;
    }
}
