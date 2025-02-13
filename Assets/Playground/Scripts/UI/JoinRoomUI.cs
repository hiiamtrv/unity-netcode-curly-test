using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Playground.UI
{
    public class JoinRoomUI : MonoBehaviour
    {
        [SerializeField] private Button btnLeave;
        [SerializeField] private Button btnConnect;
        [SerializeField] private TMP_Text textResult;
        [SerializeField] private TMP_InputField inpAddress;
        [SerializeField] private TMP_InputField inpPort;

        public Action<string, string> OnConnect;
        public UnityAction OnLeave;

        private void Awake()
        {
            btnConnect.onClick.AddListener(() => OnConnect?.Invoke(inpAddress.text, inpPort.text));
            btnLeave.onClick.AddListener(OnLeave);
            SetStateJoined(false);
        }

        public void SetTextResult(string text, bool error = false)
        {
            textResult.text = text;
            textResult.color = error ? Color.red : Color.black;
        }

        public void SetStateJoined(bool isJoined)
        {
            btnConnect.gameObject.SetActive(!isJoined);
            btnLeave.gameObject.SetActive(isJoined);
        }
    }
}