using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using NetworkPlayer = Playground.Player.NetworkPlayer;

namespace Playground.Graphics
{
    public class OverheadNameDrawer : MonoBehaviour
    {
        [SerializeField] private LayerMask hitLayer;
        [SerializeField] private TMP_Text sample;
        [SerializeField] private float appearRange;

        private List<TMP_Text> nameTags = new List<TMP_Text>();
        private Camera mainCamera;
        private RectTransform canvasRect;

        private void Awake()
        {
            mainCamera = Camera.main;
            canvasRect = sample.canvas.GetComponent<RectTransform>();
        }

        private void Update()
        {
            var players = FindObjectsByType<NetworkPlayer>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            var tagCount = 0;
            var camPos = mainCamera.transform.position;
            for (var i = 0; i < players.Length; i++)
            {
                var headPos = players[i].transform.position + 1.8f * Vector3.up;
                if (Vector3.Distance(headPos, camPos) > appearRange) continue;
                Debug.DrawLine(headPos, camPos);
                if (Physics.Linecast(headPos, camPos, hitLayer)) continue;

                var nameTag = GetTMPTextAt(tagCount++);
                nameTag.gameObject.SetActive(true);
                nameTag.text = players[i].Name.Value.Value;

                var screenPos = mainCamera.WorldToScreenPoint(headPos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPos,
                    null,
                    out var point
                );
                nameTag.rectTransform.anchoredPosition = point + 1.0f * Vector2.up;
            }

            for (var i = tagCount; i < nameTags.Count; i++)
            {
                var nameTag = GetTMPTextAt(i);
                nameTag.gameObject.SetActive(false);
            }
        }

        private TMP_Text GetTMPTextAt(int index)
        {
            while (nameTags.Count <= index)
            {
                var newTag = Instantiate(sample, sample.transform.parent);
                nameTags.Add(newTag);
            }

            return nameTags[index];
        }
    }
}