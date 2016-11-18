using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {
    private void ToMainMenu() {
        Camera.main.enabled = true;}
    
	void OnClientDisconnect() {
        ToMainMenu();}

	void OnServerStop() {
        ToMainMenu();}}
