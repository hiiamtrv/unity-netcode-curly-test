using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Playground.Player
{
    public class PlayerServer : MonoBehaviour
    {
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private LayerMask groundLayers;

        private NetworkPlayer player;
        private Rigidbody rb;
        private bool prevGrounded;
        private bool canJump = true;
        private float force;

        private void OnEnable()
        {
            player = GetComponent<NetworkPlayer>();
            rb = GetComponent<Rigidbody>();

            player.OnJump += OnJump;
        }

        private void OnDisable()
        {
            player.OnJump -= OnJump;
        }

        private void Start()
        {
            if (player.Name.Value == default)
            {
                player.Name.Value = $"Newbie {Random.Range(11, 100)}";
            }
        }

        private void Update()
        {
            UpdateMovement();
            UpdateGround();
        }

        private void OnJump()
        {
            if (!canJump) return;
            rb.AddRelativeForce(0, player.config.jumpForce, 0, ForceMode.Impulse);
            canJump = false;
        }

        private void UpdateMovement()
        {
            //Update rotation
            var moveDirection = player.MoveDirection.Value;
            if (moveDirection.magnitude > 0.1f)
            {
                var targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                var offsetRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation,
                    player.config.rotateSpeed * Time.deltaTime);
                rb.MoveRotation(Quaternion.Euler(0f, offsetRotation, 0f));
                Debug.Log($"Rots {transform.eulerAngles.y} {targetRotation} {offsetRotation}");


                //Update position
                var lift = 0f;
                if (player.IsGrounded.Value)
                {
                    force = moveDirection.magnitude * player.config.moveForce;
                    lift = player.config.moveLiftForce;
                }

                rb.AddRelativeForce(0, lift, force);
            }

            //Calculate speed
            player.Speed.Value = rb.velocity.magnitude;
        }

        private void UpdateGround()
        {
            player.IsGrounded.Value = Physics.CheckSphere(
                transform.position,
                groundCheckRadius,
                groundLayers,
                QueryTriggerInteraction.Ignore
            );

            canJump |= (player.IsGrounded.Value && !prevGrounded);
            prevGrounded = player.IsGrounded.Value;
        }
    }
}