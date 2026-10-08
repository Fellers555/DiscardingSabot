using UnityEngine;

//Dervies from basic enemy class; BMP will as well
public class T90 : EnemyTank
{
    void Start()
    {
        
    }

    public override void Update()
    {
        
    }

    //Override test: works
    public override void take_damage()
    {
        base.take_damage();
        Debug.Log("T90 DESTROYED!");
    }
}
