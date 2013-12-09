using UnityEngine;
using System.Collections;

public class ImAnEgg : MonoBehaviour {

	public Transform target;
	public bool seeking;
	Vector3 start_point, total_distance, left;
	Vector3 nextpos, lerp_pos, last_pos, pos;
	public AnimationCurve haight = new AnimationCurve();
	public float maxspeed, distance;
	Quaternion rot;
	bool render;
	Transform parent;



	void Update() {
		if (networkView.isMine) {
			if (seeking) {
				Seek();
			}
		}
		else {
			if (transform.position != pos) {
				lerp_pos = pos + (pos - last_pos);
				transform.position = Vector3.MoveTowards(transform.position,lerp_pos, Time.deltaTime*Vector3.Distance(pos, transform.position)*10);
				last_pos = pos;
			}
		}
	}

	void Seek() {

		distance += maxspeed;
		total_distance = new Vector3(target.position.x - start_point.x, 0, target.position.z - start_point.y);
		nextpos = target.position + Vector3.up - start_point;
		nextpos = start_point + nextpos.normalized*distance;
		nextpos.y += haight.Evaluate(distance/total_distance.magnitude);
		transform.position = nextpos;
		left = target.position + Vector3.up - transform.position;
		if (left.magnitude < 0.3f) {
			seeking = false;
			transform.parent = target;
			renderer.enabled = false;
			player_controller new_owner = target.GetComponent(typeof(player_controller)) as player_controller;
			new_owner.networkView.RPC ("Carry", RPCMode.All, true);
		}

	}


	void Throw_Egg(Transform to) {
		target = to;
		distance = 0;
		transform.parent = null;
		renderer.enabled = true;
		start_point = transform.position;
		seeking = true;
	}
	



	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			pos = transform.position;
			rot = transform.rotation;
			render = renderer.enabled;
			stream.Serialize(ref render);
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		}
		else {
			stream.Serialize(ref render);
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			transform.rotation = rot;
			renderer.enabled = render;
		}
		
	}

}
