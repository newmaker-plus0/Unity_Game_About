using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject gameCanvas;
    public GameObject sceneCamera;

    //PING
    public Text pingText;

    //斷開連結
    public GameObject disconnectUI;
    private bool off = false;

    //進入訊息
    public GameObject playerFeed;
    public GameObject feedGrid;

    //復活相關
    [HideInInspector] public GameObject localPlayer;
    public Text respawnTimerText;
    public GameObject respawnMenu;
    private float timerAmount = 5f;
    private bool runSpawnTimer = false;

    private void Awake()
    {
        instance = this;
        gameCanvas.SetActive(true);
    }

    private void Update()
    {
        CheckInput();
        pingText.text = "Ping: " + PhotonNetwork.GetPing();

        if(runSpawnTimer)
        {
            StartRespawn();
        }
    }

    public void EnableRespawn()
    {
        timerAmount = 5f;
        runSpawnTimer = true;
        respawnMenu.SetActive(true);
    }

    private void StartRespawn()
    {
        timerAmount -= Time.deltaTime;
        respawnTimerText.text = "Respawn in" + timerAmount.ToString("F0");

        if(timerAmount <= 0)
        {
            localPlayer.GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
            respawnMenu.SetActive(false);
            runSpawnTimer = false;
        }
    }

    private void CheckInput()
    {
        if(off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(false);
            off = false;
        }
        else if(!off && Input.GetKeyDown(KeyCode.Escape))
        {
            disconnectUI.SetActive(true);
            off = true;
        }
    }

    //生成玩家
    public void SpawnPlayer()
    {
        float randomValue = Random.Range(-1f, 1f);

        PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(this.transform.position.x * randomValue, this.transform.position.y), Quaternion.identity, 0);
        gameCanvas.SetActive(false);
        sceneCamera.SetActive(true);
    }

    //離開房間
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }

    //進入訊息
    private void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        GameObject obj = Instantiate(playerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<Text>().text = player.name + " joined the game";
        obj.GetComponent<Text>().color = Color.green;
    }

    //離開訊息
    private void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("get away");
        GameObject obj = Instantiate(playerFeed, new Vector2(0, 0), Quaternion.identity);
        obj.transform.SetParent(feedGrid.transform, false);
        obj.GetComponent<Text>().text = player.name + " left the game";
        obj.GetComponent<Text>().color = Color.red;
    }
}
