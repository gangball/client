using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonitorAI : MonoBehaviour {

	bool chase = true;
	public Transform target;
	Vector3 distance;
	public Transform monitor;
	public Animator mon_animator;
	Quaternion rotation;
	float last_update, speed, mobility;
	public float call_delay;
	public float movement_speed, turn_speed, acceleration;
	Vector3 pos, nextpos, last_pos;
	Quaternion rot;


		
	void Start() {
		last_pos = transform.position;
	}
	
	void Update() {
		last_update += Time.fixedDeltaTime;
		if (networkView.isMine) {
		if (last_update >= call_delay) {
	
				if (chase == true) {
					if (Quaternion.Angle(transform.rotation, rotation) < 10) {
						speed += Time.deltaTime*acceleration/Mathf.Clamp(Quaternion.Angle(transform.rotation, rotation)/2, 1, 10);
						}
					else {
						speed -= Time.deltaTime*acceleration*Mathf.Clamp(Quaternion.Angle(transform.rotation, rotation)/10, 1, 10)/10;
						}

					if (speed < 9) {
						mobility += Mathf.Clamp(turn_speed/speed/20, 0.001f, 0.05f);
					}
					else {
						mobility -= Mathf.Clamp(turn_speed*speed/60, 0.001f, 0.01f);
					}

					mobility = Mathf.Clamp(mobility, 0.02f, 0.3f);
					speed = Mathf.Clamp(speed, 4, movement_speed);

					Debug.Log(speed+" "+mobility);

			distance = target.position - this.transform.position;
			
						NavMeshPath YOBA = new NavMeshPath();
						NavMesh.CalculatePath(this.transform.position,target.position, -1, YOBA);
						//Debug.Log(YOBA.corners.Length);
				if (YOBA.corners.Length > 0) { 
						rotation = Quaternion.LookRotation(new Vector3(YOBA.corners[1].x,transform.position.y,YOBA.corners[1].z) - transform.position);
				}
				else rotation = Quaternion.LookRotation(new Vector3(target.position.x,transform.position.y,target.position.z) - transform.position);

				}
			 

	
			last_update = 0;
			}
		
		monitor.transform.rotation = Quaternion.Slerp(transform.rotation,rotation,mobility);
		rigidbody.AddForce(transform.rotation *  Vector3.forward * speed*100, ForceMode.Force);
		}
		else {
			if (transform.position != pos) {
				nextpos = pos + (pos - last_pos);
				transform.position = Vector3.MoveTowards(transform.position,nextpos, Time.deltaTime*Vector3.Distance(pos, transform.position)*10);
				last_pos = pos;
				//Debug.Log("pos: "+pos+" real: "+transform.position+" distance: "+Vector3.Distance(pos, transform.position));
			}
		}
		mon_animator.SetFloat("run_speed", speed);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			pos = transform.position;
			rot = transform.rotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref speed);
		}
		else {
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			stream.Serialize(ref speed);
			transform.rotation = rot;
			
		}
		
	}

	void OnConnectedToServer()  {
		rigidbody.isKinematic = true;
	}

}
