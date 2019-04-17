using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMatches
{
    private List<GameObject> matchedTiles;


    public IEnumerable<GameObject> MatchedTiles
    {
        get
        {
            return matchedTiles.Distinct();
        }
    }

    public void AddObject(GameObject go)
    {
        if (!matchedTiles.Contains(go))
            matchedTiles.Add(go);
    }

    public void AddObjectRange(IEnumerable<GameObject> gos)
    {
        foreach (var item in gos)
        {
            AddObject(item);
        }
    }

    public TileMatches()
    {
        matchedTiles = new List<GameObject>();
    }
}
