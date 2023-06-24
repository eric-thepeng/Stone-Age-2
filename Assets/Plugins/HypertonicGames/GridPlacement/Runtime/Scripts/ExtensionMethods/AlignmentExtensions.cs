using Hypertonic.GridPlacement.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hypertonic.GridPlacement
{
    public static class AlignmentExtensions 
    {
        public static bool IsLeftAligned(this ObjectAlignment alignment)
        {
            if(alignment.Equals(ObjectAlignment.UPPER_LEFT)
                || alignment.Equals(ObjectAlignment.MIDDLE_LEFT)
                || alignment.Equals(ObjectAlignment.BOTTOM_LEFT))
            {
                return true;
            }

            return false;
        }

        public static bool IsRightAligned(this ObjectAlignment alignment)
        {
            if (alignment.Equals(ObjectAlignment.UPPER_RIGHT)
                || alignment.Equals(ObjectAlignment.MIDDLE_RIGHT)
                || alignment.Equals(ObjectAlignment.BOTTOM_RIGHT))
            {
                return true;
            }

            return false;
        }

        public static bool IsUpperAligned(this ObjectAlignment alignment)
        {
            if (alignment.Equals(ObjectAlignment.UPPER_LEFT)
                || alignment.Equals(ObjectAlignment.UPPER_MIDDLE)
                || alignment.Equals(ObjectAlignment.UPPER_RIGHT))
            {
                return true;
            }

            return false;
        }

        public static bool IsBottomAligned(this ObjectAlignment alignment)
        {
            if (alignment.Equals(ObjectAlignment.BOTTOM_RIGHT)
                || alignment.Equals(ObjectAlignment.BOTTOM_MIDDLE)
                || alignment.Equals(ObjectAlignment.BOTTOM_RIGHT))
            {
                return true;
            }

            return false;
        }

        public static bool IsXAligned(this ObjectAlignment alignment)
        {
            return alignment.IsLeftAligned() || alignment.IsRightAligned();
        }

        public static bool IsYAligned(this ObjectAlignment alignment)
        {
            return alignment.IsUpperAligned() || alignment.IsBottomAligned();
        }

        public static bool IsCenterAligned(this ObjectAlignment alignment)
        {
            return alignment.Equals(ObjectAlignment.CENTER);
        }
    }
}