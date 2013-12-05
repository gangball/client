using UnityEngine;
using System.Collections;

public class player_controller : MonoBehaviour {


	public Animator player_animator;
	public float player_speed;
	public Vector3 movement;
	Quaternion angles;
	bool grounded;

	void Start() {
		angles = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y, 0);
	}

	void Update() {
			
		if (grounded) {
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
		}
		else { 
			movement = Vector3.zero;
		}


		movement = angles * movement.normalized * player_speed;

		movement.y = rigidbody.velocity.y;

		rigidbody.velocity = movement;

		if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
			transform.LookAt(new Vector3(transform.position.x + movement.x, transform.position.y, transform.position.z + movement.z));
		}



		/*
		Animator stuff
		*/

		player_animator.SetFloat("speed", movement.magnitude);

	}

	void OnCollisionStay(Collision collision) {
		for (int i = 0; i < collision.contacts.Length; i++) {
			if (collision.contacts[i].normal.y > 0.5f) {
				grounded = true;
				return;
			}
			else grounded = false;
		}

	
	}

}
