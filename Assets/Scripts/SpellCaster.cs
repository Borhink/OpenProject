using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour {

    private Player _player;
    public int spellIndex = 0;

    public Spell[] spells;
    public GameObject[] effectZonePrefab;
    private GameObject effectZone;
    public GameObject rangeZonePrefab;
    private GameObject _rangeZone;
    private Spell _selectedSpell;

    void Start()
    {
        _player = GetComponent<Player>();
        _rangeZone = Instantiate(rangeZonePrefab, _player.transform);
        if (spellIndex == 0)
            _rangeZone.SetActive(!_rangeZone.activeSelf);
    }

    void Update()
    {
        if (IsActiveSpell())
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = _player.transform.position.z;
            if (Vector2.SqrMagnitude(pos - _player.transform.position) > _selectedSpell.range * _selectedSpell.range)
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
        else if (index >= 0 && index <= spells.Length)
            spellIndex = index;
        if (effectZone)
            Destroy(effectZone);
        if (spellIndex > 0 && spellIndex <= spells.Length)
        {
            _selectedSpell = spells[spellIndex - 1];
            _rangeZone.SetActive(true);
            _rangeZone.transform.localScale = new Vector3(_selectedSpell.range * 2, _selectedSpell.range * 2);
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = _player.transform.position.z;
            effectZone = Instantiate(effectZonePrefab[(int)_selectedSpell.zoneType], pos, Quaternion.identity);
            Transform child = effectZone.transform.GetChild(0);
            child.localScale = _selectedSpell.scale;
            child.localPosition = _selectedSpell.offset;
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
