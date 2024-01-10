using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEntity : BaseEntity
{

    void Update()
    {
        if (!HasEnemy)
            FindTarget();

        if (IsInRange && !moving)
        {
            if (canAttack)
            {
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
