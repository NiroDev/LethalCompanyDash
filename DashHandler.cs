using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Dash
{
    internal class DashHandler
    {
        public float lastDashedAt = 0f;

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

        internal void OnUpdate()
        {
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.wKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Forward);

            else if(Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.sKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Backward);

            else if(Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Left);

            else if(Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.dKey.wasReleasedThisFrame)
                HandleDashInDirection(Direction.Right);
        }

        internal void HandleDashInDirection(Direction direction)
        {
            var currentTime = Time.time;
            if (currentTime - lastDashedAt < Config.Instance.Cooldown.Value)
                return;

            var diff = currentTime - lastKeyChangeMap[direction];
            lastKeyChangeMap[direction] = currentTime;
            if (diff < Config.Instance.Precision.Value)
                dashProgressMap[direction]++;
            else
            {
                dashProgressMap[direction] = 0;
                return;
            }

            if (dashProgressMap[direction] >= 3)
                PerformDash(direction);
        }

        internal void PerformDash(Direction direction)
        {
            if (StartOfRound.Instance.localPlayerController == null)
                return;

            if (StartOfRound.Instance.localPlayerController.sprintMeter < Config.Instance.StaminaCost.Value)
                return;

            Vector3 directionalVector = StartOfRound.Instance.localPlayerController.gameplayCamera.transform.forward;
            directionalVector.y = 0f; // Don't throw me up

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
            DashRoutine.StartRoutine(StartOfRound.Instance.localPlayerController, directionalVector, Config.Instance.Power.Value, Config.Instance.Speed.Value);
            StartOfRound.Instance.localPlayerController.sprintMeter = Mathf.Clamp(StartOfRound.Instance.localPlayerController.sprintMeter - Config.Instance.StaminaCost.Value, 0f, 1f);
            lastDashedAt = Time.time;

            dashProgressMap[direction] = 0;
        }
    }
}
