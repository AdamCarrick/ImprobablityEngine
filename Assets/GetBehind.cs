using UnityEngine;
using System.Collections;

public class GetBehind : MonoBehaviour {

    public GameObject target;
    Vector3 differnce;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        differnce = this.transform.position - target.transform.position;
        
        Debug.Log(Vector3.Dot(differnce, this.transform.forward));
	}
}
