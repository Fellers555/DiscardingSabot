using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class tankCorpse : MonoBehaviour
{

    [Header("ENGINE SMOKE")]
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject smokeEmitter;
    [SerializeField] private float smokeInterval = 5f;
    private float smokeIntervalTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        emit_smoke();
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
            smoke_part.transform.rotation = Quaternion.Euler(smoke_part.transform.rotation.eulerAngles + new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
        }
    }
}
