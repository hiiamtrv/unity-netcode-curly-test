using System;
using System.Collections;
using Playground.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Playground.RoomManager
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] private CreateRoomUI createRoomUI;
        [SerializeField] private JoinRoomUI joinRoomUI;
        [SerializeField] private Button toggleButton;

        private bool isShowingCreate = true;
        private NetworkManager network => NetworkManager.Singleton;

        private void Awake()
        {
            toggleButton.onClick.AddListener(ToggleShowingCreate);

            createRoomUI.OnCreateRoom += CreateRoom;
            createRoomUI.OnCloseRoom += CloseRoom;
            joinRoomUI.OnConnect += ConnectRoom;
            joinRoomUI.OnLeave += LeaveRoom;

            UpdateUI();
            createRoomUI.SetCloseState();
            joinRoomUI.SetStateJoined(false);
        }

        private void ToggleShowingCreate()
        {
            isShowingCreate = !isShowingCreate;
            UpdateUI();
        }

        private void UpdateUI()
        {
            createRoomUI.gameObject.SetActive(isShowingCreate);
            joinRoomUI.gameObject.SetActive(!isShowingCreate);

            toggleButton.GetComponentInChildren<TMP_Text>().text = isShowingCreate
                ? ">> Join room"
                : ">> Create room";
        }

        private void CreateRoom(string sPort)
        {
            if (!ushort.TryParse(sPort, out ushort port))
            {
                createRoomUI.SetTextResult("Invalid port", true);
                return;
            }

            if (network.NetworkConfig.NetworkTransport is UnityTransport transport)
            {
                if (network.IsListening)
                {
                    network.Shutdown();
                }

                try
                {
                    StartCoroutine(GetPublicIP((address) =>
                    {
                        transport.ConnectionData.Address = address;
                        transport.ConnectionData.Port = port;
                        network.StartHost();

                        createRoomUI.SetOpenState(address, port);
                        createRoomUI.SetTextResult("Room Created", true);
                    }));
                }
                catch (Exception e)
                {
                    createRoomUI.SetTextResult(e.Message, true);
                }
            }
        }

        private void CloseRoom()
        {
            network.Shutdown();
            createRoomUI.SetCloseState();
            createRoomUI.SetTextResult("Room Closed");
        }

        private void ConnectRoom(string address, string sPort)
        {
            if (!ushort.TryParse(sPort, out ushort port))
            {
                joinRoomUI.SetTextResult("Invalid port", true);
                return;
            }

            if (network.NetworkConfig.NetworkTransport is UnityTransport transport)
            {
                if (network.IsListening)
                {
                    network.Shutdown();
                }

                try
                {
                    transport.ConnectionData.ServerListenAddress = address;
                    transport.ConnectionData.Port = port;
                    network.StartClient();

                    joinRoomUI.SetTextResult("Connected to server", true);
                    joinRoomUI.SetStateJoined(true);
                }
                catch (Exception e)
                {
                    joinRoomUI.SetTextResult(e.Message, true);
                    joinRoomUI.SetStateJoined(false);
                }
            }
        }

        private void LeaveRoom()
        {
            network.Shutdown();
            joinRoomUI.SetStateJoined(false);
            joinRoomUI.SetTextResult("Left room");
        }

        private IEnumerator GetPublicIP(Action<string> onComplete)
        {
            var webRequest = UnityWebRequest.Get("https://api64.ipify.org?format=text");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var publicIP = webRequest.downloadHandler.text;
                onComplete.Invoke(publicIP);
            }
            else
            {
                Debug.Log("Get API Failed");
            }
        }
    }
}