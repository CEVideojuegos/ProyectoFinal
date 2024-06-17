using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAttack : MonoBehaviour
{
    [SerializeField] private GameObject HitboxAtaque;
    private bool canDealDamage;

    void Start()
    {
        Destroy(this.gameObject, 1.6f);
    }

    public void CanDealDamage()
    {
        canDealDamage = true;
        HitboxAtaque.SetActive(true);
    }

    public void CantDealDamage()
    {
        canDealDamage = false;
        HitboxAtaque.SetActive(false);
    }
}
