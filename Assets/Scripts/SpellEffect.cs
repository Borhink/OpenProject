using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffect : MonoBehaviour {

	public enum Type { Damage = 0, Heal };

    public Type type = Type.Damage; //Effect type
    public int min = 1; //Minimum effect's value
    public int max = 1; //Maximum effect's value
}
