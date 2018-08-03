using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {

	//UI Hookup
	public Text score;
	private int intScore = 0;

	//Key inputs
	public KeyCode leftKey;
	public KeyCode rightKey;
	public KeyCode attackKey;
	public KeyCode blockKey;
	public KeyCode throwKey;

	//Unity components
	Rigidbody2D rb;

	//Physics variables - We set these
	private float maxMovespeed = 20;					// If this could be in actual unity units per second somehow, that would be great
	private float accelerationTime = 0.1f;				// This is in actual seconds
	private float deccelerationTime = 0.1f;				// This is in actual seconds

	//Physics variables - These get set for us
	private float acceleration;
	private float decceleration;

	//Physics variables - State variables
	float leftSpeed = 0.0f;
	float rightSpeed = 0.0f;

	//Sprite settings variables
	private float inputDirection = 1.0f;


	//Blocking
	public bool isBlocking = false;

	//Attacking
	// RYU CROUCHING HK
	// STARTUP		ACTIVE		RECOVERY		ADVANTAGE-BLOCK		ADVANTAGE-HIT
	// 7			2			22				-11					+4

	// CURRENT ATTACK DATA
	// STARTUP		ACTIVE		RECOVERY		BLOCKSTUN (ADV-HIT)		HITSTUN (ADV-BLOCK)
	// 10			3			25				10 (-15)				30 (+5)

	public GameObject attackPrefab;
	public bool isAttacking = false;

	private float startupTimer = 0.0f;
	private float startupTime = 10 * (1/60.0f);
	public bool atkStartUp = false;

	private float activeTimer = 0.0f;
	private float activeTime = 3 * (1 / 60.0f);
	public bool atkActive = false;

	private float recoveryTimer = 0.0f;
	private float recoveryTime = 25 * (1 / 60.0f);
	public bool atkRecovery = false;

	private float blockStunTimer = 0.0f;
	private float blockStunTime = 10 * (1 / 60.0f);
	public bool blockStun = false;

	private float hitStunTimer = 0.0f;
	private float hitStunTime = 30 * (1 / 60.0f);
	public bool hitStun = false;

	public float attackDirection = 1.1f;
	private Vector3 attackDirectionVector;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		attackDirectionVector = new Vector3(attackDirection, 0, 0);
		SetupMoveSpeed ();
	}
	
	// Update is called once per frame
	void Update () {

		HorizontalSpeed();
		Attacking();
		Blocking();
		HitStun();
		BlockStun();

		//Debug.Log (rb.velocity.x);
	}

	public float GetInputDirection(){
		return inputDirection;
	}

	void OnTriggerEnter2D(Collider2D collision) {

		if (isBlocking) {
			blockStun = true;
		}

		if (!isBlocking){

			intScore++;
			score.text = intScore.ToString();

			hitStun = true;
			if (isAttacking) {
				InterruptAttack();
			}
		}
	}

	void Blocking(){
		if (!isAttacking && !hitStun && !blockStun) {
		
			isBlocking = Input.GetKey(blockKey);
		}
	}

	void InterruptAttack(){
		isAttacking = false;

		startupTimer = 0.0f;
		atkStartUp = false;

		activeTimer = 0.0f;
		atkActive = false;

		recoveryTimer = 0.0f;
		atkRecovery = false;
	}

	void BlockStun(){

		if (blockStun) {

			blockStunTimer += Time.deltaTime;
			if (blockStunTimer > blockStunTime) {
				blockStunTimer = 0.0f;
				blockStun = false;
			}
		}
	}

	void HitStun() {

		if (hitStun) {

			hitStunTimer += Time.deltaTime;
			if (hitStunTimer > hitStunTime) {
				hitStunTimer = 0.0f;
				hitStun = false;
			}
		}
	}

	void Attacking(){

		if (!isAttacking && !hitStun && !blockStun) {
			if (Input.GetKey(attackKey)) {
				isAttacking = true;
				atkStartUp = true;
			}
		}
		
			
		if(isAttacking){
			if(atkStartUp){
				
				startupTimer += Time.deltaTime;
				if (startupTimer > startupTime) {
					startupTimer = 0.0f;
					atkStartUp = false;
					atkActive = true;

					GameObject attackObject = Instantiate(attackPrefab, transform.position + attackDirectionVector, transform.rotation) as GameObject;
					attackObject.GetComponent<Attack>().DoAttack(activeTime, hitStunTime, blockStunTime);
				}

			}

			if(atkActive){

				activeTimer += Time.deltaTime;
				if (activeTimer > activeTime) {
					activeTimer = 0.0f;
					atkActive = false;
					atkRecovery = true;
				}

			}

			if (atkRecovery) {

				recoveryTimer += Time.deltaTime;
				if (recoveryTimer > recoveryTime) {
					recoveryTimer = 0.0f;
					atkRecovery = false;
					isAttacking = false;
				}

			}
		}
	}

	void HorizontalSpeed(){
		
		if(Input.GetKey (leftKey) && !isAttacking && !isBlocking && !hitStun && !blockStun){
			inputDirection = -1.0f;
			leftSpeed = acceleration * -1.0f * Time.deltaTime;
		} else {
			leftSpeed = decceleration * 1.0f * Time.deltaTime;
		}

		if(Input.GetKey (rightKey) && !isAttacking && !isBlocking && !hitStun && !blockStun){
			inputDirection = 1.0f;
			rightSpeed = acceleration * 1.0f * Time.deltaTime;
		} else {
			rightSpeed = decceleration * -1.0f * Time.deltaTime;
		}	

		rb.velocity = new Vector2 (rightSpeed + leftSpeed, rb.velocity.y);
	}

	void SetupMoveSpeed(){
		//Scale acceleration values to the movespeed and wanted acceleration times
		acceleration = maxMovespeed / accelerationTime;
		decceleration = maxMovespeed / deccelerationTime;
	}
}
