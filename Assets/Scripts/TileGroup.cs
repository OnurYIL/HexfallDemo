using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexFallDemo
{
    public class TileGroup
    {
        public List<Tile> tiles;

        public void SwapTiles(TileMatrix allTiles, SwapDirection _swapDirection)
        {
            int tempRow2 = tiles[2].row;
            int tempColumn2 = tiles[2].column;

            int tempRow1 = tiles[1].row;
            int tempColumn1 = tiles[1].column;

            int tempRow0 = tiles[0].row;
            int tempColumn0 = tiles[0].column;

            if (_swapDirection == SwapDirection.clockwise)
            {
                tiles[2].row = tempRow1;
                tiles[2].column = tempColumn1;

                tiles[1].row = tempRow0;
                tiles[1].column = tempColumn0;

                tiles[0].row = tempRow2;
                tiles[0].column = tempColumn2;

                allTiles[tempRow1, tempColumn1] = tiles[2].gameObject;
                allTiles[tempRow0, tempColumn0] = tiles[1].gameObject;
                allTiles[tempRow2, tempColumn2] = tiles[0].gameObject;
            }
            else
            {
                tiles[2].row = tempRow0;
                tiles[2].column = tempColumn0;

                tiles[1].row = tempRow2;
                tiles[1].column = tempColumn2;

                tiles[0].row = tempRow1;
                tiles[0].column = tempColumn1;


                allTiles[tempRow0, tempColumn0] = tiles[2].gameObject;
                allTiles[tempRow2, tempColumn2] = tiles[1].gameObject;
                allTiles[tempRow1, tempColumn1] = tiles[0].gameObject;
            }
        }

        /// <summary>
        /// Orders selected tiles in so that we can easily rotate and check them later.
        /// </summary>
        public void OrderTiles()
        {
            //Copy the selected list to a temp list to modify
            List<Tile> tempList;
            tempList = tiles;

            //Group the tiles by the row. Creates 2 lists with first being the list with the highest amount of tiles.
            var ordered = tempList.GroupBy(x => x.row).OrderByDescending(x => x.Count()).ToArray();

            //Order the list with highest amount of same row tiles to go from left to right
            List<Tile> tList = ordered[0].OrderBy(x => x.column).ToList();

            //Clear the actual list since we're going to add the new ordered tiles to it.
            tiles.Clear();

            //Add the ordered list first with the highest amount of same rows.
            tiles.AddRange(tList);

            //If the last tile is under the row with the highest amount of same row tiles, add the last tile to the end of the list.
            //Else, add it to the middle of the list
            if (ordered[1].FirstOrDefault().row < tList[0].row)
                tiles.AddRange(ordered[1]);
            else
                tiles.InsertRange(1, ordered[1]);
        }
        public void OrderTiles1()
        {
        }
        public void SetParent(Transform parent)
        {
            foreach (Tile t in tiles)
                t.transform.SetParent(parent, true);
        }
        public void RemoveParent()
        {
            foreach (Tile t in tiles)
                t.transform.parent = null;
        }
    }
}
