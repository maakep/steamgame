using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance;

    public TextMeshProUGUI LobbyNameText;

    public GameObject PlayerListViewContent;
    public GameObject PlayerListItemPrefab;
    public GameObject LocalPlayerObject;

    public ulong CurrentLobbyID;
    public bool PlayerItemCreated = false;
    private List<PlayerListItem> PlayerListItems = new List<PlayerListItem>();
    public PlayerObjectController LocalPlayerController;

    private CustomNetworkManager manager;
    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdateLobbyName()
    {
        CurrentLobbyID = Manager.GetComponent<SteamLobby>().CurrentLobbyID;
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(CurrentLobbyID), "name");
    }

    public void UpdatePlayerList()
    {
        if (!PlayerItemCreated)
        {
            CreateHostPlayerItem();
        }
        if (PlayerListItems.Count < Manager.GamePlayers.Count)
        {
            CreateClientPlayerItem();
        }
        if (PlayerListItems.Count > Manager.GamePlayers.Count)
        {
            RemovePlayerItem();
        }
        if (PlayerListItems.Count == Manager.GamePlayers.Count) 
        {
            UpdatePlayerItem();
        }
    }

    public void FindLocalPlayer()
    {
        LocalPlayerObject = GameObject.Find("LocalGamePlayer");
        LocalPlayerController = LocalPlayerObject.GetComponent<PlayerObjectController>();
    }

    private void CreateHostPlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            var newPlayerItem = Instantiate(PlayerListItemPrefab);
            var newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

            newPlayerItemScript.PlayerName = player.PlayerName;
            newPlayerItemScript.ConnectionID = player.ConnectionID;
            newPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
            newPlayerItemScript.SetPlayerValues();

            newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;
            PlayerListItems.Add(newPlayerItemScript);
        }

        PlayerItemCreated = true;
    }


    public void CreateClientPlayerItem() 
    {
        foreach (var player in Manager.GamePlayers)
        {
            if (!PlayerListItems.Any(item => item.ConnectionID == player.ConnectionID))
            {
                var newPlayerItem = Instantiate(PlayerListItemPrefab);
                var newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.PlayerName = player.PlayerName;
                newPlayerItemScript.ConnectionID = player.ConnectionID;
                newPlayerItemScript.PlayerSteamID = player.PlayerSteamID;
                newPlayerItemScript.SetPlayerValues();

                newPlayerItem.transform.SetParent(PlayerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;
                PlayerListItems.Add(newPlayerItemScript);
            }
        }
    }

    public void UpdatePlayerItem()
    {
        foreach (var player in Manager.GamePlayers)
        {
            foreach (var item in PlayerListItems)
            {
                if (item.ConnectionID == player.ConnectionID)
                {
                    item.PlayerName = player.PlayerName;
                    item.SetPlayerValues();
                }
            }
        }
    }

    public void RemovePlayerItem()
    {
        var toRemove = new List<PlayerListItem>();
        foreach (var playerListItem in PlayerListItems)
        {
            if (!Manager.GamePlayers.Any(p => p.ConnectionID == playerListItem.ConnectionID))
            {
                toRemove.Add(playerListItem);
            }
        }

        if (toRemove.Count > 0)
        {
            foreach (var item in toRemove)
            {
                var objectToRemove = item.gameObject;
                PlayerListItems.Remove(item);
                Destroy(objectToRemove);
                objectToRemove = null;
            }
        }
    }


    public void StartGame(string sceneName)
    {
        LocalPlayerController.CanStartGame(sceneName);
    }
}
