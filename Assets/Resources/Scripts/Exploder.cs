using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Exploder : NetworkBehaviour {

	public GameObject explosion;
	[SyncVar] float explosionDelay;
	[SyncVar] float explosionSize;
	[SyncVar] float timer;
	Detonator det;
	GameObject spawnedExplosion;

	// Use this for initialization
	void Start () {	
		
		spawnedExplosion = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
		det = spawnedExplosion.GetComponent<Detonator> ();

		if (isServer) {
			NetworkServer.Spawn (spawnedExplosion);
		}

		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		det.size = explosionSize;
		if (timer >= explosionDelay) {
			det.Explode ();
			Destroy (gameObject);
		}
	}

	public void init(float delay, float size) {
		explosionDelay = delay;
		explosionSize = size;
	}
}
