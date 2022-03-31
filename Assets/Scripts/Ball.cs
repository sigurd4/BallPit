using System;
using System.Text; 
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    public static readonly float minSize = 0.001f;
    public static readonly float minVolume = Utils.spherePerCubeVolume*Ball.minSize*Ball.minSize*Ball.minSize;
    public long age = 0;
    public long killCount = 0;
    public float fatigueCoefficient = 1;
    public float[] charge = new float[0];
    public float density = 1.0f;

    private int behaviourStage = 0;
    public Behavioural[] behaviourals;
    public new Rigidbody rigidbody;
    public MeshRenderer meshrenderer;
    public AudioSource audioSource;
    public Ball parent;

    // Start is called before the first frame update
    public void Start()
    {
        this.meshrenderer = this.GetComponent<MeshRenderer>();
        this.audioSource = this.GetComponent<AudioSource>();

        //this.meshrenderer.enabled = false;

        BallPit.balls.Add(this);
        this.diameter = 1;

        this.rigidbody = this.GetComponent<Rigidbody>();
        this.radius = this.transform.localScale[0];
        
        var dummy = AudioClip.Create ("dummy", 1, 1, AudioSettings.outputSampleRate, false);

        dummy.SetData(new float[] { 0 }, 0);
        this.audioSource.clip = dummy; //just to let unity play the audiosource
        this.audioSource.loop = true;
        this.audioSource.spatialBlend = 1;
        this.audioSource.Play();
    }

    public void Init(Ball ball)
    {
        int count = ball.behaviourals.Length;
        Behavioural[] behaviourals = new Behavioural[count];

        for(int i = 0; i < count; i++)
        {
            behaviourals[i] = ball.behaviourals[i].Clone(this);
        }

        this.Init(behaviourals, ball.neurons.Clone());
    }
    public void Init()
    {
        int vocalChannels = 8;

        Behavioural[] behaviourals = new Behavioural[]{
            new GravityManipulator(this, Utils.GetRandomPositiveScalar(BallPit.rand)*5),
            new MemoryStack(this, BallPit.rand.Next(1, 32), BallPit.rand.Next(1, 8), 1),
            new MemoryStack(this, BallPit.rand.Next(1, 4), BallPit.rand.Next(1, 16), 4),
            new NoiseGenerator(this, BallPit.rand.Next(0, 32)),
            new SelfDigester(this, 1f/((float)BallPit.rand.Next(1, 10000000))),
            new Vocalizer(this, BallPit.rand.Next(0, vocalChannels), Utils.GetRandomPositiveScalar(BallPit.rand)*5f),
            new Accelerometer(this),
            new AgeKnower(this),
            new ChargeManipulator(this, BallPit.rand.Next(0, 12), 0.0001f),
            new Colorizer(this, 0.8f, 0.1f),
            new FatigueKnower(this, 16),
            new MassKnower(this),
            new SelfSurgeon(this),
            new SelfMutator(this, BallPit.rand.Next(0, 32)),
            new TorqueEngine(this, 3600000f),
            new Wiggler(this, Utils.GetRandomPositiveScalar(BallPit.rand)*300f),
            new Ears(this, BallPit.rand.Next(0, vocalChannels), Utils.GetRandomPositiveScalar(BallPit.rand)*100),
            new FreeEnergy(this, 1f),
            //new EnergyExchange(this, BallPit.rand.Next(0, 32), 0.000000000000000001f/BallPit.rand.Next(1, Int32.MaxValue)),
            new Splitter(this, Utils.GetRandomPositiveScalar(BallPit.rand)*10000000000f, 0.1f*Utils.GetRandomPositiveScalar(BallPit.rand))
        };
        
        (int isize, int osize) = Ball.IOSize(behaviourals);

        int hsizeMax = BallPit.rand.Next(0, 64);
        LayerGroup hiddenLayers = hsizeMax == 0 ? new LayerGroup() : Neurons.GenerateHiddenLayers(hsizeMax, BallPit.rand.Next(1, hsizeMax));
        LayerGroup brainStructure = new Layer(isize) + hiddenLayers + new Layer(osize);

        this.Init(behaviourals, brainStructure.GenerateNeurons());
    }

    public void Init(Behavioural[] behaviourals, Neurons neurons)
    {
        this.behaviourals = behaviourals;
        this.behaviourals = Utils.ShuffleArray(this.behaviourals);

        this.neurons = neurons;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            GameObject go = contact.otherCollider.gameObject;
            Ball other = go.GetComponent<Ball>();
            if(other != null)
            {
                //DISTRIBUTE CHARGES
                /*float chargeDensity = (this.charge + other.charge)/(this.mass + other.mass);
                this.charge = chargeDensity*this.mass;
                other.charge = chargeDensity*other.mass;*/

                //EAT
                if(other.radius < this.radius*0.8f)
                {
                    this.mass = this.mass + other.mass*0.99f;
                    this.inertia += other.inertia*0.99f;
                    this.momentum += other.momentum*0.99f;
                    other.Kill("Eaten");
                }
            }
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void FixedUpdate()
    {
        if(this.dead || this.transform.position.y < -50 || this.transform.position.magnitude > 1000)
        {
            BallPit.balls.Remove(this);
            UnityEngine.Object.Destroy(this.gameObject);
        }
        else
        {
            if(neurons != null)
            {
                this.neurons.UpdateNeurons(12);
                this.UpdateBehaviour();
            }

            this.age++;
        }
    }

    #region audio
    public float[] voice = new float[0];
    //private float[] voicePrev = new float[0];
    private ulong audioTimeIndex = 0;

    public void ResetAudioTimeIndex()
    {
        this.audioTimeIndex = 0;
    }
    private readonly int sampleRate = 32000;
    void OnAudioFilterRead(float[] data, int channels)
    {
        int N = this.voice.Length;

        if(N != 0)
        {
            /*if(N != voicePrev.Length)
            {
                voicePrev = this.voice;
            }*/
            float fundamental = BallPit.unitFrequency/this.diameter;
            float interval = 2f;
            uint cycleLength = (uint)(1000*this.sampleRate/fundamental);

            for(int i = 0, l = data.Length; i < l; i += channels)
            {
                float fade1 = (float)i/l;
                float fade0 = 1f - fade1;

                float x = 0f;
                float f = fundamental;
                for(int n = 0; n < N; n++)
                {
                    if(f >= 20 && f <= 16000)
                    {
                        float a = this.voice[n];//*fade1 + this.voicePrev[n]*fade0;
                        x += a*Mathf.Sin(Utils.radian*i*f/sampleRate);
                    }
                    f *= interval;
                }
            
                this.audioTimeIndex++;
            
                //if timeIndex gets too big, reset it to 0
                if((uint)this.audioTimeIndex == cycleLength)
                {
                    this.audioTimeIndex = 0;
                }

                x *= BallPit.audioVolume;

                for(int c = 0; c < channels; c++)
                {
                    data[i + c] = x;
                }
            }
        }
    }
    #endregion

    #region gameobjects
        public Ball Duplicate()
        {
            GameObject go = BallPit.ballpit.NewBall();
            if(go != null)
            {
                Ball ball = go.GetComponent<Ball>();
                if(ball != null)
                {
                    ball.Init(this);
                    return ball;
                }
            }
            return null;
        }
    #endregion

    #region behaviourals
        private static (int, int) IOSize(Behavioural[] behaviourals)
        {
            int isize = 0, osize = 0;
            for(int i = 0, l = behaviourals.Length; i < l; i++)
            {
                isize += behaviourals[i].inputLayerNodes;
                osize += behaviourals[i].outputLayerNodes;
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

            (int isize, int osize) = Ball.IOSize(this.behaviourals);
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
        private bool dead = false;
        public void Kill(string message)
        {
            this.dead = true;
            if(this.audioSource != null)
            {
                this.audioSource.Stop();
            }
            //Debug.Log(message);
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
                    //this.mass += -fatigue/Utils.speedOfLightSquared;
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
                if(value <= 0)
                {
                    this.Kill("Negative mass");
                }
                this.volume = value/this.density;
            }
        }
        public Vector3 inertia
        {
            get
            {
                return this.rigidbody.inertiaTensor;
            }
            set
            {
                this.rigidbody.inertiaTensor = value;
            }
        }
        public Vector3 momentum
        {
            get
            {
                return this.rigidbody.velocity*this.mass;
            }
            set
            {
                this.rigidbody.velocity = value/this.mass;
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
            public float _radius;
            public float radius
            {
                get
                {
                    return this._radius;
                }
                set
                {
                    if(Single.IsNaN(value))
                    {
                        this.Kill("NaN radius");
                        return;
                    }
                    if(value < 0.001f)
                    {
                        this.Kill("Too small radius");
                        return;
                    }
                    this._radius = value;
                    
                    Vector3 scale = this.scale;
                    if(Utils.IsNaN(scale))
                    {
                        this.Kill("NaN scale");
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
                    if(value <= 0)
                    {
                        this.Kill("Negative volume");
                    }
                    float r = Utils.Cbrt(value/Utils.spherePerCubeVolume);
                    if(Single.IsNaN(r))
                    {
                        r = 0;
                    }
                    this.radius = r;
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
                    return Vector3.one*(this.diameter);
                }
            }
        #endregion
    #endregion
}
