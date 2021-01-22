using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Exploder : NetworkBehaviour {

	public GameObject explosion;
	[SyncVar] float explosionDelay;
	[SyncVar] float explosionSize;
	[SyncVar] public float timer;
	Detonator det;
	GameObject spawnedExplosion;

	// Use this for initialization
	void Start () {	

		/*spawnedExplosion = Instantiate (explosion, transform.position, Quaternion.identity);
		det = spawnedExplosion.GetComponent<Detonator> ();

		if (isServer) {
			NetworkServer.Spawn (spawnedExplosion);
		}

		timer = 0;*/
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		//det.size = explosionSize;
		if (timer >= explosionDelay) {

			spawnedExplosion = Instantiate (explosion, transform.position, Quaternion.identity);
			det = spawnedExplosion.GetComponent<Detonator> ();
			det.size = explosionSize;			
			det.Explode ();
			Destroy (gameObject);
		}
	}

	public void Init(float delay, float size) {
		explosionDelay = delay;
		explosionSize = size;
	}
}
