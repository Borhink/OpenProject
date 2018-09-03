using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectZone : MonoBehaviour {

    public enum Type { Circle = 0, Cone, Rectangle };

    public Vector3 offset = new Vector3(1.5f, 0);
    public Vector3 rotation = new Vector3(0, 0);
    public Vector3 scale = new Vector3(3f, 3f);
    public Type type = Type.Circle;
}
