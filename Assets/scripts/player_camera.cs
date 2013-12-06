using UnityEngine;
using System.Collections;

public class player_camera : MonoBehaviour {

	public Transform follow_object;
	public Vector3 offset;


void Update() {
	if (follow_object != null) {
		transform.position = follow_object.position + offset;
		}
	}

void Watch(Transform target) {
		follow_object = target;
		offset = transform.position - follow_object.position;
	}
}