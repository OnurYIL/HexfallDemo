using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexFallDemo
{
    public class TileMatrix
    {
        private GameObject[,] tiles;
        private ParticleSystem explosionParticle;
        bool added = false;
        public List<GameObject> totalMatches = new List<GameObject>();

        public TileMatrix(int row, int column)
        {
            tiles = new GameObject[row, column];
            explosionParticle = GameObject.FindGameObjectWithTag("Explosion").GetComponent<ParticleSystem>();
        }

        public GameObject this[int row, int column]
        {
            get
            {
                try { return tiles[row, column]; }
                catch (Exception e) { throw e; }
            }
            set { tiles[row, column] = value; }
        }

        /// <summary>
        /// Checks all 6 corners of a single tile 
        /// to find 2 or more matching tiles that are also neighbours.
        /// If do so, recursively check those new tiles to find more matches.
        /// </summary>
        public void GetMatchesTest(GameObject go)
        {
            List<GameObject> localMatches = new List<GameObject>();

            localMatches.Add(go);

            var tile = go.GetComponent<Tile>();

            {
                //Check for even columns
                if (tile.column % 2 == 0)
                {
                    {
                        try
                        {
                            ///      *
                            ///  *       m
                            ///      m
                            ///  *       m 
                            ///      * 
                            if (Match3(tile, tiles[tile.row, tile.column + 1].GetComponent<Tile>(), tiles[tile.row + 1, tile.column + 1].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row, tile.column + 1]);
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column + 1]);
                            }
                        }
                        catch { }
                        try
                        {
                            ///      m
                            ///  *       m
                            ///      m
                            ///  *       * 
                            ///      * 
                            if (Match3(tile, tiles[tile.row + 1, tile.column + 1].GetComponent<Tile>(), tiles[tile.row + 1, tile.column].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column + 1]);
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (Match3(tile, tiles[tile.row + 1, tile.column].GetComponent<Tile>(), tiles[tile.row + 1, tile.column - 1].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column]);
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column - 1]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (Match3(tile, tiles[tile.row + 1, tile.column - 1].GetComponent<Tile>(), tiles[tile.row, tile.column - 1].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row + 1, tile.column - 1]);
                                AddIfNone(localMatches, tiles[tile.row, tile.column - 1]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (Match3(tile, tiles[tile.row, tile.column - 1].GetComponent<Tile>(), tiles[tile.row - 1, tile.column].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row, tile.column - 1]);
                                AddIfNone(localMatches, tiles[tile.row - 1, tile.column]);
                            }
                        }
                        catch { }
                        try
                        {
                            if (Match3(tile, tiles[tile.row - 1, tile.column].GetComponent<Tile>(), tiles[tile.row, tile.column + 1].GetComponent<Tile>()))
                            {
                                AddIfNone(localMatches, tiles[tile.row - 1, tile.column]);
                                AddIfNone(localMatches, tiles[tile.row, tile.column + 1]);
                            }
                        }
                        catch { }
                    }
                }
                ///Check for odd columns
                else
                {
                    try
                    {
                        if (Match3(tile, tiles[tile.row, tile.column + 1].GetComponent<Tile>(), tiles[tile.row - 1, tile.column + 1].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row, tile.column + 1]);
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column + 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (Match3(tile, tiles[tile.row - 1, tile.column + 1].GetComponent<Tile>(), tiles[tile.row - 1, tile.column].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column + 1]);
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (Match3(tile, tiles[tile.row - 1, tile.column].GetComponent<Tile>(), tiles[tile.row - 1, tile.column - 1].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column]);
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column - 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (Match3(tile, tiles[tile.row - 1, tile.column - 1].GetComponent<Tile>(), tiles[tile.row, tile.column - 1].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row - 1, tile.column - 1]);
                            AddIfNone(localMatches, tiles[tile.row, tile.column - 1]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (Match3(tile, tiles[tile.row, tile.column - 1].GetComponent<Tile>(), tiles[tile.row + 1, tile.column].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row, tile.column - 1]);
                            AddIfNone(localMatches, tiles[tile.row + 1, tile.column]);
                        }
                    }
                    catch { }
                    try
                    {
                        if (Match3(tile, tiles[tile.row + 1, tile.column].GetComponent<Tile>(), tiles[tile.row, tile.column + 1].GetComponent<Tile>()))
                        {
                            AddIfNone(localMatches, tiles[tile.row + 1, tile.column]);
                            AddIfNone(localMatches, tiles[tile.row, tile.column + 1]);
                        }
                    }
                    catch { }
                }
            }
            //If any new tile is added to the matches, do another check for the new tiles.
            if (added)
            {
                added = false;

                //Remove the actualy tile we're checking against, since it was already added.
                if (totalMatches.Contains(localMatches[0]))
                    localMatches.RemoveAt(0);

                totalMatches.AddRange(localMatches);

                foreach (var _go in localMatches)
                {
                    GetMatchesTest(_go);
                }
            }
        }

        /// <summary>
        /// Adds a tile to the matches list if it doesn't already contain that one.
        /// </summary>
        void AddIfNone(List<GameObject> matches, GameObject go)
        {
            if (!totalMatches.Contains(go))
            {
                matches.Add(go);
                added = true;
            }
        }

        /// <summary>
        /// Checks if 3 tiles are of the same color.
        /// </summary>
        bool Match3(Tile t0, Tile t1, Tile t2)
        {
            if (t0.Compare(t1))
                return t1.Compare(t2);
            return false;
        }


        /// <summary>
        /// 2 overload functions for checking a single tile 
        /// and checking a selection group. 
        /// </summary>
        public TileMatches CheckTileForMaches(TileGroup group)
        {
            TileMatches matches = new TileMatches();
            //_gameState = GameState.matching;
            foreach (var go in group.tiles)
            {
                matches.AddObjectRange(CheckTileForMaches(go.gameObject).MatchedTiles);
            }

            return matches;
        }

        public TileMatches CheckTileForMaches(GameObject go)
        {
            TileMatches matches = new TileMatches();
            //_gameState = GameState.matching;
            GetMatchesTest(go);

            if (totalMatches.Count > 2)
            {
                matches.AddObjectRange(totalMatches);

                Explode(Helpers.FindGroupCenter(totalMatches), totalMatches[0].GetComponent<Tile>().color);

            }

            ClearMatches();

            return matches;
        }

        /// <summary>
        /// Checks for potential matches on all grid.
        /// If can't find any, game is over.
        /// </summary>
        /// <returns></returns>
        public bool CheckPotentialMatches()
        {
            foreach (var tile in tiles)
            {
                bool val = CheckPotentialMatches(tile.GetComponent<Tile>());
                if (val)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check a tile for potential matches.
        /// This works much like the matching algorithm.
        /// Only difference is that this one starts with an overlap cast on the actual tile.
        /// If returned objects contains a match, means now we have 2 tiles side by side,
        /// we check for the group of tiles that can create a 3 match when rotated. Example:
        /// If we find 2 tiles like this;  
        ///      *
        ///  *       m
        ///      m
        ///  *       * 
        ///      * 
        /// we cast another overlap on P tiles that has a radius of that reaches to the surrounding 6 tiles only. 
        /// If those 6 tiles which aren't the ones we already checked, contains another match,
        /// we call this a potential match and return true.
        ///      p
        ///  *       m
        ///      m
        ///  *       P 
        ///      * 
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        private bool CheckPotentialMatches(Tile tile)
        {
            Debug.Log("Checking for potential matches..");
            List<GameObject> matchedTiles = new List<GameObject>();
            List<GameObject> secondaryTiles = new List<GameObject>();

            matchedTiles.Add(tile.gameObject);
            matchedTiles.AddRange(CheckTile(tile, matchedTiles));

            secondaryTiles.AddRange(matchedTiles);

            foreach (var tile1 in matchedTiles)
                Debug.Log("These are in: [" + tile1.GetComponent<Tile>().row + "," + tile1.GetComponent<Tile>().column + "]");

            if (matchedTiles.Count > 1)
            {
                for (int i = 1; i < matchedTiles.Count; i++)
                {
                    if (tile.column % 2 == 0)
                    {
                        if (tile.row == matchedTiles[i].GetComponent<Tile>().row)
                        {
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                            try
                            {

                                secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                        }
                        else if (tile.column == matchedTiles[i].GetComponent<Tile>().column)
                        {
                            if (tile.row < matchedTiles[i].GetComponent<Tile>().row)
                            {
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column - 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                                try
                                {

                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                            }
                            else
                                continue;
                        }
                        else if (tile.column < matchedTiles[i].GetComponent<Tile>().column)
                        {
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                            try
                            {

                                secondaryTiles.AddRange(CheckTile(tiles[tile.row, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                        }
                        else
                        {
                            if (tile.column > matchedTiles[i].GetComponent<Tile>().column)
                                continue;

                            try
                            {

                                secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row, tile.column - 1].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        if (tile.row == matchedTiles[i].GetComponent<Tile>().row)
                        {
                            if (tile.column < matchedTiles[i].GetComponent<Tile>().column)
                            {
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                            }
                            else
                                continue;

                        }
                        else if (tile.column == matchedTiles[i].GetComponent<Tile>().column)
                        {
                            if (tile.row > matchedTiles[i].GetComponent<Tile>().row)
                            {
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column - 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                            }
                            else
                            {
                                continue;
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row, tile.column - 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                                try
                                {
                                    secondaryTiles.AddRange(CheckTile(tiles[tile.row, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                                }
                                catch { }
                            }
                        }
                        else if (tile.column > matchedTiles[i].GetComponent<Tile>().column)
                        {
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column - 1].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row + 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row, tile.column + 1].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                            try
                            {
                                secondaryTiles.AddRange(CheckTile(tiles[tile.row - 1, tile.column].GetComponent<Tile>(), secondaryTiles));
                            }
                            catch { }
                        }
                    }
                    if (secondaryTiles.Count > 2)
                    {
                        Debug.Log(secondaryTiles.Count);
                        for (int j = 0; j < 3; j++)
                        {
                            Debug.Log("Final tiles:[" + secondaryTiles[j].GetComponent<Tile>().row + "," + secondaryTiles[j].GetComponent<Tile>().column + "]");
                        }

                        return true;
                    }
                    else
                        return false;
                }
            }

            return false;
        }

        public IEnumerator CollapseAllTiles()
        {
            foreach (var tile in tiles)
            {
                CollapseTile(tile);
                yield return new WaitForSeconds(0.01f);
            }
        }

        void CollapseTile(GameObject tile)
        {
            tile.transform.DOMove(new Vector3(tile.transform.position.x, -(Values.WorldScreenSize.y / 2) - (Values.TileSize.x / 2), 0), 0.01f);
        }

        private List<GameObject> CheckTile(Tile tile, List<GameObject> checkedTiles)
        {
            Debug.Log("Checking [" + tile.row + "," + tile.column + "]");

            List<GameObject> foundTiles = new List<GameObject>();
            List<Collider2D> candidateTiles = Physics2D.OverlapCircleAll(tile.transform.position, Values.TileSize.x / 2).ToList();

            candidateTiles.RemoveAt(0);
            foreach (var _candidateTile in candidateTiles)
            {
                Debug.Log("Candidate:" + "[" + _candidateTile.GetComponent<Tile>().row + ", " + _candidateTile.GetComponent<Tile>().column + "]");

                if (checkedTiles.Contains(_candidateTile.gameObject))
                {
                    Debug.Log("Already contains this.");
                    continue;
                }
                if (checkedTiles[0].GetComponent<Tile>().Compare(_candidateTile.GetComponent<Tile>()))
                {
                    Debug.Log("Found [" + tile.row + "," + tile.column + "] - Matching");
                    foundTiles.Add(_candidateTile.gameObject);
                }
            }
            return foundTiles;

        }

        void ClearMatches()
        {
            totalMatches = new List<GameObject>();
        }

        void Explode(Vector2 position, Color color)
        {
            explosionParticle.startColor = color;
            explosionParticle.transform.position = position;
            explosionParticle.Play();
        }
    }
}
