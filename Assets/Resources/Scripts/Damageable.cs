using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Damageable : NetworkBehaviour {

	public static ObjectList damageableList;

	public float maxArmor;
	[SyncVar] float armor;

	void Start() {
		armor = maxArmor;
		if (damageableList == null) {
			damageableList = new ObjectList();
		}
		damageableList.addObject (gameObject);
	}

	void OnDestroy() {
		damageableList.removeObject (gameObject);
	}

	[Server]
	public void damage(float amount) {
		if (amount > 0) {

			//Check if the object has a shield
			Shield shield = GetComponent<Shield>();
			if (shield) {
				if (shield.getActive ()) {
					armor -= amount / shield.getDamageReduction ();
					shield.damageShield(amount);
				} else {
					armor -= amount;
				}
			} else {
				armor -= amount;
			}
		}
	}

	[Server]
	public void heal(float amount) {
		if (amount > 0) {
			armor += amount;
			check();
		}
	}

	public void setArmor(float value) {
		if (value > 0) {
			armor = value;
			check ();
		}
	}

	public void setMaxArmor(float value) {
		if (value > 0) {
			maxArmor = value;
			check ();
		}
	}

	void check() {
		if (armor < 0) {
			armor = 0;
		}

		if (armor > maxArmor) {
			armor = maxArmor;
		}
	}

	public float getArmor() {
		return armor;
	}

	public float getMaxArmor() {
		return maxArmor;
	}
}
