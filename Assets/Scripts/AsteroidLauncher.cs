using UnityEngine;
using System.Collections;

public class AsteroidLauncher : MonoBehaviour 
{
    public GameObject AsteroidPrefab;
	public bool InstantlyScanUserLaunchedAsteroids = true;
	public Radar Radar;

    /// <summary>
    /// The asteroid spawn interval in seconds. 
    /// </summary>
	public float SpawnInterval = 2f;

    /// <summary>
    /// The radius of the circle on which asteroids are spawned.
    /// </summary>
	public float SpawnCircleRadius = 50f;

    /// <summary>
    /// The negative and positive range for the spawn direction angle used for non-user spawned asteroids. 
    /// </summary>
	public float SpawnAngleRange = 35f;

    /// <summary>
    /// The maximum velocity for non-user spawned asteroids. Used as a multiplier for user spawned asteroids.
    /// </summary>
	public float LaunchVelocity = 10.0f;

	private Asteroid _draggedAsteroid;

	void Start()
	{
		StartCoroutine(SpawnRandomAsteroids());
	}

	void Update() 
    {
		HandleUserInput();
	}

	private void HandleUserInput()
	{
		// Spawn a new asteroid on mouse down
		if (Input.GetMouseButtonDown(0))
		{
			_draggedAsteroid = SpawnAsteroid(GetMouseScreenPosition());
		}
		
		// Drag the asteroid along as long as mouse button is hold down
		if (Input.GetMouseButton(0) && _draggedAsteroid != null)
		{
			_draggedAsteroid.transform.position = GetMouseScreenPosition();
		}
		
		// Launch the asteroid once the mouse button is released
		if (Input.GetMouseButtonUp(0) && _draggedAsteroid != null)
		{
			// Use the delta mouse movement to get the launch direction and speed for the asteroid
			var launchDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			var launchSpeed = launchDirection.magnitude * LaunchVelocity;

			var success = LaunchAsteroid(_draggedAsteroid, launchDirection.normalized * launchSpeed);

			// If the asteroid was launched successfully and the option is set to scan user
			// launched asteroids instantly, use the radar to scan the asteroid
			if (success && InstantlyScanUserLaunchedAsteroids)
				Radar.ScanAsteroid(_draggedAsteroid);

			_draggedAsteroid = null;
		}
	}

	private Vector2 GetMouseScreenPosition()
	{
		var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		position.z = 0f;

		return position;
	}

	private IEnumerator SpawnRandomAsteroids()
	{
		while (true)
		{
			yield return new WaitForSeconds(SpawnInterval);

			var position = RandomLaunchPosition(transform.position, SpawnCircleRadius);
			var asteroid = SpawnAsteroid(position);
			var velocity = RandomLaunchVelocity(asteroid.transform.position, transform.position, LaunchVelocity);

			LaunchAsteroid(asteroid, velocity);
		}
	}

	/// <summary>
	/// Calculates a random position on a circle defined by the given center position and radius.
	/// </summary>
	private Vector2 RandomLaunchPosition(Vector2 center, float radius)
	{
		// Create random angle between 0 to 360 degrees
		var angle = Random.value * 360f;

		Vector2 position;
		position.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
		position.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);

		return position;
	}

	/// <summary>
	/// Calculates a random launch velocity for an asteroid at the given position with the given target.
	/// </summary>
	private Vector2 RandomLaunchVelocity(Vector2 asteroidPosition, Vector2 targetPosition, float maxSpeed)
	{
		var launchDirection = targetPosition - asteroidPosition;
		launchDirection.Normalize();

		var randomSpeed = Random.Range(3f, maxSpeed);
		var randomAngle = Random.Range(-SpawnAngleRange, SpawnAngleRange);
		var randomDirection = Quaternion.Euler(0f, 0f, randomAngle) * launchDirection;

		return randomDirection * randomSpeed;
	}

	private Asteroid SpawnAsteroid(Vector2 position)
	{
		var gameObj = GameObject.Instantiate(AsteroidPrefab, position, Quaternion.identity) as GameObject;

		// Disable collider so that a dragged asteroid won't collide with other asteroids
		gameObj.collider2D.enabled = false;

		return gameObj.GetComponent<Asteroid>();
	}

	/// <summary>
	/// Launches the asteroid by setting the given velocity for its rigidbody, and returns true if the velocity
	/// is high enough for the launch to succeeded. Returns false if the launch failed and the asteroid was destroyed.
	/// </summary>
	private bool LaunchAsteroid(Asteroid asteroid, Vector2 velocity)
	{
		if (velocity.magnitude > 0f)
		{
			asteroid.rigidbody2D.velocity = velocity;
			asteroid.collider2D.enabled = true;

			return true;
		}

		GameObject.Destroy(asteroid.gameObject);

		return false;
	}
}
