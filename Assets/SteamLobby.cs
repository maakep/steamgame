using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class SteamLobby : MonoBehaviour
{

    public static SteamLobby Instance;
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;


    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private CustomNetworkManager manager;


    
    private void Start()
    {
        if (!SteamManager.Initialized) { return; }
        if (Instance == null)
        {
            Instance = this;
        }

        manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnetered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t cb)
    {
        if (cb.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created successfully");

        manager.StartHost();

        var steamId = new CSteamID(cb.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(steamId, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(steamId, "name", "Lobby of " + SteamFriends.GetPersonaName());

    }

    private void OnJoinRequest(GameLobbyJoinRequested_t cb)
    {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(cb.m_steamIDLobby);
    }

    private void OnLobbyEnetered(LobbyEnter_t cb)
    {
        Debug.Log("Lobby enter");
        var steamId = new CSteamID(cb.m_ulSteamIDLobby);
        
        CurrentLobbyID = cb.m_ulSteamIDLobby;

        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(steamId, HostAddressKey);
        manager.StartClient();
    }
}
