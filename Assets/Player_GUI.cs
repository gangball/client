using UnityEngine;
using System.Collections;

public class Player_GUI : MonoBehaviour {

	Transform cursor;
	player_camera camscript;
	Projector render;
	GameObject player;
	Vector3 distance;
	public float max_distance;
	public player_controller controller;

	void Start() {
		cursor = GameObject.Find ("target_projector").transform as Transform;
		render = cursor.GetComponent(typeof(Projector)) as Projector;
	}

	void Update() {
		Ray target = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitinfo;
		if (player != null) {
		if (Physics.Raycast(target, out hitinfo, 200)) {
			cursor.position = hitinfo.point;
			distance = player.transform.position - hitinfo.point;
			if (hitinfo.transform.tag == "Player" && distance.magnitude < max_distance) {
				render.material.color = Color.green;
					if (Input.GetMouseButtonDown(0) && controller.carrying && hitinfo.transform != player.transform && controller.throw_delay > 0.5f) {
						if (!Network.isServer) {
						hitinfo.transform.networkView.RPC ("hot_potato", RPCMode.Server);
						controller.carrying = false;
						}
						else {
							hitinfo.transform.SendMessage("hot_potato", SendMessageOptions.RequireReceiver);
							controller.carrying = false;
						}
					}
			}
			else if (distance.magnitude > max_distance) {
				render.material.color = Color.red;
			}
			else render.material.color = Color.white;
		}
		}
		else render.material.color = Color.clear;
	}

	void Initialize() {
		camscript = gameObject.GetComponent(typeof(player_camera)) as player_camera;
		player = camscript.follow_object.gameObject;
		controller = player.GetComponent(typeof(player_controller)) as player_controller;
	}
}
