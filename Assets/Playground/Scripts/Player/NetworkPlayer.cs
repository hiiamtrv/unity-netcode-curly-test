using System;
using Playground.Base;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Playground.Player
{
    public class NetworkPlayer : MyNetworkObject
    {
        [Header("Fields")] public NetworkPlayerConfig config;

        public Action OnJump;

        public NetworkVariable<Vector2> MoveDirection = NetworkVar.Make<Vector2>(true, true);
        public NetworkVariable<FixedString64Bytes> Name = NetworkVar.Make<FixedString64Bytes>(true, true);
        public NetworkVariable<bool> IsGrounded = NetworkVar.Make<bool>(true, true);
        public NetworkVariable<float> Speed = NetworkVar.Make<float>(true, true);

        [ServerRpc]
        public void RequestJumpServerRpc()
        {
            OnJump?.Invoke();
        }
    }
}