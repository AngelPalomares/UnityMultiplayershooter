using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
    }


    //Series of names.
    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        ChangeStat
    }


    public List<PlayerInfo> AllPlayers = new List<PlayerInfo>();
    private int Index;

    private void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }

    void Update()
    {
        
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {

            EventCodes theevent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("Received Event " + theevent);

            switch(theevent)
            {
                case EventCodes.NewPlayer:

                    NewPlayerReceived(data);

                    break;

                case EventCodes.ListPlayers:
                    ListPlayersReceived(data);
                    break;

                case EventCodes.ChangeStat:
                    UpdatedChangeReceived(data);
                    break;

            }

        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string UserName)
    {
        object[] Package = new object[4];
        Package[0] = UserName;
        Package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        Package[2] = 0;
        Package[3] = 0;


        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.NewPlayer,
            Package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }



            ) ;

    }

    public void NewPlayerReceived(object[] dataReceived) {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);

        AllPlayers.Add(player);
    }

    public void ListPlayersSend()
    {

    }

    public void ListPlayersReceived(object[] dataReceived)
    {

    }

    public void UpdateChangeSedn()
    {

    }

    public void UpdatedChangeReceived(object[] dataReceived)
    {

    }


}
[System.Serializable]
public class PlayerInfo
{
    public string NameOfPlayer;
    public int Actor, Kills, Deaths;

    public PlayerInfo(string _name, int _Actor, int _Kills, int _Deaths)
    {
        NameOfPlayer = _name;
        Actor = _Actor;
        Kills = _Kills;
        Deaths = _Deaths;
    }

}