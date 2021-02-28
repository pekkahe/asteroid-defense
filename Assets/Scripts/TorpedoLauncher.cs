using UnityEngine;
using System.Collections;

public class TorpedoLauncher : MonoBehaviour
{
	public GameObject TorpedoPrefab;
	public GameObject ImpactIndicatorPrefab;
	
    /// <summary>
    /// The magnitude of the torpedo's velocity.
    /// </summary>
    public float TorpedoSpeed = 15f;

    /// <summary>
    /// The range of the space station's safety zone. Asteroids entering this area are shot down. 
    /// </summary>
	public float SafetyZoneRange = 10f;

	/// <summary>
	/// Returns true if the asteroid is bound to cross the safety zone, false otherwise.
	/// </summary>
	public bool DoesTrajectoryEnterSafetyZone(Asteroid asteroid)
	{
		return DoesTrajectoryEnterSafetyZone(asteroid.transform.position, asteroid.rigidbody2D.velocity);
	}

	/// <summary>
	/// Returns true if the object is bound to cross the safety zone, false otherwise.
	/// </summary>
	public bool DoesTrajectoryEnterSafetyZone(Vector3 objectPosition, Vector3 objectVelocity)
	{
		// Calculate the angle between the object trajectory and space station
		var trajectoryAngle = CalculateTrajectoryAngle(objectPosition, objectVelocity);

		// If the trajectory angle is over 90 degrees, we can already determine that the object
		// will not cross the safety zone, since it's been launched at the opposite direction
		if (trajectoryAngle > 90f)
			return false;

		// Now, calculate the distance at which the object bypasses the space station,
		// if it continues to move on its trajectory: tan(θ) = opposite / adjacent
		var adjacent = (transform.position - objectPosition).magnitude;
		var opposite = adjacent * Mathf.Tan(trajectoryAngle * Mathf.Deg2Rad);

		// If the opposite side, or bypass distance, is smaller than the designated safety zone range,
		// the trajectory is within the safety zone
		return opposite < SafetyZoneRange;

		// Note that, if the object is launched at a near distance to the safety zone with a wide
		// enough trajectory angle, it might traverse inside the safety zone without getting fired at.
		// This is because the bypass distance is still higher than the safety zone range,
		// regardless if the object moves a short distance inside the safety zone.
	}

	/// <summary>
	/// Fires a torpedo towards the asteroid with a velocity that hits the asteroid on its fly path.
	/// </summary>
	public void FireTorpedo(Asteroid asteroid)
	{
		var torpedoVelocity = CalculateTorpedoVelocity(asteroid.transform.position, asteroid.rigidbody2D.velocity);

		FireTorpedo(torpedoVelocity);
	}

	/// <summary>
	/// Fires a torpedo with the given velocity.
	/// </summary>
	public void FireTorpedo(Vector3 velocity)
	{
		if (velocity.sqrMagnitude > 0f)
		{
			var gameObj = GameObject.Instantiate(TorpedoPrefab, transform.position, Quaternion.identity) as GameObject;
			gameObj.rigidbody2D.velocity = velocity;
		}
	}

	/// <summary>
	/// Calculates the velocity at which the torpedo should intercept the target on its trajectory.
	/// </summary>
	public Vector3 CalculateTorpedoVelocity(Vector3 targetPosition, Vector3 targetVelocity)
	{
		// First calculate the time to impact, so we can determinate the impact position
		var t = CalculateTimeToImpact(targetPosition, targetVelocity);
		if (t <= 0f)
			return Vector3.zero;
		
		var impactPosition = targetPosition + targetVelocity * t;

		IndicateImpact(impactPosition);
		
		// Now we can calculate the torpedo direction
		var torpedoDirection = impactPosition - transform.position;
		torpedoDirection.Normalize();
		
		// Finally, multiply the direction with the speed to form our velocity
		return torpedoDirection * TorpedoSpeed;
	}
	
	/// <summary>
	/// Calculates the angle in degrees between the object trajectory and space station.
	/// </summary>
	private float CalculateTrajectoryAngle(Vector3 objectPosition, Vector3 objectVelocity)
	{
		var distanceToStation = transform.position - objectPosition;
		distanceToStation.Normalize();
		objectVelocity.Normalize();
		
		// Calculate angle with dot product: a·b = |a|×|b|×cos(θ), where
		// 	|a|×|b| = 1 with normalized unit vectors => cos(θ) = a·b
		var cos = Vector3.Dot(objectVelocity, distanceToStation);
		
		return Mathf.Acos(cos) * Mathf.Rad2Deg;
	}

	/// <summary>
	/// Calculates the time it takes for the torpedo and object to impact, assuming that they both
	/// traverse at a constant velocity and direction. Returns zero if the torpedo is slower than
	/// the object, and no impact is possible.
	/// </summary>
	private float CalculateTimeToImpact(Vector3 objectPosition, Vector3 objectVelocity)
	{
		// The torpedo (B), incoming object (C) and interception (A) points form an arbitrary triange
		// on which we can use the law of cosines (http://en.wikipedia.org/wiki/Law_of_cosines):
		// c^2 = a^2 + b^2 - 2abcosC, where
		// a = distance between object and torpedo (CB)
		// b = distance between object and interception point (CA)
		// c = distance between torpedo and interception point (BA)
		// C = angle between object trajectory and torpedo position

		// We can immediately calculate a and C ...
		var a = (transform.position - objectPosition).magnitude;
		var C = CalculateTrajectoryAngle(objectPosition, objectVelocity);

		// ... and we can express the remaining b and c distances based on time (x = vt): 
		// b = va*t
		// c = vm*t
		var va = objectVelocity.magnitude;
		var vm = TorpedoSpeed;

		// This leaves us with only one unknown variable, t, and reduces the law of cosines
		// formula into a quadratic equation: (va^2 - vm^2)t^2 - (2a*va*cosC)t + a^2 = 0

		// Let's assign the values we already know:
		// (X)t^2 + (-Y)t + (Z) = 0
		var X = (va * va) - (vm * vm);
		var Y = -(2 * a * va * Mathf.Cos(C * Mathf.Deg2Rad));
		var Z = a * a;
		
		// Now we can solve t using the quadratic formula (http://en.wikipedia.org/wiki/Quadratic_formula):
		// t = (-Y + sqrt(Y^2 - 4XY)) / 2X or
		// t = (-Y - sqrt(Y^2 - 4XY)) / 2X
		var D = (Y * Y) - (4 * X * Z);
		var p = -Y / (2 * X);
		var q = Mathf.Sqrt(D) / (2 * X);
		
		var t1 = p + q;
		var t2 = p - q;
		
		// If both values of t are below zero, there's no solution; this essentially means that the torpedo
		// is slower than the incoming object and would never reach it.
		if (t1 <= 0f && t2 <= 0f)
		{
			Debug.LogWarning("No solution: the torpedo is slower than the incoming object and would never reach it.");
			return 0f;
		}
		
		// Return the time which is faster
		return (t1 > 0f && t1 < t2) ? t1 : t2;
	}

	/// <summary>
	/// Indicates the position of the missile impact by creating a self-destructing animated game object.
	/// </summary>
	private void IndicateImpact(Vector2 impactPosition)
	{
		var gameObj = GameObject.Instantiate(ImpactIndicatorPrefab, impactPosition, Quaternion.identity) as GameObject;
		GameObject.Destroy(gameObj, 3f);
	}
}