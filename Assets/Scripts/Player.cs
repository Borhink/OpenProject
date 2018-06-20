using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private float _speed = 2;
    private PathFinder _pf;

	void Start () {
        _pf = GetComponent<PathFinder>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetMouseButtonDown(0))
        {
            /*Ray ray = camera.main.screenpointtoray(input.mouseposition);
            raycasthit hit;
            physics.Raycast(ray, out hit);*/
            Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            dir = dir.normalized;
            if (_pf.CanWalk(transform.position, dir))
                transform.Translate(dir * 0.5f);
            else
                Debug.Log("Cant walk !");
            //transform.position = Vector2.MoveTowards(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), _speed * Time.fixedDeltaTime);
        }
    }
}
