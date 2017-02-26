using UnityEngine;
using System.Collections;

public class RotateSample : MonoBehaviour
{	
	void Start(){
		iTween2.RotateBy(gameObject, iTween2.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
	}
}

