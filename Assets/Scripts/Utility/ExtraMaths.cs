using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class ExtraMaths
    {
        public static float Map(float oldMin, float oldMax, float newMin, float newMax, float value)
        {
            return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }
    }
}
