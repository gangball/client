using UnityEngine;
using System.Collections;

public class player_camera : MonoBehaviour {

	public Transform follow_object;
	public Vector3 offset;


void Awake() {

		offset = transform.position - follow_object.position;

	}

void Update() {

		transform.position = follow_object.position + offset;
	}
}