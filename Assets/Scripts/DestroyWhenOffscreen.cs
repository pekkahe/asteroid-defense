using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class DestroyWhenOffscreen : MonoBehaviour 
{
	private Renderer _renderer;
	private bool _hasBeenRenderedOnce;
	
	void Awake()
	{
		_renderer = renderer;
	}

	void Update()
	{
		if (!_renderer.isVisible && _hasBeenRenderedOnce)
		{
			GameObject.Destroy(gameObject);
		} 
	}

	void OnBecameVisible() 
	{
		_hasBeenRenderedOnce = true;
	}
}
