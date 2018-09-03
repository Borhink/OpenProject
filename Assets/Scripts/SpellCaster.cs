using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCaster : MonoBehaviour {

    private Player _player;
    public int spellIndex = 0; //index of the selected spell

    public Spell[] spells; // Spells database
    public GameObject[] effectZonePrefab; // Zone's shape database
    private GameObject _effectZone; //Effect zone of the spell
    public GameObject rangeZonePrefab;
    private GameObject _rangeZone; 
    private Spell _selectedSpell; //Current selected spell
    private bool launchingSpell = false;
    private Vector3 lauchingPos;
    HashSet<Player> triggeredPlayers;

    void Start()
    {
        _player = GetComponentInParent<Player>();
        _rangeZone = Instantiate(rangeZonePrefab, _player.transform);
        _effectZone = transform.Find("EffectZone").gameObject;
        if (spellIndex == 0)
            _rangeZone.SetActive(!_rangeZone.activeSelf);
        triggeredPlayers = new HashSet<Player>();
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
            if (Input.GetMouseButtonDown(0))
            {
                launchingSpell = true;
                lauchingPos = pos;
            }
            if (Input.GetMouseButtonUp(0) && launchingSpell && Vector3.SqrMagnitude(lauchingPos - pos) < 0.02f)
                LaunchSpell();
        }
    }

    public void LaunchSpell()
    {
        launchingSpell = false;
        //foreach ()
        SetActiveSpell(0);
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
            EffectZone zone = newZone.GetComponent<EffectZone>();
            zone.target = ez.target;
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
        if (_effectZone.transform.childCount > 0)
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

    public void OnEffectZoneEnter(Collider2D collision, EffectZone zone)
    {
        Player other = collision.GetComponent<Player>();
        if (other == null)
            return;
        if ((zone.target == EffectZone.Target.All)//If zone trigger all
        || (((int)zone.target & (int)EffectZone.Target.Self) != 0 && _player.gameObject == collision.gameObject)//Or Self triggered
        || (((int)zone.target & (int)EffectZone.Target.Ally) != 0 && _player.gameObject != collision.gameObject && other.team == _player.team)//Or Ally triggered
        || (((int)zone.target & (int)EffectZone.Target.Enemy) != 0 && other.team != _player.team))//Or Enemy Triggered
        {
            triggeredPlayers.Add(other);
            other.gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.5f, 0);
        }
    }
}
