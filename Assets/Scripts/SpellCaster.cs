using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour {

    private Player _player;
    public int spellIndex = 0;

    public Spell[] spells;
    public GameObject[] effectZonePrefab;
    private GameObject _effectZone;
    public GameObject rangeZonePrefab;
    private GameObject _rangeZone;
    private Spell _selectedSpell;

    void Start()
    {
        _player = GetComponent<Player>();
        _rangeZone = Instantiate(rangeZonePrefab, _player.transform);
        _effectZone = transform.Find("EffectZone").gameObject;
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
                _effectZone.SetActive(false);
            else
            {
                _effectZone.SetActive(true);
                _effectZone.transform.position = pos;
                _effectZone.transform.rotation = LookAtCursor(pos);
            }
        }
    }

    public void CreateEffectZone()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = _player.transform.position.z;

        foreach (EffectZone ez in _selectedSpell.zones)
        {
            GameObject newZone = Instantiate(effectZonePrefab[(int)ez.type], _effectZone.transform);
            newZone.transform.localScale = ez.scale;
            newZone.transform.localPosition = ez.offset;
            newZone.transform.localEulerAngles = ez.rotation;
        }
        _effectZone.transform.rotation = LookAtCursor(pos);
    }

    public void DeleteEffectZone()
    {
        foreach (Transform child in _effectZone.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetActiveSpell(int index)
    {
        if (index == spellIndex)
            spellIndex = 0;
        else if (index >= 0 && index <= spells.Length)
            spellIndex = index;
        if (_effectZone)
            DeleteEffectZone();
        if (spellIndex > 0 && spellIndex <= spells.Length)
        {
            _selectedSpell = spells[spellIndex - 1];
            _rangeZone.SetActive(true);
            _rangeZone.transform.localScale = new Vector3(_selectedSpell.range * 2, _selectedSpell.range * 2);
            CreateEffectZone();
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
