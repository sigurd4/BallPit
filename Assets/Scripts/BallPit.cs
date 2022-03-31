using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class BallPit : MonoBehaviour
{
    public static readonly float unitFrequency = 220f;
    public static readonly float audioVolume = 0.1f;
    public static readonly HashSet<Ball> balls = new HashSet<Ball>();
    public static readonly System.Random rand = new System.Random(DateTime.Now.Millisecond);
    public static BallPit ballpit;

    public int capacity;
    public int spawnCountdown;
    public int spawnInterval = 100;

    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {
        BallPit.ballpit = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        this.UpdatePhysics();
        if(BallPit.balls.Count < capacity)
        {
            if(spawnCountdown > 0)
            {
                spawnCountdown--;
            }
            else
            {
                spawnCountdown = spawnInterval;
                GameObject go = this.NewBall();
                Ball ball = go.GetComponent<Ball>();
                ball.Init();
            }
        }
    }

    public GameObject NewBall()
    {
        int count = BallPit.balls.Count;
        if(count >= capacity)
        {
            return null;
        }
        GameObject ball = (GameObject)PrefabUtility.InstantiatePrefab(this.ballPrefab, this.GetComponent<Transform>());
        ball.name = "Ball (" + count + ")";
        ball.SetActive(true);
        return ball;
    }

    private void UpdatePhysics()
    {
        Ball[] balls = Utils.ToArray(Ball.GetLiving());

        for(int i = 0; i < balls.Length; i++)
        {
            Ball ball1 = balls[i];
            Vector3 pos1 = ball1.transform.position;
            int clen1 = ball1.charge.Length;
            if(ball1 != null)
            {
                for(int j = 0; j < i; j++)
                {
                    Ball ball2 = balls[j];
                    
                    if(ball2 != null)
                    {
                        int clen2 = ball2.charge.Length;
                        Vector3 pos2 = ball2.transform.position;
                        Vector3 r = pos2 - pos1;
                        Vector3 rnorm = r.normalized;
                        float rmag = r.magnitude;
                        Vector3 force = Vector3.zero;
                        for(int n = 0, N = Math.Min(clen1, clen2); n < N; n++)
                        {
                            Vector3 f = rnorm*Time.deltaTime*Utils.k*ball1.charge[n]*ball2.charge[n]/Mathf.Pow(rmag, n + 1);
                            if(Utils.IsFinite(f))
                            {
                                force += f;
                            }
                        }
                        if(Utils.IsFinite(force))
                        {
                            ball1.rigidbody.AddForce(force);
                            ball2.rigidbody.AddForce(-force);
                        }
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
