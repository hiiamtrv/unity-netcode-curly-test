using Playground.UI;
using Unity.Collections;
using UnityEngine;

namespace Playground.Player
{
    public class PlayerOwner : MonoBehaviour
    {
        private PlayerInputAction playerInput;
        private NetworkPlayer player;
        private Camera mainCamera;
        private NameUI nameUI;

        private Vector2 lookDirection;

        [SerializeField] private float cameraDistance;
        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private Color ownerColor;

        private Color[] defaultColors;
        private bool cursorLocked;


        private void OnEnable()
        {
            mainCamera = Camera.main;
            playerInput = new PlayerInputAction();
            player = GetComponentInParent<NetworkPlayer>();
            nameUI = FindObjectOfType<NameUI>();

            player.Name.OnValueChanged += OnNameChanged;
            nameUI.OnConfirmName += OnConfirmName;
            nameUI.OnCancel += OnCancelName;

            playerInput.Enable();
            cursorLocked = true;

            defaultColors = new Color[skinnedMeshRenderer.materials.Length];
            for (var i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                defaultColors[i] = skinnedMeshRenderer.materials[i].color;
                skinnedMeshRenderer.materials[i].color = ownerColor;
            }
        }

        private void OnDisable()
        {
            playerInput.Disable();
            Cursor.lockState = CursorLockMode.None;

            for (var i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                skinnedMeshRenderer.materials[i].color = defaultColors[i];
            }
        }

        private void Update()
        {
            if (playerInput.Player.Look.triggered)
            {
                var rInput = playerInput.Player.Look.ReadValue<Vector2>();
                lookDirection += rInput;

                AuditLookDirection(ref lookDirection);
            }

            var mInput = playerInput.Player.Move.ReadValue<Vector2>();
            var rot = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
            var move = rot * new Vector3(mInput.x, 0, mInput.y);
            player.MoveDirection.Value = new Vector2(move.x, move.z);

            UpdateCameraRotation();

            if (playerInput.Player.ToogleCursor.triggered)
            {
                if (Application.isFocused)
                {
                    cursorLocked = !cursorLocked;
                }
            }

            if (playerInput.Player.Jump.triggered && player.IsGrounded.Value)
            {
                player.RequestJumpServerRpc();
            }

            if (playerInput.Player.PrimaryAction.WasPressedThisFrame())
            {
                player.RequestPrimaryActionServerRpc(false);
            }
            else if (playerInput.Player.PrimaryAction.WasReleasedThisFrame())
            {
                player.RequestPrimaryActionServerRpc(true);
            }
            else if (playerInput.Player.SecondaryAction.WasPressedThisFrame())
            {
                player.RequestSecondaryActionServerRpc();
            }

            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void UpdateCameraRotation()
        {
            //Rotate Camera
            var forward = Quaternion.Euler(lookDirection.y, lookDirection.x, 0.0f).normalized * Vector3.back;
            var cameraPos = forward * cameraDistance + transform.position;
            mainCamera.transform.position = cameraPos;
            mainCamera.transform.LookAt(player.transform.position + cameraOffset);
        }

        private static void AuditLookDirection(ref Vector2 lookDirection)
        {
            lookDirection.y = Mathf.Clamp(lookDirection.y, -5.0f, 60.0f);
        }

        private void OnConfirmName(string newName)
        {
            if (newName.Length > FixedString64Bytes.UTF8MaxLengthInBytes)
            {
                OnCancelName();
            }

            player.Name.Value = newName;
        }

        private void OnCancelName()
        {
            nameUI?.SetName(player.Name.Value.Value);
        }

        private void OnNameChanged(FixedString64Bytes prev, FixedString64Bytes curr)
        {
            nameUI?.SetName(curr.Value);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            cursorLocked = hasFocus;
        }
    }
}