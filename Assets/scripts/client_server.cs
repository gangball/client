using UnityEngine;
using System.Collections;

public class client_server : MonoBehaviour {

	public string game_version;
	public GameObject[] player;
	public Transform[] spawners;
	bool refreshing;
	HostData[] servers;


	void Start() {

	}

	public void Create(int connections, int port, string server_name) {
		Network.InitializeServer(connections, port, !Network.HavePublicAddress()); 
		MasterServer.RegisterHost(game_version, server_name);
	}

	public void Join(HostData data) {
		Network.Connect(data);


	}


	[RPC]
	public void Spawn(int model, int at, NetworkViewID viewID) {
		GameObject instantiated_player;
		instantiated_player = Instantiate(player[model], spawners[at].position, Quaternion.identity) as GameObject;
		instantiated_player.networkView.viewID = viewID;

	}

	void OnConnectedToServer() {
		NetworkViewID myID = Network.AllocateViewID();
		networkView.RPC("Spawn", RPCMode.AllBuffered, 0, 0, myID);
	}

	void OnServerInitialized() {
		NetworkViewID myID = Network.AllocateViewID();
		networkView.RPC("Spawn", RPCMode.AllBuffered, 0, 0, myID);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}


	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
		if (GUI.Button(new Rect(20, 20, 100, 30), "Create")) {
			Create(2, 25001, "Test server");
		}
		if (!refreshing) {
		if (GUI.Button(new Rect(20, 60, 100, 30), "Join")) {
			MasterServer.RequestHostList(game_version);
			refreshing = true;
		}
		}
		else {
				GUI.Label(new Rect(20, 60, 100, 30), "Searching");
		}
	}
	}

	void Update() {
		if (refreshing) {
			if (MasterServer.PollHostList().Length > 0) {
				servers = MasterServer.PollHostList();
				Join(servers[0]);
				refreshing = false;
			}
		}
	}


}
