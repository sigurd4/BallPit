using UnityEngine;
using System;
public class Accelerometer : Behavioural
{
    private Vector3 lastVelocity;
    private Vector3 lastAngularVelocity;
    private readonly float scale;
    public Accelerometer(Ball ball, float scale) : base(ball, 20, 9)
    {        
        this.lastVelocity = Vector3.zero;
        this.lastAngularVelocity = Vector3.zero;
        this.scale = scale;
    }

    protected override void UpdateNeurons(float[] inputLayer, float[] outputLayer)
    {
        Vector3 sampleDir1 = new Vector3(Neurons.Lin(outputLayer[0]), Neurons.Lin(outputLayer[1]), Neurons.Lin(outputLayer[2]));
        Vector3 sampleDir2 = new Vector3(Neurons.Lin(outputLayer[3]), Neurons.Lin(outputLayer[4]), Neurons.Lin(outputLayer[5]));
        Vector3 sampleDir3 = new Vector3(Neurons.Lin(outputLayer[6]), Neurons.Lin(outputLayer[7]), Neurons.Lin(outputLayer[8]));

        Vector3 velocity = Quaternion.Inverse(this.ball.transform.rotation)*this.ball.rigidbody.velocity;
        Vector3 acceleration = (velocity - this.lastVelocity)*this.scale;
        this.lastVelocity = velocity;

        inputLayer[0] = Neurons.Tanh(acceleration.x);
        inputLayer[1] = Neurons.Tanh(acceleration.y);
        inputLayer[2] = Neurons.Tanh(acceleration.z);
        inputLayer[3] = Neurons.Tanh(acceleration.sqrMagnitude);
        inputLayer[4] = Neurons.Tanh(Vector3.Dot(acceleration, sampleDir1));
        inputLayer[5] = Neurons.Tanh(Vector3.Dot(acceleration, sampleDir2));
        inputLayer[6] = Neurons.Tanh(Vector3.Dot(acceleration, sampleDir3));

        Vector3 accelerationAngle = new Vector3(Vector3.Angle(acceleration, Vector3.up), Vector3.Angle(acceleration, Vector3.right), Vector3.Angle(acceleration, Vector3.forward))/180f;

        inputLayer[7] = Neurons.Tanh(accelerationAngle.x);
        inputLayer[8] = Neurons.Tanh(accelerationAngle.y);
        inputLayer[9] = Neurons.Tanh(accelerationAngle.z);
        inputLayer[10] = Neurons.Tanh(Vector3.Dot(accelerationAngle, sampleDir1));
        inputLayer[11] = Neurons.Tanh(Vector3.Dot(accelerationAngle, sampleDir2));
        inputLayer[12] = Neurons.Tanh(Vector3.Dot(accelerationAngle, sampleDir3));

        Vector3 angularVelocity = this.ball.rigidbody.angularVelocity;
        Vector3 angularAcceleration = (angularVelocity - this.lastAngularVelocity)*this.scale;
        this.lastAngularVelocity = angularVelocity;

        inputLayer[13] = Neurons.Tanh(angularAcceleration.x);
        inputLayer[14] = Neurons.Tanh(angularAcceleration.y);
        inputLayer[15] = Neurons.Tanh(angularAcceleration.z);
        inputLayer[16] = Neurons.Tanh(angularAcceleration.sqrMagnitude);
        inputLayer[17] = Neurons.Tanh(Vector3.Dot(angularAcceleration, sampleDir1));
        inputLayer[18] = Neurons.Tanh(Vector3.Dot(angularAcceleration, sampleDir2));
        inputLayer[19] = Neurons.Tanh(Vector3.Dot(angularAcceleration, sampleDir3));
    }
}