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

        public NetworkVariable<Vector2> MoveDirection = NetworkVar.Make<Vector2>();
        public NetworkVariable<FixedString64Bytes> Name = NetworkVar.Make<FixedString64Bytes>();
        public NetworkVariable<bool> IsGrounded = NetworkVar.Make<bool>();
        public NetworkVariable<float> Speed = NetworkVar.Make<float>();
        public NetworkVariable<float> ChargeValue = NetworkVar.Make<float>(false, false, -1);

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

        [ServerRpc]
        public void RequestChangeNameServerRpc(FixedString64Bytes newName)
        {
            Name.Value = newName;
        }

        [ServerRpc]
        public void RequestChangeMoveDirectionServerRpc(float x, float z)
        {
            MoveDirection.Value = new Vector2(x, z);
        }
    }
}