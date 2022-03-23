using UnityEngine;
using System;
public static class Utils
{

    public static readonly float elementaryCharge = 1.602176634f*Mathf.Pow(10, -19);
    public static readonly float epsilonZero = 0.0000000000088541878128f;
    public static readonly float k = 1/(4*Mathf.PI*Utils.epsilonZero);
    public static readonly float speedOfLight = 299792458f;
    public static readonly float speedOfLightSquared = Mathf.Pow(Utils.speedOfLight, 2);

    public static readonly float speedOfSound = 343f;
    public static readonly float speedOfSoundSquared = Mathf.Pow(Utils.speedOfSound, 2);
    public static readonly float atmosphericPressure = 101325f;
    
    public static bool GetRandomBool(System.Random rand)
    {
        return rand.Next(0, 1) == 1;
    }
    public static int GetRandomSign(System.Random rand)
    {
        return rand.Next(0, 1)*2-1;
    }
    public static float GetRandomMultiplier(System.Random rand)
    {
        return (float)rand.Next(0, Int32.MaxValue)/(float)Int32.MaxValue;
    }
    public static float GetRandomWeigthLin(System.Random rand)
    {
        return Utils.GetRandomSign(rand)*Utils.GetRandomMultiplier(rand);
    }
    public static Quaternion GetRandomDirection(System.Random rand)
    {
        return Quaternion.Euler(rand.Next(0, 360), rand.Next(0, 360), 0);
    }
}