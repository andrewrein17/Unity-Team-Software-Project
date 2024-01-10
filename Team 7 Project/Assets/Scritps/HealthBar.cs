using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform bar;
    public Vector3 offset;

    private float maxHealth;
    private Transform target;

    public void Setup(Transform target, float maxHealth){

        this.maxHealth = maxHealth;
        UpdateBar(maxHealth);
        this.target = target;

    }

    public void UpdateBar(float newHealthValue){

        float newScale = newHealthValue / maxHealth;
        Vector3 scale = bar.transform.localScale;
        scale.x = newScale * 3;
        bar.transform.localScale = scale;

    }

    private void Update(){

        if(target != null){
            this.transform.position = target.position + offset;
        }

    }

}
