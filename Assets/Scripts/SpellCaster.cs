using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    private Vector3 startPos;
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
        //DEBUG
        if (!_player.controller)
            return;

        if (IsActiveSpell())
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = _player.transform.position.z;

            if (InLineOfSight(pos) == false)
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
                startPos = pos;
            }
            if (Input.GetMouseButtonUp(0) && launchingSpell)
                LaunchSpell(pos);
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                StopSpellLaunching();
        }
    }

    bool InLineOfSight(Vector3 pos)
    {
        Vector2 dir = pos - _player.transform.position;
        float dist = Vector2.Distance(pos, _player.transform.position);
        float firstDist = 0;
        bool first = true;

        if (dist <= _selectedSpell.range)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(_player.transform.position, 0.03f, dir, dist, ~LayerMask.GetMask("EffectZone")).OrderBy(h => h.distance).ToArray();
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != _player.gameObject)
                {
                    if (first && hit.collider.gameObject.GetComponent<Player>() != null)
                    {
                        first = false;
                        firstDist = Vector2.Distance(hit.transform.position, _player.transform.position) + 0.25f;
                    }
                    else
                        return (false);
                }
            }
            if (first || (!first && dist <= firstDist))
                return (true);
        }
        return (false);
    }

    public void LaunchSpell(Vector3 pos)
    {
        if (InLineOfSight(pos))
        {
            _player.AddMana(-5);
            foreach (SpellEffect se in _selectedSpell.effects)
            {
                int value = Random.Range(se.min, se.max + 1);
                foreach (Player player in triggeredPlayers)
                {
                    if (se.type == SpellEffect.Type.Damage)
                        player.TakeDamage(value);
                    if (se.type == SpellEffect.Type.Heal)
                        player.Heal(value);
                }
            }
        }
        StopSpellLaunching();
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
        if (spellIndex > 0 && spellIndex <= spells.Length && _player.mana >= spells[spellIndex - 1].mana)
        {
            _selectedSpell = spells[spellIndex - 1];
            _rangeZone.SetActive(true);
            _rangeZone.transform.localScale = new Vector3(_selectedSpell.range * 2, _selectedSpell.range * 2);
            CreateEffectZone();
        }
        else
        {
            spellIndex = 0;
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

    RaycastHit2D GetMouseHit()
    {
        return (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 1.0f, ~LayerMask.GetMask("EffectZone")));
    }

    void StopSpellLaunching()
    {
        launchingSpell = false;
        SetActiveSpell(0);
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

    public void OnEffectZoneExit(Collider2D collision, EffectZone zone)
    {
        Player other = collision.GetComponent<Player>();
        if (other == null)
            return;
        if (triggeredPlayers.Contains(other))
        {
            triggeredPlayers.Remove(other);
            other.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}

/*
 
Cooldown
Ligne de vue                    OK
Impossible lancer obstacle      OK

 */