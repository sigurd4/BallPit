using UnityEngine;
using System;
public class Accelerometer : Behavioural
{
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;
    public Accelerometer(Ball ball) : base(ball, 21, 18)
    {        
        this.lastVelocity = Vector3.zero;
        this.lastAngularVelocity = Vector3.zero;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        Vector3 sampleDir1 = new Vector3(Neurons.Lin(outputLayer[0]), Neurons.Lin(outputLayer[1]), Neurons.Lin(outputLayer[2]));
        Vector3 sampleDir2 = new Vector3(Neurons.Lin(outputLayer[3]), Neurons.Lin(outputLayer[4]), Neurons.Lin(outputLayer[5]));
        Vector3 sampleDir3 = new Vector3(Neurons.Lin(outputLayer[6]), Neurons.Lin(outputLayer[7]), Neurons.Lin(outputLayer[8]));

        Vector3 velocity = Quaternion.Inverse(this.ball.transform.rotation)*this.ball.GetComponent<Rigidbody>().velocity;
        Vector3 acceleration = (velocity - this.lastVelocity)/Time.deltaTime;
        this.lastVelocity = velocity;

        float dist1 = Neurons.Sigmoid(outputLayer[9]);
        float dist2 = Neurons.Sigmoid(outputLayer[10]);
        float dist3 = Neurons.Sigmoid(outputLayer[11]);

        inputLayer[0] = Neurons.Tanh(Mathf.Pow(acceleration.x, dist1));
        inputLayer[1] = Neurons.Tanh(Mathf.Pow(acceleration.y, dist1));
        inputLayer[2] = Neurons.Tanh(Mathf.Pow(acceleration.z, dist1));
        inputLayer[3] = Neurons.Tanh(Mathf.Pow(acceleration.sqrMagnitude, dist2));
        inputLayer[4] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(acceleration, sampleDir1), dist3));
        inputLayer[5] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(acceleration, sampleDir2), dist3));
        inputLayer[6] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(acceleration, sampleDir3), dist3));

        Vector3 accelerationAngle = new Vector3(Vector3.Angle(acceleration, Vector3.up), Vector3.Angle(acceleration, Vector3.right), Vector3.Angle(acceleration, Vector3.forward))/180f;

        float dist4 = Neurons.Sigmoid(outputLayer[12]);
        float dist5 = Neurons.Sigmoid(outputLayer[13]);
        float dist6 = Neurons.Sigmoid(outputLayer[14]);

        inputLayer[7] = Neurons.Tanh(Mathf.Pow(accelerationAngle.x, dist4));
        inputLayer[8] = Neurons.Tanh(Mathf.Pow(accelerationAngle.y, dist4));
        inputLayer[9] = Neurons.Tanh(Mathf.Pow(accelerationAngle.z, dist4));
        inputLayer[10] = Neurons.Tanh(Mathf.Pow(accelerationAngle.sqrMagnitude, dist5));
        inputLayer[11] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(accelerationAngle, sampleDir1), dist6));
        inputLayer[12] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(accelerationAngle, sampleDir2), dist6));
        inputLayer[13] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(accelerationAngle, sampleDir3), dist6));

        Vector3 angularVelocity = this.ball.GetComponent<Rigidbody>().angularVelocity;
        Vector3 angularAcceleration = (angularVelocity - this.lastAngularVelocity)/Time.deltaTime;
        this.lastAngularVelocity = angularVelocity;

        float dist7 = Neurons.Sigmoid(outputLayer[15]);
        float dist8 = Neurons.Sigmoid(outputLayer[16]);
        float dist9 = Neurons.Sigmoid(outputLayer[17]);

        inputLayer[14] = Neurons.Tanh(Mathf.Pow(angularAcceleration.x, dist7));
        inputLayer[15] = Neurons.Tanh(Mathf.Pow(angularAcceleration.y, dist7));
        inputLayer[16] = Neurons.Tanh(Mathf.Pow(angularAcceleration.z, dist7));
        inputLayer[17] = Neurons.Tanh(Mathf.Pow(angularAcceleration.sqrMagnitude, dist8));
        inputLayer[18] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(angularAcceleration, sampleDir1), dist9));
        inputLayer[19] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(angularAcceleration, sampleDir2), dist9));
        inputLayer[20] = Neurons.Tanh(Mathf.Pow(Vector3.Dot(angularAcceleration, sampleDir3), dist9));
    }

    public override Behavioural Clone(Ball ball)
    {
        return new Accelerometer(ball);
    }
}