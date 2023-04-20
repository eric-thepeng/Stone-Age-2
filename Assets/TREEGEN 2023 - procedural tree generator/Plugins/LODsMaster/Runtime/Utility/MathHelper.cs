using System;
using System.Runtime.CompilerServices;

namespace UnityMeshSimplifier
{
    /// <summary>
    /// Math helpers.
    /// </summary>
    public static class MathHelper
    {
        #region Consts
        /// <summary>
        /// The Pi constant.
        /// </summary>
        public const float PI = 3.14159274f;

        /// <summary>
        /// The Pi constant.
        /// </summary>
        public const double PId = 3.1415926535897932384626433832795;

        /// <summary>
        /// Degrees to radian constant.
        /// </summary>
        public const float Deg2Rad = PI / 180f;

        /// <summary>
        /// Degrees to radian constant.
        /// </summary>
        public const double Deg2Radd = PId / 180.0;

        /// <summary>
        /// Radians to degrees constant.
        /// </summary>
        public const float Rad2Deg = 180f / PI;

        /// <summary>
        /// Radians to degrees constant.
        /// </summary>
        public const double Rad2Degd = 180.0 / PId;
        #endregion

        #region Min
        /// <summary>
        /// Returns the minimum of three values.
        /// </summary>
        /// <param name="val1">The first value.</param>
        /// <param name="val2">The second value.</param>
        /// <param name="val3">The third value.</param>
        /// <returns>The minimum value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(double val1, double val2, double val3)
        {
            return (val1 < val2 ? (val1 < val3 ? val1 : val3) : (val2 < val3 ? val2 : val3));
        }
        #endregion

        #region Clamping
        /// <summary>
        /// Clamps a value between a minimum and a maximum value.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>The clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            return (value >= min ? (value <= max ? value : max) : min);
        }
        #endregion

        #region Triangle Area
        /// <summary>
        /// Calculates the area of a triangle.
        /// </summary>
        /// <param name="p0">The first point.</param>
        /// <param name="p1">The second point.</param>
        /// <param name="p2">The third point.</param>
        /// <returns>The triangle area.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double TriangleArea(ref Vector3d p0, ref Vector3d p1, ref Vector3d p2)
        {
            var dx = p1 - p0;
            var dy = p2 - p0;
            return dx.Magnitude * (Math.Sin(Vector3d.Angle(ref dx, ref dy) * Deg2Radd) * dy.Magnitude) * 0.5f;
        }
        #endregion
    }
}