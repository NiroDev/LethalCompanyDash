using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dash
{
    internal class DashHandler
    {
        public float lastDashedAt = 0f;

        private bool DashKeyRegistered = false;

        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right
        }

        internal Dictionary<Direction, int> dashProgressMap = new Dictionary<Direction, int>
        {
            { Direction.Forward, 0 },
            { Direction.Backward, 0 },
            { Direction.Left, 0 },
            { Direction.Right, 0 }
        };

        internal Dictionary<Direction, float> lastKeyChangeMap = new Dictionary<Direction, float>
        {
            { Direction.Forward, 0f },
            { Direction.Backward, 0f },
            { Direction.Left, 0f },
            { Direction.Right, 0f }
        };

        public void RegisterDashKey()
        {
            if(DashKeyRegistered)
            {
                if (!InputUtilsCompat.Enabled || !InputUtilsCompat.UseDashKey)
                {
                    InputUtilsCompat.DashKey.performed -= OnDashKeyPressed;
                    DashKeyRegistered = false;
                }
            }
            else
            {
                if (InputUtilsCompat.Enabled && InputUtilsCompat.UseDashKey)
                {
                    InputUtilsCompat.DashKey.performed += OnDashKeyPressed;
                    DashKeyRegistered = true;
                }
            }
        }

        internal void OnUpdate()
        {
            if (!FulfillsDashConditions()) return;

            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.wKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Forward);

            else if(Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.sKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Backward);

            else if(Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Left);

            else if(Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.dKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Right);
        }

        internal void OnDashKeyPressed(InputAction.CallbackContext dashContext)
        {
            if (!FulfillsDashConditions()) return;
            
            PerformDash(StartOfRound.Instance.localPlayerController.gameplayCamera.transform.forward);
        }

        internal bool FulfillsDashConditions()
        {
            if (StartOfRound.Instance.localPlayerController == null)
                return false;

            if (!Config.Instance.Enabled.Value)
                return false;

            if (Config.Instance.ToSize.Value > 0f)
            {
                var playerSize = StartOfRound.Instance.localPlayerController.gameObject.transform.localScale.y;
                if (playerSize < Config.Instance.FromSize.Value || playerSize > Config.Instance.ToSize.Value)
                    return false;
            }

            if (StartOfRound.Instance.localPlayerController.sprintMeter < Config.Instance.StaminaCost.Value)
                return false;

            if (Time.time - lastDashedAt < Config.Instance.Cooldown.Value)
                return false;

            return true;
        }

        internal void HandleDashInDirection(Direction direction)
        {
            var diff = Time.time - lastKeyChangeMap[direction];
            lastKeyChangeMap[direction] = Time.time;
            if (diff < Config.Instance.Precision.Value)
                dashProgressMap[direction]++;
            else
            {
                dashProgressMap[direction] = 0;
                return;
            }

            if (dashProgressMap[direction] >= 3)
            {
                PerformDash(direction);
                dashProgressMap[direction] = 0;
            }
        }

        internal void PerformDash(Direction direction)
        {
            Vector3 directionalVector = StartOfRound.Instance.localPlayerController.gameplayCamera.transform.forward;

            switch(direction)
            {
                case Direction.Backward:
                    directionalVector *= -1;
                    break;
                case Direction.Left:
                    directionalVector = Quaternion.Euler(0, -90, 0) * directionalVector;
                    break;
                case Direction.Right:
                    directionalVector = Quaternion.Euler(0, 90, 0) * directionalVector;
                    break;
                default: break; // Forward already correct
            }

            // Perform dash
            PerformDash(directionalVector);
        }

        internal void PerformDash(Vector3 direction)
        {
            direction.y = 0f; // Don't throw us up
            DashRoutine.StartRoutine(StartOfRound.Instance.localPlayerController, direction, Config.Instance.Power.Value, Config.Instance.Speed.Value);
            StartOfRound.Instance.localPlayerController.sprintMeter = Mathf.Clamp(StartOfRound.Instance.localPlayerController.sprintMeter - Config.Instance.StaminaCost.Value, 0f, 1f);
            lastDashedAt = Time.time;
        }
    }
}
