using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public Vector2 pos;
    public int cost;
    public float heuristic;

    public Node(int _cost, float _heuristic, Vector2 _pos, Node _parent = null)
    {
        cost = _cost;
        heuristic = _heuristic;
        pos = _pos;
        parent = _parent;
    }

}

public class PathFinder : MonoBehaviour {

    static readonly Vector2[] Directions = { new Vector2(0, 1).normalized, new Vector2(1, 1).normalized, new Vector2(1, 0).normalized, new Vector2(1, -1).normalized, new Vector2(0, -1).normalized, new Vector2(-1, -1).normalized, new Vector2(-1, 0).normalized, new Vector2(-1, 1).normalized };

    [SerializeField]
    private float _step = 0.5f;
    private Player _player;
    private List<Node> _opened = new List<Node>();
    private List<Node> _closed = new List<Node>();

    void Start()
    {
        _player = GetComponent<Player>();
        /*_opened.Add(new Node(0, 1, new Vector2(1, 1)));
        _opened.Add(new Node(2, 0, new Vector2(0.5f, 1), _opened.ElementAt(0)));
        _opened.Add(new Node(1, 0, new Vector2(0.5f, 1), _opened.ElementAt(1)));
        _opened = _opened.OrderBy(o => o.heuristic).ThenBy(o => o.cost).ToList();
        _opened.Remove(_opened.First());
        //_opened.Find(o => o.pos == cur.pos + (dir.normalized * 0.5f));
        Node test = _opened.Find(o => o.pos == new Vector2(1, 1));
        if (test != null)
            Debug.Log("test: " + test.cost);
        test = _opened.Find(o => o.pos == new Vector2(0.5f, 1));
        if (test != null)
            Debug.Log("test: " + test.cost);
        else
            Debug.Log("test not found");*/
        Vector2 start = _player.transform.position;
        Vector2 dest = new Vector2(6, 6);
        Node node = new Node(0, (start - dest).sqrMagnitude, _player.transform.position);
        List<Node> children = CreateChildren(node, dest);
        StartCoroutine(testFunction(children));
    }

    IEnumerator testFunction(List<Node> children)
    {
        foreach (Node child in children)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("child pos: " + child.pos + ", heuristic: " + child.heuristic + ", cost: " + child.cost);
            _player.transform.position = child.pos;
        }
    }

    public bool CanWalk(Vector2 origin, Vector2 dir)
    {
        dir = dir.normalized;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, 0.25f, dir, _step);
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != _player.gameObject)
                    return (false);
            }
        }
        return (true);
    }

    public bool FindPath(Vector2 dest)
    {
        Node node;
        Vector2 start = _player.transform.position;

        _opened.Add(new Node(0, (start - dest).sqrMagnitude, _player.transform.position));
        while (_opened.Count > 0)
        {
            node = _opened.First();
            if ((dest - node.pos).sqrMagnitude < _step * _step)
                return (true);
            _opened.Remove(_opened.First());
            List<Node> children = CreateChildren(node, dest);
            while (children.Count > 0)
            {
                children.Remove(children.First());
            }
        }
        return (false);
    }

    List<Node> CreateChildren(Node cur, Vector2 dest)
    {
        List<Node> children = new List<Node>();

        foreach (Vector2 dir in Directions)
        {
            Vector2 next = cur.pos + (dir * _step);
            if (CanWalk(cur.pos, dir))
                children.Add(new Node(cur.cost + 1, (next - dest).sqrMagnitude, next, cur));
        }
        return (children);
    }
}