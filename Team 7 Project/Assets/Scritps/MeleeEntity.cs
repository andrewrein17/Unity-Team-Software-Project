using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEntity : BaseEntity{
    // Update is called once per frame
    
    void Update(){
        if(!HasEnemy)
            FindTarget();

        if(IsInRange && !moving) {
            if(canAttack) {
                Attack();
            }
        }
        else
            GetInRange();
    }

    protected override void Attack()
    {
        base.Attack();
        currentTarget.TakeDamage(baseDamage);
    }
    
}
