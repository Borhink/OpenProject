using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager mm = null;

    void Awake()
    {
        if (mm == null)
            mm = this;
        else if (mm != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void MovePlayer(Player player, Vector2 dest)
    {
        Vector2 dir = dest - new Vector2(player.transform.position.x, player.transform.position.y);
        dir.Normalize();
        if (CanMoveTo(player, dir))
        {
            player.transform.Translate(dir * player.speed * Time.deltaTime);
            player.energy -= player.speed * Time.deltaTime;
        }
    }

    public bool CanMoveTo(Player player, Vector2 dir)
    {
        if (player.energy <= 0.1f)
            return (false);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(player.transform.position, 0.25f, dir, 0.1f + 1 * Time.deltaTime);
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject != player.gameObject)
                    return (false);
            }
        }
        return (true);
    }
}
