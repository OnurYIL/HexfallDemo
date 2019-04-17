using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexFallDemo
{
    public class Tile : MonoBehaviour
    {
        public Color color;
        public int row, column;
        public bool matchable = true;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// Compare a tile color with another one.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Compare(Tile t)
        {
            return t.color == color;
        }

        /// <summary>
        /// Apply the values of the tile t to the tile.
        /// </summary>
        /// <param name="t"></param>
        public void Apply(Tile t)
        {
            row = t.row;
            column = t.column;
            color = t.color;


            ApplyColor();

        }
        void ApplyColor()
        {
            GetComponent<SpriteRenderer>().color = color;
        }

        /// <summary>
        /// When a match is happened and tiles removed,
        /// this function moves down the tiles above it.
        /// </summary>
        /// <param name="allTiles"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public IEnumerator SwapDownRow(TileMatrix allTiles, Tile t)
        {
            int tempRow0 = row;
            int tempRow1 = t.row;
            row = tempRow1;
            t.row = tempRow0;

            allTiles[row, t.column] = gameObject;
            allTiles[t.row, t.column] = t.gameObject;

            var tempPos1 = transform.position;
            var tempPos2 = t.transform.position;

            t.transform.DOMove(tempPos1, 0.01f);
            transform.DOMove(tempPos2, 0.01f);



            yield return null;

        }

    }
}
