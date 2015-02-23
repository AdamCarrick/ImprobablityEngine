using UnityEngine;
using System.Collections;

public class up : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.up);
    }
}
