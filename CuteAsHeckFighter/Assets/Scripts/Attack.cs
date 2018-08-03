using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	private bool active = false;

	private float timer = 0.0f;
	private float activeTime = 3.0f;
	public float hitstun = 30.0f * (1/60.0f);
	public float blockstun = 10.0f * (1 / 60.0f);

	public void DoAttack(float activeTime, float hitstun, float blockstun){
		this.activeTime = activeTime;
		this.hitstun = hitstun;
		this.blockstun = blockstun;
		active = true;
	}

	void Update () {
		
		if(active){
			timer += Time.deltaTime;
			if (timer > activeTime) {
				Destroy(gameObject);
			}
		}

	}
}
