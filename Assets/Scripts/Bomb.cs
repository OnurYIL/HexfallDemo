using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexFallDemo
{
    public class Bomb : Tile
    {
        private TextMesh timerText;

        public int timer = 7;

        public int Tick()
        {
            timer--;

            if (timer > 0)
                transform.DOShakePosition(0.5f, 0.1f);

            DisplayTimer();

            return timer;
        }
        private void Start()
        {
            timerText = transform.GetChild(0).GetComponent<TextMesh>();

            DisplayTimer();
        }

        private void DisplayTimer()
        {
            timerText.text = timer.ToString();
        }



    }
}
