using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviour
{
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerIcon;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start()
    {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t cb)
    {
        if (cb.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerIcon.texture = GetSteamImageAsTexture(cb.m_iImage);
        }
    }

    void GetPlayerIcon()
    {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)PlayerSteamID);
        if (imageId == -1)
        {
            Debug.Log("Something went wrong when fetching image");
            return;
        }
        PlayerIcon.texture = GetSteamImageAsTexture(imageId);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        if (!AvatarReceived)
        {
            GetPlayerIcon();
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
}
