using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallPit : MonoBehaviour
{
    public static readonly HashSet<Ball> balls = new HashSet<Ball>();
    public static readonly float scale = 1f;
    public static readonly System.Random rand = new System.Random(DateTime.Now.Millisecond);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static int RandomSeed()
    {
        return BallPit.rand.Next(Int32.MinValue, Int32.MaxValue);
    }

    public class Random : System.Random
    {
        public Random() : base(BallPit.RandomSeed())
        {
            
        }
    }
}
