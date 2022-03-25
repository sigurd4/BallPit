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
    public readonly float density = 10.0f;
    public float[] voice = new float[0];

    private int behaviourStage = 0;
    public Behavioural[] behaviourals;
    public new Rigidbody rigidbody;
    public MeshRenderer meshrenderer;

    // Start is called before the first frame update
    public void Start()
    {
        BallPit.balls.Add(this);
        //this.scale /= BallPit.scale;

        this.behaviourals = new Behavioural[]{
            new MemoryStack(this, BallPit.rand.Next(1, 64), BallPit.rand.Next(1, 8), 1),
            new MemoryStack(this, BallPit.rand.Next(1, 4), BallPit.rand.Next(1, 16), 2),
            //new SelfDigester(this, 0.00000000001f),
            new Vocalizer(this, 16, 0.1f),
            new Accelerometer(this, 10f),
            new AgeKnower(this, 0.0001f),
            new ChargeManipulator(this, 0.00001f),
            new Colorizer(this, 0.0001f, 0.1f),
            new FatigueKnower(this, 16),
            new MassKnower(this, 1f),
            new SelfSurgeon(this),
            new SelfMutator(this, 16),
            new TorqueEngine(this, 36000),
            new Wiggler(this, 15000f, 2),
            new Ears(this, 16, 100),
            new FreeEnergy(this, 0.01f),
            new EnergyExchange(this, 6, 1f)
        };
        this.behaviourals = Utils.ShuffleArray(this.behaviourals);

        (int isize, int osize) = this.IOSize();

        int hsizeMax = BallPit.rand.Next(2, 1024);
        LayerGroup brainStructure = new Layer(isize) + Neurons.GenerateHiddenLayers(hsizeMax, BallPit.rand.Next(1, hsizeMax)) + new Layer(osize);

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
                this.neurons.UpdateNeurons(2);
                this.UpdateBehaviour();
            }

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
        private void UpdateBehaviour()
        {
            this.UpdateBehaviour(this.behaviourStage);
            this.behaviourStage = (this.behaviourStage + 1) % this.behaviourals.Length;
        }
        private void UpdateBehaviour(int index)
        {
            int si = 0;
            int so = 0;
            for(int i = 0, l = index; i < l; i++)
            {
                Behavioural prev = behaviourals[i];
                si += prev.inputLayerNodes;
                so += prev.outputLayerNodes;
            }

            (int isize, int osize) = this.IOSize();
            Behavioural behavioural = behaviourals[index];
            if(!behavioural.UpdateNeurons(this.neurons, si, so - osize))
            {
                this.Kill("Failed to update behavioural: " + behavioural.ToString());
            }
        }
    #endregion
    
    #region neurons
        public Neurons neurons;
    #endregion

    #region life
        public void Kill(string message)
        {
            this.Kill();
            throw new Exception(message);
        }
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
                if(!Single.IsNaN(value))
                {
                    this._fatigue = Mathf.Max(0, value);
                    this.fatigueCoefficient = Mathf.Exp(-value/1000);
                }
            }
        }
        public void AddFatigue(float energy)
        {
            this.fatigue += energy;
        }
        public float energy
        {
            get
            {
                return - this.fatigue;
            }
            set
            {
                float fatigue = this.fatigue - value;
                if(fatigue < 0)
                {
                    this.mass -= fatigue/Utils.speedOfLightSquared;
                    fatigue = 0;
                }
                this.fatigue = fatigue;
            }
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
                    if(Utils.IsFinite(value))
                    {
                        this.transform.position = value;
                    }
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
                    if(Utils.IsFinite(value))
                    {
                        this.rigidbody.velocity = value;
                    }
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
                    
                    Vector3 scale = this.scale;
                    if(Utils.IsNaN(scale))
                    {
                        this.Kill();
                        return;
                    }
                    //UPDATE UNITY GAMEOBJECT
                    this.transform.localScale = scale;
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
