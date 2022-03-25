using System;
using System.Text; 
using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    public long age = 0;
    public long killCount = 0;
    public float fatigueCoefficient = 1;
    public float charge = 0;
    public readonly float density = 0.001f;
    public float[] voice = new float[0];

    public Behavioural[] behaviourals;
    public new Rigidbody rigidbody;
    public MeshRenderer meshrenderer;

    // Start is called before the first frame update
    public void Start()
    {
        BallPit.balls.Add(this);
        //this.scale /= BallPit.scale;

        this.behaviourals = new Behavioural[]{
            new MemoryStack(this, BallPit.rand.Next(1, 32), BallPit.rand.Next(1, 32), 2),
            ////new SelfDigester(this),
            new Vocalizer(this, 16, 0.1f),
            new Accelerometer(this, 10f),
            new AgeKnower(this, 0.0001f),
            ////new ChargeManipulator(this, 10000000000000000000f),
            new Colorizer(this),
            new FatigueKnower(this, 16),
            new MassKnower(this, 1f),
            new SelfSurgeon(this),
            new SelfMutator(this, 16),
            new TorqueEngine(this, 360),
            new Wiggler(this, 15000f, 2),
            new Ears(this, 16, 100)
        };

        (int isize, int osize) = this.IOSize();

        LayerGroup brainStructure = new Layer(isize) + Neurons.GenerateHiddenLayers(BallPit.rand.Next(2, 64), 2) + new Layer(osize);

        this.neurons = brainStructure.GenerateNeurons();

        this.rigidbody = this.GetComponent<Rigidbody>();
        this.meshrenderer = this.GetComponent<MeshRenderer>();
        this.radius = this.transform.localScale[0];
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void FixedUpdate()
    {
        if(this.transform.position.y < -10)
        {
            this.Kill();
        }
        else
        {
            if(neurons != null)
            {
                this.neurons.UpdateNeurons(64);
            }
            this.UpdateBehaviour();

            this.age++;
        }
    }

    #region behaviourals
        private (int, int) IOSize()
        {
            int isize = 0, osize = 0;
            for(int i = 0, l = this.behaviourals.Length; i < l; i++)
            {
                isize += this.behaviourals[i].inputLayerNodes;
                osize += this.behaviourals[i].outputLayerNodes;
            }
            return (Math.Max(1, isize), Math.Max(1, osize));
        }
    #endregion
    
    #region neurons
        public Neurons neurons;
    #endregion

    #region behaviour
        private void UpdateBehaviour()
        {
            if(this.neurons == null) this.Kill();

            (int isize, int osize) = this.IOSize();

            int si = 0;
            int so = 0;
            for(int i = 0, l = this.behaviourals.Length; i < l; i++)
            {
                Behavioural b = behaviourals[i];
                if(!b.UpdateNeurons(this.neurons, si, so - osize))
                {
                    this.Kill();
                }
                si += b.inputLayerNodes;
                so += b.outputLayerNodes;
            }
        }
    #endregion

    #region life
        public void Kill()
        {
            BallPit.balls.Remove(this);
            UnityEngine.Object.Destroy(this.gameObject);
        }
        public static HashSet<Ball> GetLiving()
        {
            return BallPit.balls;
        }
    #endregion

    #region fatigue system
        private float _fatigue = 0;
        public float fatigue
        {
            get
            {
                return this._fatigue;
            }
            set
            {
                this._fatigue = Mathf.Max(0, value);
                this.fatigueCoefficient = Mathf.Exp(-value/1000);
            }
        }
        public void AddFatigue(float energy)
        {
            this.fatigue += energy;
        }
    #endregion
    
    #region physics
        public float mass
        {
            get
            {
                return this.density*this.volume;
            }
            set
            {
                this.volume = value/this.density;
            }
        }
    #endregion

    #region geometry
        #region world
            public Vector3 position
            {
                get
                {
                    return this.transform.position;
                }
                set
                {
                    this.transform.position = value;
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return this.rigidbody.velocity;
                }
                set
                {
                    this.rigidbody.velocity = value;
                }
            }
        #endregion
        #region sphere
            public float _radius = 1f;
            public float radius
            {
                get
                {
                    return this._radius;
                }
                set
                {
                    if(value < 0.001f)
                    {
                        this.Kill();
                        return;
                    }
                    this._radius = value;
                    
                    //UPDATE UNITY GAMEOBJECT
                    this.transform.localScale = this.scale;
                    this.rigidbody.mass = this.mass;
                }
            }
            public float diameter
            {
                get
                {
                    return 2.0f*this.radius;
                }
                set
                {
                    this.radius = value/2.0f;
                }
            }
            public float circumference
            {
                get
                {
                    return Utils.radian*this.radius*this.radius;
                }
                set
                {
                    this.radius = Mathf.Sqrt(value/Utils.radian);
                }
            }
            public float volume
            {
                get
                {
                    return Utils.spherePerCubeVolume*this.radius*this.radius*this.radius;
                }
                set
                {
                    this.radius = Utils.Cbrt(value/Utils.spherePerCubeVolume);
                }
            }
            public float surfaceArea
            {
                get
                {
                    return Utils.spherePerQuadrantArea*this.radius*this.radius;
                }
            }
            public Vector3 scale
            {
                get
                {
                    return Vector3.one*(this.radius*BallPit.scale);
                }
            }
        #endregion
    #endregion
}
