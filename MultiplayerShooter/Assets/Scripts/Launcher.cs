using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public TMP_Text RoomNameText, PlayerNameLabel;
    private List<TMP_Text> AllPlayerNames = new List<TMP_Text>();

    public GameObject ErrorScreen;
    public TMP_Text ErrorText;

    public GameObject ConnecttoGame;
    public TMP_Text ConnectText;
    public RoomButton TheRoomButton;
    private List<RoomButton> allroomButtons = new List<RoomButton>();

    public GameObject NameInputScreen;
    public TMP_InputField Playername;
    private bool HasSetNickname;

    public string LevelToPlay;

    public GameObject StartButton;
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
        ConnecttoGame.SetActive(false);
        NameInputScreen.SetActive(false);
    }

    //once connected to the master server it automatically connects the player to the lobby
    public override void OnConnectedToMaster()
    {

        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        Text.text = "Joining Lobby";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButtons.SetActive(true);

        if(!HasSetNickname)
        {
            CloseMenus();
            NameInputScreen.SetActive(true);

            if(PlayerPrefs.HasKey("PlayerName"))
            {
                Playername.text = PlayerPrefs.GetString("PlayerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("PlayerName");
        }
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        CreateRoomScreen.SetActive(true);
    }

    public void OpenFindRoom()
    {
        CloseMenus();
        ConnecttoGame.SetActive(true);
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
        ListAllPlayers();

        
        if(PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
        

    }

    private void ListAllPlayers()
    {
        foreach(TMP_Text player in AllPlayerNames)
        {
            Destroy(player.gameObject);
        }

        AllPlayerNames.Clear();


        Player[] players = PhotonNetwork.PlayerList;

        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newplayerLabel = Instantiate(PlayerNameLabel, PlayerNameLabel.transform.parent);

            newplayerLabel.text = players[i].NickName;

            newplayerLabel.gameObject.SetActive(true);
            

            AllPlayerNames.Add(newplayerLabel);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newplayerLabel = Instantiate(PlayerNameLabel, PlayerNameLabel.transform.parent);

        newplayerLabel.text = newPlayer.NickName;
        newplayerLabel.gameObject.SetActive(true);

        AllPlayerNames.Add(newplayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
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

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ErrorText.text = "Failed To Join: " + message;
        CloseMenus();
        ErrorScreen.SetActive(true);
    }


    public void JoinTheRoom(RoomInfo inputInfo)
    {

        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenus();
        Text.text = "Joining Room";
        LoadingScreen.SetActive(true);

    }

    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in allroomButtons)
        {
            Destroy(rb.gameObject);
        }

        allroomButtons.Clear();

        TheRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newbutton = Instantiate(TheRoomButton, TheRoomButton.transform.parent);
                newbutton.setbuttonDetails(roomList[i]);
                newbutton.gameObject.SetActive(true);
                allroomButtons.Add(newbutton);
            }

        }

    }

    public void Quitthegame()
    {
        Application.Quit();
    }

    public void SetNickName()
    {

        if(!string.IsNullOrEmpty(Playername.text))
        {
            PhotonNetwork.NickName = Playername.text;
            PlayerPrefs.SetString("PlayerName", Playername.text);
            CloseMenus();
            menuButtons.SetActive(true);
            HasSetNickname = true;
        }
    }

    public void StarttheGame()
    {
        PhotonNetwork.LoadLevel(LevelToPlay);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartButton.SetActive(true);
        }
        else
        {
            StartButton.SetActive(false);
        }
    }



}
