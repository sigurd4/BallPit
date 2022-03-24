using System;
using System.Text; 
using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    private readonly LayerGroup<NonPopulatedLayer> hiddenLayers;
    public long age = 0;
    public long killCount = 0;
    public float fatigueCoefficient = 1;
    public float charge = 0;
    public readonly float density = 0.001f;
    public float[] voice = new float[0];
    private readonly float[] inputLayer;
    private readonly LayerGroup<PopulatedLayer> brainStructure;
    public readonly Neurons neurons;

    public readonly Behavioural[] behaviourals;
    public new Rigidbody rigidbody;
    public MeshRenderer meshrenderer;

    // Start is called before the first frame update
/*
    public Ball()
    {
        BallPit.balls.Add(this);
        //this.scale /= BallPit.scale;

        this.behaviourals = new Behavioural[]{
            new MemoryStack(this, 4, 8),
            new SelfDigester(this),
            new Vocalizer(this, 16, 0.1f),
            new Accelerometer(this, 10f),
            new AgeKnower(this, 0.0001f),
            //new ChargeManipulator(this, 10000000000000000000f),
            new Colorizer(this),
            new FatigueKnower(this, 16),
            //new MassKnower(this, 1f),
            new SelfSurgeon(this),
            new SelfMutator(this, 16),
            new TorqueEngine(this, 3600),
            //new Wiggler(this, 1500f, 6),
            //new Ears(this, 16, 100)
        };
        
        int isize = 0, osize = 0;
        for(int i = 0, l = this.behaviourals.Length; i < l; i++)
        {
            isize += this.behaviourals[i].inputLayerNodes;
            osize += this.behaviourals[i].outputLayerNodes;
        }
        isize = Math.Max(1, isize);
        osize = Math.Max(1, osize);

        this.inputLayer = new float[isize];
        Layer.Lin outputLayer = new Layer.Lin(osize);

        this.hiddenLayers = this.GenerateHiddenLayers(16);

        this.brainStructure = new Layer.Input(this.inputLayer) + (this.hiddenLayers + outputLayer);
        this.neurons = new Neurons(this.brainStructure);
    }

    public LayerGroup<NonPopulatedLayer> GenerateHiddenLayers(int nodeCount)
    {
        List<NonPopulatedLayer> g = new List<NonPopulatedLayer>();
        for(int n = 0; n < nodeCount;)
        {
            int size = BallPit.rand.Next((nodeCount - n)/2, nodeCount - n);
            if(size > 0)
            {
                n += size;
                switch(BallPit.rand.Next(0, 5))
                {
                    case 0: g.Add(new Layer.Lin(size)); break;
                    case 4: g.Add(new Layer.ReLU(size, 0.1f)); break;
                    case 2: g.Add(new Layer.Sigmoid(size)); break;
                    case 3: g.Add(new Layer.Tanh(size)); break;
                }
            }
        }
        g.Sort((NonPopulatedLayer a, NonPopulatedLayer b) => {
            return 1;
        });
        return g.ToArray();
    }
*/

    public void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
        this.meshrenderer = this.GetComponent<MeshRenderer>();
        //UpdateScale();
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void FixedUpdate()
    {
        /*if(this.transform.position.y < -10 || this.scale < 0.001f)
        {
            this.Kill();
        }
        else
        {
            this.UpdateNeurons();

            this.age++;
        }*/
    }
    
    #region neurons
        private void UpdateNeurons()
        {
            if(this.neurons == null) this.Kill();
            float[] outputLayer = this.neurons.GetOutputLayerValues();

            int si = 0;
            int so = 0;
            for(int i = 0, l = this.behaviourals.Length; i < l; i++)
            {
                Behavioural b = behaviourals[i];
                b.UpdateNeurons(this.inputLayer, si, outputLayer, so);
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
                this.fatigue = Mathf.Max(0, value);
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
}
