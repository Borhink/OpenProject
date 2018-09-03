using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    public float range = 5; //Spell range (in unit)
    public float cooldown = 1f; //Spell cooldown (in second)
    public float mana = 5f; //Mana cost
    public SpellEffect[] effects; //Array of SpellEffect
    public EffectZone[] zones; //Array of EffectZone
}
