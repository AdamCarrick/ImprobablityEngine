using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
    public float speed = 1; // Speed we can move at
    public float turnRate = 1; //Rate at which we can turn
    public float hoverHeight = 1; // The height to hover at, Bassed on root, TODO Base this height on the terrain below
    public float hoverForce; //The speed at which the player will fly up untill they reach hover height. Must be greater than gravity to work
    public float correctionRate; //The turn rate the system will apply to leveling the tank out
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate() 
    {
        //Set the movement based on input 
        //These Strings for what Axis are are set up in an input manager somewhere
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3((moveHorizontal * speed * Time.deltaTime),0,(moveVertical * speed * Time.deltaTime));
        //Apply the movemets 
        rigidbody.AddRelativeForce(movement);

        //Set Rotation bassed on mouse input. Using force rather than direct transform.
        float turnX = Input.GetAxis("Mouse X");
        float turnY = Input.GetAxis("Mouse Y");

        Vector3 rotation = new Vector3(turnY, turnX, 0);
        rigidbody.AddRelativeTorque(rotation * turnRate);


        //If below hover height hover harder
        if (rigidbody.position.y < hoverHeight)
        {
            rigidbody.AddForce(0,hoverForce,0);
        }

        //Z Axis should be Yaw as in tilting side to side
        if (transform.rotation.z != 0) 
        {
            //If the object is tilting to one side or the other add force to turn it back
            rigidbody.AddRelativeTorque(0, 0, -rigidbody.rotation.z * correctionRate);
        }
        
       //Y is the axis of turning so dont bother with correcting it
       if (transform.rotation.y != 0)
       {
            //If the object is tilting to one side or the other add force to turn it back
            rigidbody.AddRelativeTorque(0, -rigidbody.rotation.y * 2, 0);
        }

        //X Axis should be Pitch Tilting forward and backwards
        if (transform.rotation.x != 0)
        {
            //If the object is tilting to one side or the other add force to turn it back
            rigidbody.AddRelativeTorque(-rigidbody.rotation.x * correctionRate, 0, 0);
        }
        

    
    }

    private void DrawHelperAtCenter(Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(transform.position, destination);
    }
}
