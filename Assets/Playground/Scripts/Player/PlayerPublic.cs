using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Playground.Player
{
    public class PlayerPublic : MonoBehaviour
    {
        [Header("Sound")]
        [SerializeField] private AudioClip landingAudioClip;
        [SerializeField] private AudioClip[] footstepAudioClips;
        [SerializeField] [Range(0, 1)] private float footstepAudioVolume = 0.5f;

        private readonly int animIDSpeed = Animator.StringToHash("Speed");
        private readonly int animIDGrounded = Animator.StringToHash("Grounded");
        private readonly int animIDJump = Animator.StringToHash("Jump");
        private readonly int animIDFreeFall = Animator.StringToHash("FreeFall");
        private readonly int animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        private Rigidbody rb;
        private Animator animator;
        private NetworkPlayer player;

        private bool isJumping = false;
        private bool prevGrounded = false;

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
            player = GetComponent<NetworkPlayer>();

            player.OnJump += OnJump;
        }

        private void Update()
        {
            if (!NetworkManager.Singleton.IsClient) return;
            
            JumpAndGravity();
            Move();
        }

        private void JumpAndGravity()
        {
            if (player.IsGrounded.Value)
            {
                animator.SetBool(animIDJump, false);
                animator.SetBool(animIDFreeFall, false);

                if (isJumping)
                {
                    animator.SetBool(animIDJump, true);
                }
            }
            else
            {
                animator.SetBool(animIDFreeFall, true);
            }
            animator.SetBool(animIDGrounded, player.IsGrounded.Value);
            isJumping = false;
        }

        private void Move()
        {
            var maxSpeed = player.config.moveForce / (rb.drag * rb.mass);
            var blendValue = player.Speed.Value / maxSpeed;

            var inputMag = player.MoveDirection.Value.magnitude;

            animator.SetFloat(animIDSpeed, blendValue * 6);
            animator.SetFloat(animIDMotionSpeed, inputMag);
        }

        private void OnJump()
        {
            if (rb.velocity.y < 0f) return;
            isJumping = true;
        }
      

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (footstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, footstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.position, footstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.position, footstepAudioVolume);
            }
        }
    }
}