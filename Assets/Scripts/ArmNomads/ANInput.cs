using UnityEngine;

namespace ArmNomads
{
    public static class ANInput
    {
        public static bool GetPointer
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Input.GetMouseButton(0) || Input.GetMouseButtonUp(0);
#elif UNITY_IOS || UNITY_ANDROID
            return Input.touchCount > 0;
#endif
            }
        }

        public static int TouchCount
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return GetPointer ? 1 : 0;
#elif UNITY_IOS || UNITY_ANDROID
            return Input.touchCount;
#endif
            }
        }

        public static Touch GetTouch(int touchIndex)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Touch t = new Touch();
            t.position = PointerPos;
            t.phase = PointerPhase;
            return t;
#elif UNITY_IOS || UNITY_ANDROID
            return Input.GetTouch(touchIndex);
#endif
        }

        public static Vector2 NormalizedPointerPos
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Input.mousePosition / Mathf.Min(Screen.width, Screen.height);
#elif UNITY_IOS || UNITY_ANDROID
            return Input.GetTouch(0).position / Mathf.Min(Screen.width, Screen.height);
#endif
            }
        }

        public static Vector2 ViewportPointerPos
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
#elif UNITY_IOS || UNITY_ANDROID
            return new Vector2(Input.GetTouch(0).position.x / Screen.width, Input.GetTouch(0).position.y / Screen.height);
#endif
            }
        }

        public static Vector2 PointerPos
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
            return Input.GetTouch(0).position;
#endif
            }
        }

        public static TouchPhase PointerPhase
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                if (Input.GetMouseButtonDown(0))
                    return TouchPhase.Began;
                if (Input.GetMouseButtonUp(0))
                    return TouchPhase.Ended;
                if (Input.GetMouseButton(0))
                    return TouchPhase.Moved;
                return TouchPhase.Canceled;

#elif UNITY_IOS || UNITY_ANDROID
                if(Input.touchCount > 0)
                    return Input.GetTouch(0).phase;
                return TouchPhase.Canceled;
#endif
            }
        }

    }
}