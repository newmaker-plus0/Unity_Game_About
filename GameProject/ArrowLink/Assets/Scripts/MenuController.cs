using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string versionName = "0.1";
    [SerializeField] private GameObject userNameMenu;
    [SerializeField] private GameObject connectPanel;

    //輸入
    [SerializeField] private InputField userNameInput;
    [SerializeField] private InputField joinGameInput;
    [SerializeField] private InputField createGameInput;

    //按鈕
    [SerializeField] private GameObject startButton;

    //應該是登記版本號
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(versionName);
    }

    private void Start()
    {
        userNameMenu.SetActive(true);
    }

    //連線登陸
    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("connected");
    }

    //修改名字輸入框的內容
    public void ChangeUserNameInput()
    {
        if(userNameInput.text.Length >= 1)
        {
            startButton.SetActive(true);
        }else
        {
            startButton.SetActive(false);
        }
    }

    //設定名字
    public void SetUserName()
    {
        userNameMenu.SetActive(false);
        PhotonNetwork.playerName = userNameInput.text;
    }

    //創建房間
    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(createGameInput.text, new RoomOptions() { maxPlayers = 2 }, null);
    }

    //加入遊戲
    public void JoinGame()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(joinGameInput.text, roomOptions, TypedLobby.Default);
    }

    private void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainGame"); 
    }
}
