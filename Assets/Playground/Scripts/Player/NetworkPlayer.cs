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
        public Action OnBlock;
        public Action OnStartCharge;
        public Action OnResetCharge;
        public Action OnReleaseCharge;

        public NetworkVariable<Vector2> MoveDirection = NetworkVar.Make<Vector2>(true, true);
        public NetworkVariable<FixedString64Bytes> Name = NetworkVar.Make<FixedString64Bytes>(true, true);
        public NetworkVariable<bool> IsGrounded = NetworkVar.Make<bool>(true, true);
        public NetworkVariable<float> Speed = NetworkVar.Make<float>(true, true);
        public NetworkVariable<float> ChargeValue = NetworkVar.Make<float>(false, true, -1);

        [ServerRpc]
        public void RequestJumpServerRpc()
        {
            OnJump?.Invoke();
        }

        [ServerRpc]
        public void RequestPrimaryActionServerRpc(bool isRelease)
        {
            if (!isRelease)
            {
                if (Mathf.Approximately(ChargeValue.Value, -1)) OnStartCharge?.Invoke();
            }
            else
            {
                if (ChargeValue.Value > 0.1f) OnReleaseCharge?.Invoke();
                else OnResetCharge.Invoke();
            }
        }

        [ServerRpc]
        public void RequestSecondaryActionServerRpc()
        {
            if (ChargeValue.Value >= 0)
            {
                OnResetCharge?.Invoke();
            }
            else
            {
                OnBlock?.Invoke();
            }
        }
    }
}