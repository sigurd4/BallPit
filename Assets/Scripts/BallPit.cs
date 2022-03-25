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

    void FixedUpdate()
    {
        this.UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        Ball[] balls = Utils.ToArray(Ball.GetLiving());

        for(int i = 0; i < balls.Length; i++)
        {
            Ball ball1 = balls[i];
            Vector3 pos1 = ball1.transform.position;
            if(ball1 != null)
            {
                for(int j = 0; j < i; j++)
                {
                    Ball ball2 = balls[j];
                    if(ball2 != null)
                    {
                        Vector3 pos2 = ball2.transform.position;
                        Vector3 r = pos2 - pos1;
                        Vector3 force = r.normalized*Time.deltaTime*Utils.k*ball1.charge*ball2.charge/Mathf.Pow(r.magnitude, 2);
                        if(!Utils.IsFinite(force))
                        {
                            return;
                        }
                        ball1.GetComponent<Rigidbody>().AddForce(force);
                        ball2.GetComponent<Rigidbody>().AddForce(-force);
                    }
                }
            }
        }
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
