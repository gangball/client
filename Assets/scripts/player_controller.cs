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
	Vector3 pos, last_pos, nextpos;
	Quaternion rot;

	void Start() {
		if (networkView.isMine) {
		angles = Quaternion.Euler(0,Camera.main.transform.eulerAngles.y, 0);
		mat = this.collider.material;
		Camera.main.gameObject.SendMessage("Watch", transform, SendMessageOptions.DontRequireReceiver);
		}
		else rigidbody.isKinematic = true;
	}

	void Update() {

		if (networkView.isMine) {
			


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

		

		}
		else {
			if (transform.position != pos) {
				nextpos = pos + (pos - last_pos);
				transform.position = Vector3.MoveTowards(transform.position,nextpos, Time.deltaTime*Vector3.Distance(pos, transform.position)*10);
				last_pos = pos;
			}
		}

		/*
		Animator stuff
		*/

		player_animator.SetFloat("speed", movement.magnitude);

	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			 pos = transform.position;
			 rot = transform.rotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref movement);
		}
		else {
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref movement);
			transform.rotation = rot;
			
		}

	}

	bool GroundCheck() {
		if (Physics.CheckSphere(transform.position, 0.1f, playermask)) {
			return true;
		}
		else return false;
	}

	
	}


