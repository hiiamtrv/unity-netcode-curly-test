using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Playground.UI
{
    public class NameUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inpName;
        [SerializeField] private Button btnConfirm;
        [SerializeField] private Button btnCancel;

        public Action<string> OnConfirmName;
        public UnityAction OnCancel;

        private void Awake()
        {
            btnConfirm.onClick.AddListener(() => OnConfirmName?.Invoke(inpName.text));
            btnCancel.onClick.AddListener(OnCancel);
        }

        public void SetName(string name)
        {
            inpName.text = name;
        }
    }
}