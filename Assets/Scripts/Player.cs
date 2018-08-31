using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    //[SerializeField]
    public float hp = 100;
    //private float mana = 20;
    //[HideInInspector]
    //private PathFinder _pf;

    //Deplacement
    public float speed = 2;
    public float maxEnergy = 10;
    public float energy;
    public bool movementDist = false;
    public GameObject movementCirclePrefab;
    private GameObject _movementCircle;

    //Sorts
    private SpellCaster _spellcaster;

    void Start () {
        //_pf = GetComponent<PathFinder>();
        _spellcaster = GetComponent<SpellCaster>();
        energy = maxEnergy;
        _movementCircle = Instantiate(movementCirclePrefab, transform);
        if (!movementDist)
            _movementCircle.SetActive(!_movementCircle.activeSelf);
	}

    void Update()
    {
        energy += 0.25f * Time.deltaTime;
        if (energy >= maxEnergy)
            energy = maxEnergy;

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
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = transform.position.z;
            MovementManager.mm.MovePlayer(this, pos);
        }
    }
}
