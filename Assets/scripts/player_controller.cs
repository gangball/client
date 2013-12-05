using UnityEngine;
using System.Collections;

public class player_controller : MonoBehaviour {


	public Animator player_animator;
	public float player_speed;
	public Vector3 movement;
	public LayerMask playermask;
	PhysicMaterial mat;
	Quaternion angles;
	bool grounded;

	void Start() {
		angles = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y, 0);
		mat = this.collider.material;
	}

	void Update() {
			


		if (GroundCheck()) {
			grounded = true;
		} 
		else grounded = false;

			if (Input.GetAxis("Vertical") != 0) {
				movement.z = Input.GetAxisRaw("Vertical");
			}
			else {
				movement.z = 0;
			}
			if (Input.GetAxis("Horizontal") != 0) {
				movement.x = Input.GetAxisRaw("Horizontal");
			}
			else {
				movement.x = 0;
			}







		movement = angles * movement.normalized * player_speed;

		if (!grounded) {
			movement *= 0.1f;
		}

		movement.y = rigidbody.velocity.y;

		rigidbody.velocity = movement;

		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
			transform.LookAt(new Vector3(transform.position.x + movement.x, transform.position.y, transform.position.z + movement.z));
		}

		/*
		Kostyli
		*/

		if (movement != Vector3.zero && (mat.dynamicFriction != 0.1f || mat.staticFriction != 0.1f)) {
			mat.dynamicFriction = Mathf.MoveTowards(mat.dynamicFriction, 0.1f, 0.05f);
			mat.staticFriction = Mathf.MoveTowards(mat.staticFriction, 0.1f, 0.05f);
		}
		else if (mat.dynamicFriction != 0.5f || mat.staticFriction != 0.5f) {
			mat.dynamicFriction = Mathf.MoveTowards(mat.dynamicFriction, 0.5f, 0.05f);
			mat.staticFriction = Mathf.MoveTowards(mat.staticFriction, 0.5f, 0.05f);
		}



		/*
		Animator stuff
		*/

		player_animator.SetFloat("speed", movement.magnitude);

	}

	bool GroundCheck() {
		if (Physics.CheckSphere(transform.position, 0.1f, playermask)) {
			return true;
		}
		else return false;
	}

	
	}


