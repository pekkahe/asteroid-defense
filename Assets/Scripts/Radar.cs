using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Radar : MonoBehaviour 
{
	public TorpedoLauncher TorpedoLauncher;

    /// <summary>
    /// Asteroid scanning interval in seconds.
    /// </summary>
	public float ScanningInterval = 1f;

    /// <summary>
    /// The radius of the asteroid scanner.
    /// </summary>
	public float ScanningRadius = 35f;

	void Start() 
    {
		StartCoroutine(ScanIncomingAsteroids());
	}

	private IEnumerator ScanIncomingAsteroids()
	{
		while (true)
		{
			yield return new WaitForSeconds(ScanningInterval);

			var asteroids = GetIncomingAsteroids();

			// Scan the first found asteroid on a single coroutine interval
			var asteroid = asteroids.FirstOrDefault();
			if (asteroid != null)
				ScanAsteroid(asteroid);
		}
	}

	/// <summary>
	/// Returns all asteroids inside the scanning radius which have not yet been scanned
	/// and are a possible threat to the space station.
	/// </summary>
	public IEnumerable<Asteroid> GetIncomingAsteroids()
	{
		// Get all colliders on the asteroid layer which currently overlap or are inside the scanning radius
		var asteroidMask = 1 << LayerMask.NameToLayer("Asteroid");
		var asteroidColliders = Physics2D.OverlapCircleAll(transform.position, ScanningRadius, asteroidMask);

		foreach (var collider in asteroidColliders)
		{
			var asteroid = collider.GetComponent<Asteroid>();

			// Only scan asteroids once they are visible to the camera,
			// so that we don't shoot torpedoes outside the viewport
			if (asteroid.IsVisible && !asteroid.IsScanned)
				yield return asteroid;
		}
	}

	/// <summary>
	/// Scans the particular asteroid for its trajectory and fires a missile to intercept it, 
	/// if the asteroid would otherwise enter the safety zone.
	/// </summary>
	public void ScanAsteroid(Asteroid asteroid)
	{
		if (TorpedoLauncher.DoesTrajectoryEnterSafetyZone(asteroid))
		{
			// For convenience, change the asteroid trajectory color to indicate it's within safety zone
			var drawer = asteroid.GetComponent<DrawTrajectory>();
			if (drawer != null)
				drawer.Color = Color.red;

			TorpedoLauncher.FireTorpedo(asteroid);
		}

		asteroid.IsScanned = true;
	}
}
