using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NN;

public class MoveToPoint : MonoBehaviour {

    public Vector3 target;
    public float speed;
    public float forceScale;
    public NeuralNet TestNetwork;
    // Use this for initialization
    void Start()
    {
        TestNetwork = new NeuralNet(4, 4, 10, 10);
        //TestNetwork.InitializeNetwork();

    }

    // Update is called once per frame
    void Update()
    {

        Vector3 C = target - transform.position;
        if (C.magnitude > 1.0f) 
        {
            C.Normalize();
            //transform.position += (C * 10 * Time.deltaTime);
            //rigidbody.AddForce(C * forceScale);
            //transform.LookAt(target);
        }
        
    }

    MoveToPoint() 
    {
        

    }

    void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.up);
        List<List<double>> NNinputs = new List<List<double>>();
        List<double> NNOutputs = new List<double>();
        List <List<double>> NNExpectedOut = new List<List<double>>();
        NNExpectedOut.Add(new List<double>());
        NNinputs.Add(new List<double>());
        NNExpectedOut[0].Add(100.0f);
        NNExpectedOut[0].Add(600.0f);
        NNExpectedOut[0].Add(14.0f);
        NNExpectedOut[0].Add(406.0f);
        NNinputs[0].Add(transform.position.x);
        NNinputs[0].Add(transform.position.y);
        NNinputs[0].Add(transform.position.z);
        NNinputs[0].Add(transform.position.x);
        

        TestNetwork.NetworkTrainingEpoch(NNinputs,NNExpectedOut);
        NNOutputs = TestNetwork.UpdateNN(NNinputs[0]);
        //Debug.Log("NN Output 1" + NNOutputs[0]);
        //Debug.Log("NN Output 1" + NNOutputs[1]);
        //Debug.Log("NN Output 1" + NNOutputs[2]);
        //Debug.Log("NN Output 1" + NNOutputs[3]);

    }

//   void FixedUpdate() 
  //  {
/*
        Vector3 topRight = new Vector3(5, 0, 5);
        Vector3 topLeft = new Vector3(-5, 0, 5);
        Vector3 bottomRight = new Vector3(-5, 0, -5);
        Vector3 bottomLeft = new Vector3(5, 0, -5);
        rigidbody.AddForceAtPosition(this.transform.up * forceScale * Time.deltaTime, (transform.TransformPoint(topRight)));
        rigidbody.AddForceAtPosition(this.transform.up * forceScale * Time.deltaTime, (transform.TransformPoint(bottomRight)));
        rigidbody.AddForceAtPosition(this.transform.up * forceScale * Time.deltaTime, (transform.TransformPoint(topLeft)));
        rigidbody.AddForceAtPosition(this.transform.up * forceScale * Time.deltaTime, (transform.TransformPoint(bottomLeft)));
    
    }*/

}
