using UnityEngine;

public class smoke : MonoBehaviour
{
    [SerializeField] float max_scale = 1.2f;
    [SerializeField] float scale_rate = .1f;
    [SerializeField] float rotation_rate = 2f;

    [SerializeField] float starting_opacity = .75f;

    [SerializeField] private SpriteRenderer rendering;

    float scale_track = 0f;

    void Start()
    {
        rendering.color = new Color(1, 1, 1, starting_opacity);
    }

    // Update is called once per frame
    void Update()
    {
        scale_track = Mathf.Min(scale_track + scale_rate * Time.deltaTime, max_scale);
        transform.localScale = new Vector3(scale_track,scale_track,scale_track);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 0, rotation_rate * Time.deltaTime));
        rendering.color = new Color(1, 1, 1, rendering.color.a - (1-starting_opacity/(max_scale / scale_rate)) * Time.deltaTime);
        if (scale_track == max_scale)
        {
            Destroy(gameObject);
        }
    }
}
