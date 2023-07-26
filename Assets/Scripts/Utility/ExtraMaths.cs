using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    
    public class ExtraMaths
    {
        private static System.Random rand = new System.Random();
        public static float Map(float oldMin, float oldMax, float newMin, float newMax, float value)
        {
            return (value - oldMin) / (oldMax - oldMin) * (newMax - newMin) + newMin;
        }

        public static float FloatRandom(float min, float max)
        {
            float returnNumber = Map(int.MinValue, int.MaxValue, min, max, rand.Next(int.MinValue, int.MaxValue));

            return returnNumber;
        }
         
        public static int IntRandom(int min, int max)
        {
            
            int returnNumber = (int)Map(int.MinValue, int.MaxValue, min, max, rand.Next(int.MinValue, int.MaxValue));

            return returnNumber;

        }
    }
}
