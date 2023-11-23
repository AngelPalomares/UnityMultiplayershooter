using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject LoadingScreen;

    public TMP_Text Text;

    public GameObject menuButtons;

    public GameObject CreateRoomScreen;

    public TMP_Text RoomNameInput;

    public GameObject RoomScreen;
    public TMP_Text RoomNameText;

    public GameObject ErrorScreen;
    public TMP_Text ErrorText;
    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();

        LoadingScreen.SetActive(true);

        Text.text = "Connecting To Network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    void CloseMenus()
    {
        LoadingScreen.SetActive(false);
        menuButtons.SetActive(false);
        CreateRoomScreen.SetActive(false);
        RoomScreen.SetActive(false);
        ErrorScreen.SetActive(false);
    }

    //once connected to the master server it automatically connects the player to the lobby
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
        Text.text = "Joining Lobby";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        CreateRoomScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(RoomNameInput.text))
        {
            RoomOptions options = new RoomOptions();

            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(RoomNameInput.text, options);

            CloseMenus();
            Text.text = "Creating Room...";
            LoadingScreen.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        RoomScreen.SetActive(true);

        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Failed To Create Room: " + message;
        CloseMenus();
        ErrorScreen.SetActive(true);
        
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        Text.text = "Leaving Room";
        LoadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

}
