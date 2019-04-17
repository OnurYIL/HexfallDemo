using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexFallDemo
{
    public class Helpers
    {
        public static float CalculateVerticalDistance(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x);
        }
        public static Vector2 CalculateHexaPosition(int x, int y, float diameter, Vector2 startPos)
        {
            float d = diameter;
            float alpha = d / 4;
            float beta = alpha * Mathf.Sqrt(3);
            float posY = (d / 2 + alpha) * y;
            float posX = beta * 2 * x;
            if (y % 2 != 0) { posX -= beta; }
            return startPos + new Vector2(posY, posX);
        }
        /// <summary>
        /// Finds the center of the 3 tiles selected
        /// Can use any component attached to a gameobject.
        /// </summary>
        public static Vector2 FindGroupCenter<T>(T[] tiles) where T : Component
        {
            var totalX = 0f;
            var totalY = 0f;
            foreach (var tile in tiles)
            {
                totalX += tile.transform.position.x;
                totalY += tile.transform.position.y;
            }

            var centerX = totalX / tiles.Length;
            var centerY = totalY / tiles.Length;

            return new Vector2(centerX, centerY);
        }

        public static Vector2 FindGroupCenter(List<GameObject> tiles)
        {
            var totalX = 0f;
            var totalY = 0f;
            foreach (var tile in tiles)
            {
                totalX += tile.transform.position.x;
                totalY += tile.transform.position.y;
            }

            var centerX = totalX / tiles.Count;
            var centerY = totalY / tiles.Count;

            return new Vector2(centerX, centerY);
        }
    }
}
