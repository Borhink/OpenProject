using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {

    private Player _player;
    public int spellIndex = 0;

    public GameObject[] effectZonePrefab;
    public GameObject effectZone;
    public GameObject rangeZonePrefab;
    public GameObject _rangeZone;

    //Spell Stats
    public float range = 5;
    public Vector3 offset = new Vector3(1.5f, 0);

    void Start () {
        _player = GetComponent<Player>();
        _rangeZone = Instantiate(rangeZonePrefab, _player.transform);
        if (spellIndex == 0)
            _rangeZone.SetActive(!_rangeZone.activeSelf);
    }
	
	void Update ()
    {
        if (IsActiveSpell())
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = _player.transform.position.z;
            if (Vector2.SqrMagnitude(pos - _player.transform.position) > range * range)
                effectZone.SetActive(false);
            else
            {
                effectZone.SetActive(true);
                effectZone.transform.position = pos;
                effectZone.transform.rotation = LookAtCursor(pos);
            }
        }
	}

    public void SetActiveSpell(int index)
    {
        if (index == spellIndex)
            spellIndex = 0;
        else
            spellIndex = index;
        if (effectZone)
            Destroy(effectZone);
        if (spellIndex > 0 && spellIndex <= effectZonePrefab.Length)
        {
            _rangeZone.SetActive(true);
            _rangeZone.transform.localScale = new Vector3(range * 2, range * 2);
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = _player.transform.position.z;
            effectZone = Instantiate(effectZonePrefab[spellIndex - 1], pos, Quaternion.identity);
            effectZone.transform.GetChild(0).localPosition = offset;
            effectZone.transform.rotation = LookAtCursor(pos);
        }
        else
        {
            _rangeZone.SetActive(false);
        }
    }

    public bool IsActiveSpell()
    {
        return (spellIndex != 0);
    }

    Quaternion LookAtCursor(Vector3 pos)
    {
        var dir = pos - _player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return (Quaternion.AngleAxis(angle, Vector3.forward));
    }
}
