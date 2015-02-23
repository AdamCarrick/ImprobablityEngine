using UnityEngine;
using System.Collections;
//using Encog;

public class move : MonoBehaviour {

	// Use this for initialization
    Vector3 movement;

	void Start () {
	    movement = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
        movement = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            
            transform.position = transform.position * movement.x;
        }
	
	}
}
