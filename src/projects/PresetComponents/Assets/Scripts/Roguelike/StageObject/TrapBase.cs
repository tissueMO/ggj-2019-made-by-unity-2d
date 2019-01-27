using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Roguelike;

public abstract class TrapBase : StageObjectBase,IActictible
{
    
    public  virtual void ActiveTrap(StageObjectBase character)
    {
        OnActivate(character);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected abstract void OnActivate(StageObjectBase character);   
}
