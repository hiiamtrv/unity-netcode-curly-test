using UnityEngine;
using UnityEngine.Serialization;

namespace Playground.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Playground/PlayerConfig", order = 0)]
    public class NetworkPlayerConfig : ScriptableObject
    {
        public float moveForce;
        public float moveLiftForce;
        public float rotateSpeed;
        public float jumpForce;
    }
}