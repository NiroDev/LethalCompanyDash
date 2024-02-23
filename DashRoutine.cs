using GameNetcodeStuff;
using System;
using System.Collections;
using UnityEngine;

namespace Dash
{
    internal class DashRoutine : MonoBehaviour
    {
        public static void StartRoutine(PlayerControllerB targetPlayer, Vector3 direction, float force, float duration = 0.5f, Action onComplete = null)
        {
            if (targetPlayer?.gameObject == null )
                return;

            var routine = targetPlayer.gameObject.AddComponent<DashRoutine>();
            routine.StartCoroutine(routine.Run(targetPlayer, direction, force, duration));
        }

        private IEnumerator Run(PlayerControllerB targetPlayer, Vector3 direction, float force, float duration)
        {
            float time = 0f;

            Vector3 startForce = direction * force;

            while (time < duration)
            {
                var sinusProgress = Mathf.Lerp(Mathf.PI / 2f, 0f, time / duration);
                var externalForce = startForce * sinusProgress;

                targetPlayer.externalForces = externalForce;

                time += Time.deltaTime;
                yield return null;
            }
            targetPlayer.externalForces = Vector3.zero;
        }
    }
}
