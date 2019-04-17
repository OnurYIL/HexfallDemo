using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HexFallDemo
{
    public class CustomInputManager : MonoBehaviour, IPointerUpHandler
    {
        private Vector2 startPos;
        private Vector2 direction;

        public Text debugText;


        public static bool isEnabled = true;

        public delegate void OnScreenTouch(Vector2 touchPos);
        public event OnScreenTouch onScreenTouchEvent;

        public delegate void OnScreenDrag(SwipeDirection swipeDirection);
        public event OnScreenDrag OnScreenDragEvent;
        // Start is called before the first frame update
        void Start()
        {
            isEnabled = true;
        }
        // Update is called once per frame
        void Update()
        {
            if ((Input.touchCount > 0) && isEnabled)
            {
                Touch touch = Input.GetTouch(0);

                if (Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(touch.position)) != null)
                {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            startPos = touch.position;
                            direction = Vector2.zero;
                            break;

                        case TouchPhase.Moved:
                            direction = touch.position - startPos;
                            break;

                        case TouchPhase.Stationary:

                            break;
                        case TouchPhase.Ended:
                            {
                                if (direction.magnitude > 2)
                                {
                                    if (direction.x > 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                                        OnScreenDragEvent?.Invoke(SwipeDirection.right);

                                    else if (direction.x < 0 && Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                                        OnScreenDragEvent?.Invoke(SwipeDirection.left);

                                    else if (direction.y > 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                                        OnScreenDragEvent?.Invoke(SwipeDirection.up);

                                    else if (direction.y < 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                                        OnScreenDragEvent?.Invoke(SwipeDirection.down);
                                }
                                else
                                    onScreenTouchEvent?.Invoke(Camera.main.ScreenToWorldPoint(touch.position));
                            }
                            break;
                    }
                }


            }
        }
        public Vector2 GetSwipeStartPosition()
        {
            return Camera.main.ScreenToWorldPoint(startPos);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
        }
    }
}