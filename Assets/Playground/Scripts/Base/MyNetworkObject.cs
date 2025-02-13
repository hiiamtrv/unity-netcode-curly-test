using Unity.Netcode;
using UnityEngine;

namespace Playground.Base
{
    public class MyNetworkObject : NetworkBehaviour
    {
        [Header("Owner/Server/Remote")] [SerializeField]
        protected GameObject ownerModule;

        [SerializeField] protected GameObject serverModule;
        [SerializeField] protected GameObject remoteModule;

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
            if (serverModule != null && IsServer)
            {
                var mdl = Instantiate(serverModule, transform);
                mdl.name = "_serverModule";
            }

            var isOwner = IsOwner;
            if (ownerModule != null && isOwner)
            {
                var mdl = Instantiate(ownerModule, transform);
                mdl.name = "_ownerModule";
            }

            if (remoteModule != null && !isOwner)
            {
                var mdl = Instantiate(remoteModule, transform);
                mdl.name = "_remoteModule";
            }
        }
    }
}