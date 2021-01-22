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
		damageableList.AddObject (gameObject);
	}

	void OnDestroy() {
		damageableList.RemoveObject (gameObject);
	}

	[Server]
	public void Damage(float amount) {
		if (amount > 0) {

			//Check if the object has a shield
			Shield shield = GetComponent<Shield>();
			if (shield) {
				if (shield.GetActive ()) {
					armor -= amount / shield.GetDamageReduction ();
					shield.DamageShield(amount);
				} else {
					armor -= amount;
				}
			} else {
				armor -= amount;
			}
		}
	}

	[Server]
	public void Heal(float amount) {
		if (amount > 0) {
			armor += amount;
			Check();
		}
	}

	public void SetArmor(float value) {
		if (value > 0) {
			armor = value;
			Check ();
		}
	}

	public void SetMaxArmor(float value) {
		if (value > 0) {
			maxArmor = value;
			Check ();
		}
	}

	void Check() {
		if (armor < 0) {
			armor = 0;
		}

		if (armor > maxArmor) {
			armor = maxArmor;
		}
	}

	public float GetArmor() {
		return armor;
	}

	public float GetMaxArmor() {
		return maxArmor;
	}
}
