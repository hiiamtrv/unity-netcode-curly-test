using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Playground.UI
{
    public class CreateRoomUI : MonoBehaviour
    {
        [SerializeField] private Button btnCreateRoom;
        [SerializeField] private Button btnCloseRoom;
        [SerializeField] private TMP_Text textRoomInfo;
        [SerializeField] private TMP_Text textResult;
        [SerializeField] private TMP_InputField inpPort;

        public Action<string> OnCreateRoom;
        public UnityAction OnCloseRoom;

        private void Awake()
        {
            btnCreateRoom.onClick.AddListener(() => OnCreateRoom?.Invoke(inpPort.text));
            btnCloseRoom.onClick.AddListener(OnCloseRoom);

            SetOpenState();
        }

        public void SetOpenState(string ip = null, ushort port = 0)
        {
            textRoomInfo.gameObject.SetActive(true);
            textRoomInfo.text = $"Address: {ip} - Port: {port.ToString()}";

            btnCreateRoom.gameObject.SetActive(false);
            btnCloseRoom.gameObject.SetActive(true);
        }

        public void SetCloseState()
        {
            textRoomInfo.gameObject.SetActive(false);
            btnCreateRoom.gameObject.SetActive(true);
            btnCloseRoom.gameObject.SetActive(false);
        }

        public void SetTextResult(string text, bool error = false)
        {
            textResult.text = text;
            textResult.color = error ? Color.red : Color.black;
        }
    }
}