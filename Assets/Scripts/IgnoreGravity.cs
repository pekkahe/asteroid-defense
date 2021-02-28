using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class IgnoreGravity : MonoBehaviour 
{
	void Awake()
	{
		// Set the gravity scale to zero so that gravity has no effect on this object.
		// Another option would be to set the rigidbody to kinematic, but currently there
		// seems to be a bug in RigidBody2D that causes kinematic rigidbodies to ignore triggers:
		// http://answers.unity3d.com/questions/575438/how-to-make-ontriggerenter2d-work.html
		rigidbody2D.gravityScale = 0f;
		rigidbody2D.isKinematic = false;
	}
}
