﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public Stats charStats = null;
    public float attackMod = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Perform a simple attack
    public void Attack()
    {
        // Animation
        Managers.CombatManager.Instance.DealDamage(attackMod);
    }

    public void Defend()
    {
        //charStats.
    }
}