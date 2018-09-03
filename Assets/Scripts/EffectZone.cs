using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour {

    public enum Type { Circle = 0, Cone, Rectangle };
    public enum Target { Self = 1, Ally = 2, Self_Ally = 3, Enemy = 4, Self_Enemy = 5, Ally_Enemy = 6, All = 7 };

    public Vector3 offset = new Vector3(1.5f, 0); //Zone offset
    public Vector3 rotation = new Vector3(0, 0); //Zone rotation
    public Vector3 scale = new Vector3(3f, 3f); //Zone scale
    public Type type = Type.Circle; //Zone type
    public Target target = Target.All; //Effective target of the zone
}
