using Unity.Netcode;
using UnityEngine;

namespace Playground.Base
{
    public class MyNetworkObject : NetworkBehaviour
    {
        [Header("Owner/Server/Remote")] [SerializeField]
        protected MonoBehaviour ownerComponent;

        [SerializeField] protected MonoBehaviour serverComponent;
        [SerializeField] protected MonoBehaviour remoteComponent;

        public override void OnNetworkSpawn()
        {
            RefreshNetworkRole();
            base.OnNetworkSpawn();
        }

        public override void OnGainedOwnership()
        {
            RefreshNetworkRole();
            base.OnGainedOwnership();
        }

        public override void OnLostOwnership()
        {
            RefreshNetworkRole();
            base.OnLostOwnership();
        }

        protected void RefreshNetworkRole()
        {
            if (serverComponent != null) serverComponent.enabled = IsServer;

            var isOwner = IsOwner;
            if (ownerComponent != null) ownerComponent.enabled = isOwner;
            if (remoteComponent != null) remoteComponent.enabled = !isOwner;
        }
    }
}