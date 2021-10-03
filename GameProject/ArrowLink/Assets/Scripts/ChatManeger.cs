using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManeger : MonoBehaviour
{
    public Player plMove;
    public PhotonView photonView;
    public GameObject bubbleSpeechObject;
    public Text updateText;

    private InputField chatInputField;
    private bool disableSend;

    private void Awake()
    {
        chatInputField = GameObject.Find("ChatInputField").GetComponent<InputField>();
    }

    private void Update()
    {
        if(photonView.isMine)
        {
            if(!disableSend && chatInputField.isFocused)
            {
                if(chatInputField.text != "" && chatInputField.text.Length > 0 && Input.GetKeyDown(KeyCode.Slash))
                {
                    photonView.RPC("SendMessage", PhotonTargets.AllBuffered, chatInputField.text);
                    bubbleSpeechObject.SetActive(true);

                    chatInputField.text = "";
                    disableSend = true;
                }
            }
        }
    }

    [PunRPC]
    private void SendMessage(string message)
    {
        updateText.text = message;

        StartCoroutine("Remove");
    }

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(4f);
        bubbleSpeechObject.SetActive(false);
        disableSend = false;
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.isWriting)
        {
            stream.SendNext(bubbleSpeechObject.active);
        }
        else if(stream.isReading)
        {
            bubbleSpeechObject.SetActive((bool)stream.ReceiveNext());
        }
    }
}
