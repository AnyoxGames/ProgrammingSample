using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace AnyoxGames.Util
{
    public static class CursorUtils
    {
        public static bool IsCursorVisible => Cursor.visible;
        public static bool IsCursorCaptured => Cursor.lockState != CursorLockMode.None;

        private static readonly HashSet<string> CaptureBlockReasons = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Load()
        {
            CaptureBlockReasons.Clear();
        }

        public static void CaptureCursor(bool setInvisible = true, CursorLockMode lockMode = CursorLockMode.Locked)
        {
            if (!CaptureBlockReasons.IsNullOrEmpty())
            {
                return;
            }

            if (lockMode == CursorLockMode.None)
                throw new InvalidExpressionException("Cannot capture cursor with lock mode none");

            Cursor.visible = !setInvisible;
            Cursor.lockState = lockMode;
        }

        public static void ReleaseCursor(bool setVisible = true)
        {
            Cursor.visible = setVisible;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void AddCaptureBlockReason(string reason)
        {
            CaptureBlockReasons.Add(reason);
        }

        public static void RemoveCaptureBlockReason(string reason)
        {
            CaptureBlockReasons.Remove(reason);
        }
    }
}