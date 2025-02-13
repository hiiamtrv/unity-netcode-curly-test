using System;
using Playground.Player;
using Unity.Netcode;
using UnityEngine;
using NetworkPlayer = Playground.Player.NetworkPlayer;
using Random = UnityEngine.Random;

namespace Playground.Scripts.Environment
{
    public class RespawnWall : MonoBehaviour
    {
        private Collider col;

        private void Start()
        {
            col = GetComponent<Collider>();
        }

        private void Update()
        {
            col.enabled = NetworkManager.Singleton.IsServer;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out NetworkPlayer player))
            {
                var playerServer = player.GetComponentInChildren<PlayerServer>();
                playerServer.FreeFall();
            }
        }
    }
}