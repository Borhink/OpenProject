using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    public enum ZoneType { Circle = 0, Cone, Rectangle };

    public float range = 5;
    public Vector3 offset = new Vector3(1.5f, 0);
    public Vector3 scale = new Vector3(3f, 3f);
    public ZoneType zoneType = ZoneType.Circle;
}
