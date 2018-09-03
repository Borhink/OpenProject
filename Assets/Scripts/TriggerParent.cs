﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParent : MonoBehaviour {

    SpellCaster spellCaster = null;

	void Start () {
        spellCaster = GetComponentInParent<SpellCaster>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spellCaster)
            spellCaster.OnEffectZoneEnter(collision, GetComponent<EffectZone>());
    }
}