using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class DrawTrajectory : MonoBehaviour 
{
	public Color Color;

	void Start()
	{ 
		if (!Debug.isDebugBuild)
			enabled = false;
	}
	
	void Update()
	{
		Debug.DrawRay(transform.position, rigidbody2D.velocity.normalized * 100f, Color);
	}
}
