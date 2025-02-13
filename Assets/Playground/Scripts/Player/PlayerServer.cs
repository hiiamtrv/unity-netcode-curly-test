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
        private float chargeDirection = 1;

        private void OnEnable()
        {
            player = GetComponentInParent<NetworkPlayer>();
            rb = GetComponentInParent<Rigidbody>();

            player.OnJump += OnJump;
            player.OnStartCharge += StartCharge;
            player.OnResetCharge += ResetCharge;
            player.OnReleaseCharge += ReleaseCharge;

            player.Name.Value = $"Newbie {Random.Range(11, 100)}";
            FreeFall();
        }

        private void OnDisable()
        {
            player.OnJump -= OnJump;
        }

        private void Update()
        {
            UpdateMovement();
            UpdateGround();
            UpdateCharge();
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
            var isCharging = player.ChargeValue.Value >= 0f;
            var moveDirection = player.MoveDirection.Value * (isCharging ? player.config.chargeSpeedEffect : 1f);
            if (moveDirection.magnitude > 0.1f)
            {
                if (rb.velocity.magnitude > 0.1f)
                {
                    var targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
                    var offsetRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation,
                        player.config.rotateSpeed * Time.deltaTime);
                    rb.MoveRotation(Quaternion.Euler(0f, offsetRotation, 0f));
                }

                //Update position
                var lift = 0f;
                if (player.IsGrounded.Value)
                {
                    force = moveDirection.magnitude * player.config.moveForce;
                    lift = player.config.moveLiftForce;
                }

                rb.AddRelativeForce(0, lift, force);
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
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

        private void UpdateCharge()
        {
            if (Mathf.Approximately(player.ChargeValue.Value, -1)) return;

            var targetValue = chargeDirection > 0 ? player.config.chargeMax : 0f;
            player.ChargeValue.Value = Mathf.MoveTowards(
                player.ChargeValue.Value,
                targetValue,
                player.config.chargeSpeed * Time.deltaTime
            );

            if (player.ChargeValue.Value == 0f) chargeDirection = 1;
            else if (Mathf.Approximately(player.ChargeValue.Value, player.config.chargeMax)) chargeDirection = -1;

            // Debug.Log($"Charge {player.ChargeValue.Value} {chargeDirection}");
        }

        private void ReleaseCharge()
        {
            var chargeForce = player.ChargeValue.Value;
            rb.AddRelativeForce(0, 0, chargeForce, ForceMode.Impulse);

            ResetCharge();
        }

        private void ResetCharge()
        {
            player.ChargeValue.Value = -1f;
        }

        private void StartCharge()
        {
            player.ChargeValue.Value = 0f;
            chargeDirection = 1;
        }

        public void FreeFall()
        {
            var dropZone = GameObject.FindGameObjectWithTag("DropZone");
            var dropZoneCollider = dropZone.GetComponent<Collider>();
            var boundMin = dropZoneCollider.bounds.min;
            var boundMax = dropZoneCollider.bounds.max;

            var spawnPos = new Vector3(
                Random.Range(boundMin.x, boundMax.x),
                Random.Range(boundMin.y, boundMax.y),
                Random.Range(boundMin.z, boundMax.z)
            );
            player.transform.position = spawnPos;
            rb.velocity = Vector3.zero;
        }
    }
}