using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : MonoBehaviour
{
	public GameObject ExplosionPrefab;
	public GameObject SmokepuffsPrefab;

    /// <summary>
    /// Whether this asteroid has been drawn inside the viewport or not. 
    /// </summary>
	public bool IsVisible;

    /// <summary>
    /// Whether this asteroid has been scanned or not. 
    /// </summary>
	public bool IsScanned;

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Torpedo"))
		{
			CreateExplosion();

			// If torpedo hit asteroid, delete both asteroid and torpedo
			GameObject.Destroy(collision.gameObject);
			GameObject.Destroy(this.gameObject);
		}
		else if (collision.gameObject.CompareTag("SpaceStation"))
		{
			CreateSmokepuffs();

			GameObject.Destroy(this.gameObject);
		}
		else if (collision.gameObject.CompareTag("Asteroid"))
		{
			// If the asteroid collides with another asteroid, reset the scanner flag
			// since the asteroid could have changed its trajectory towards the station
			IsScanned = false;
		}
	}

	private void CreateExplosion()
	{
		var gameObj = GameObject.Instantiate(ExplosionPrefab, transform.position, Quaternion.identity) as GameObject;
		GameObject.Destroy(gameObj, 2f);
	}

	private void CreateSmokepuffs()
	{
		var gameObj = GameObject.Instantiate(SmokepuffsPrefab, transform.position, Quaternion.identity) as GameObject;
		GameObject.Destroy(gameObj, 2f);
	}

	void OnBecameVisible() 
	{
		IsVisible = true;
	}
}
