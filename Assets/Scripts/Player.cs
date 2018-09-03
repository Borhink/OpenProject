using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //[SerializeField]
    public float maxLife = 100; //Player's max life
    public float life = 100; //Player's life
    public float maxMana = 20; //Player's max mana
    public float mana; //Player's mana
    public float maxEnergy = 10; //Player's max energy (in unit)
    public float energy; //Player's current energy (in unit)
    public float team = 1;

    //Deplacement
    public float speed = 2; //Player's speed (in unit/sec)
    public bool movementDist = false; //Show movement's range
    public GameObject movementCirclePrefab;
    private GameObject _movementCircle;

    //Sorts
    private SpellCaster _spellcaster; //Class to launch spells

    void Start () {
        _spellcaster = GetComponentInChildren<SpellCaster>();
        energy = maxEnergy;
        mana = maxMana;
        life = maxLife;
        _movementCircle = Instantiate(movementCirclePrefab, transform);
        if (!movementDist)
            _movementCircle.SetActive(!_movementCircle.activeSelf);
	}

    void Update()
    {
        energy += 0.25f * Time.deltaTime;
        if (energy >= maxEnergy)
            energy = maxEnergy;
        mana += 0.5f * Time.deltaTime;
        if (mana >= maxMana)
            mana = maxMana;

        if (movementDist)
            _movementCircle.transform.localScale = new Vector3(energy * 2, energy * 2);

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _spellcaster.SetActiveSpell(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _spellcaster.SetActiveSpell(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            _spellcaster.SetActiveSpell(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            _spellcaster.SetActiveSpell(4);
        if (Input.GetKeyDown(KeyCode.Escape) && _spellcaster.IsActiveSpell())
            _spellcaster.SetActiveSpell(0);

        if (Input.GetKeyDown(KeyCode.M))
        {
            movementDist = !movementDist;
            _movementCircle.SetActive(!_movementCircle.activeSelf);
        }
        if (Input.GetMouseButton(0) && _spellcaster.IsActiveSpell() == false)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            MovementManager.mm.MovePlayer(this, pos);
        }
    }

    public void AddMana(int value)
    {
        mana += value;
        if (mana < 0)
            mana = 0;
        else if (mana > maxMana)
            mana = maxMana;
    }

    public void Heal(int value)
    {
        life += value;
        if (life > maxLife)
        {
            life = maxLife;
        }
    }

    public bool TakeDamage(int value)
    {
        life -= value;
        if (life < 0)
        {
            life = 0;
            return (false);
        }
        return (true);
    }
}
