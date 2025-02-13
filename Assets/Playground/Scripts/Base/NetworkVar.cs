using Unity.Netcode;

namespace Playground.Base
{
    public static class NetworkVar
    {
        public static NetworkVariable<T> Make<T>(bool publicRead = true, bool ownerWrite = false, T initValue = default)
        {
            return new NetworkVariable<T>(
                initValue,
                publicRead ? NetworkVariableReadPermission.Everyone : NetworkVariableReadPermission.Owner,
                ownerWrite ? NetworkVariableWritePermission.Owner : NetworkVariableWritePermission.Server
            );
        }
    }
}