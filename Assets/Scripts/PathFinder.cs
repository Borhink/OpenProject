using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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

    public Node(Node src)
    {
        cost = src.cost;
        heuristic = src.heuristic;
        pos = src.pos;
        parent = src.parent;
    }

    public static bool operator >(Node a, Node b)
    {
        if (a.getValue() > b.getValue())
            return (true);
        return (false);
    }

    public static bool operator <(Node b, Node a)
    {
        if (a.getValue() > b.getValue())
            return (true);
        return (false);
    }

    public float getValue()
    {
        return ((float)cost * 0.1f + heuristic);
    }

}

public class PathFinder : MonoBehaviour {
    static readonly float Step = 0.5f;
    //static readonly float Halfstep = Step / 2;
    static readonly Vector2[] Directions = { new Vector2(0, Step), new Vector2(Step, Step), new Vector2(Step, 0), new Vector2(Step, -Step), new Vector2(0, -Step), new Vector2(-Step, -Step), new Vector2(-Step, 0), new Vector2(-Step, Step) };

    private Player _player;
    private List<Node> _opened = new List<Node>();
    private List<Node> _closed = new List<Node>();

    public GameObject test;//DEBUG
    List<GameObject> listy = new List<GameObject>();//DEBUG

    void Start()
    {
        _player = GetComponent<Player>();
    }

    public bool CanWalk(Vector2 origin, Vector2 dir)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, 0.25f, dir, 1);
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

    List<Node> CreateChildren(Node cur, Vector2 dest)
    {
        List<Node> children = new List<Node>();

        for (int i = 0; i < Directions.Count(); i++)
        {
            Vector2 next = cur.pos + Directions[i];

            if (CanWalk(cur.pos, Directions[i]) && cur.cost + 1 + i % 2 <= _player.energy)
                children.Add(new Node(cur.cost + 1 + i % 2, (next - dest).sqrMagnitude, RoundToNearestHalf(next)));
        }
        return (children);
    }

    static public Vector2 RoundToNearestHalf(Vector2 vec)//RoundToLowerHalf
    {
        return (new Vector2(vec.x - (vec.x % Step), vec.y - (vec.y % Step)));
    }

    static public float NodeDistance(Vector2 vec)
    {
        return ((Mathf.Abs(vec.x) + Mathf.Abs(vec.y)) * 2);
    }

    public bool FindPath(Vector2 dest)
    {
        Node node, child, open, close;
        Vector2 start = _player.transform.position;

        _opened.Clear();//DEBUG
        _closed.Clear();//DEBUG
        foreach (GameObject go in listy)//DEBUG
        {//DEBUG
            Destroy(go);//DEBUG
        }//DEBUG
        listy.Clear();//DEBUG

        _opened.Add(new Node(0, NodeDistance(start - dest), RoundToNearestHalf(_player.transform.position)));
        if (NodeDistance(dest - start) > _player.energy)
            return (false);
        while (_opened.Count > 0)
        {
            node = _opened.First();
            listy.Add(Instantiate(test, node.pos, Quaternion.identity));//DEBUG
            if (NodeDistance(node.pos - start) <= _player.energy)
            {
                if ((dest - node.pos).sqrMagnitude < Step * Step)
                    return (true);
                List<Node> children = CreateChildren(node, dest);
                while (children.Count > 0)
                {
                    child = children.First();
                    open = _opened.Find(o => o.pos == child.pos);
                    close = _closed.Find(o => o.pos == child.pos);
                    if ((open != null && child > open) || (close != null && child > close))
                        children.Remove(children.First());
                    else
                    {
                        _opened.Add(new Node(child));
                        if (open != null)
                            _opened.Remove(open);
                        _opened = _opened.OrderBy(o => o.getValue()).ToList();
                        children.Remove(children.First());
                    }
                }
            }
            close = _closed.Find(o => o.pos == node.pos);
            if (close != null)
                _closed.Remove(close);
            _closed.Add(new Node(node));
            _opened.Remove(node);
        }
        return (false);
    }
}