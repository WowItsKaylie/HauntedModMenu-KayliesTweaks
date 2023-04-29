using System.Collections;
using UnityEngine;
using HoneyLib.Utils;

namespace HMMKayliesTweaks.Buttons
{
	internal class HandTrigger : MonoBehaviour
	{
		protected bool triggered = false;
		protected Collider handCollider = null;

		protected static bool leftHand = true;
		protected static float handSensitivity = 1f;
		protected static Utils.ObjectTracker leftHandTracker = null;
		protected static Utils.ObjectTracker rightHandTracker = null;


		private Coroutine timerRoutine = null;


		protected virtual void Awake()
		{
			this.gameObject.layer = LayerMask.NameToLayer("GorillaInteractable");

			if(leftHandTracker == null)
				leftHandTracker = Utils.RefCache.LeftHandFollower?.AddComponent<Utils.ObjectTracker>();

			if(rightHandTracker == null)
				rightHandTracker = Utils.RefCache.RightHandFollower?.AddComponent<Utils.ObjectTracker>();
		}

		protected virtual void OnDisable()
		{
			triggered = false;
			if(timerRoutine != null)
				StopCoroutine(timerRoutine);
		}


		private void OnTriggerEnter(Collider collider)
		{
			if (triggered)
				return;

			GorillaTriggerColliderHandIndicator hand = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (hand == null)
				return;

			float lhSpeed = leftHandTracker != null ? leftHandTracker.Speed : 0f;
			float rhSpeed = rightHandTracker != null ? rightHandTracker.Speed : 0f;

			bool canTrigger = lhSpeed < handSensitivity && rhSpeed < handSensitivity;

			if (canTrigger && hand.isLeftHand != leftHand)
			{
				triggered = true;
				handCollider = collider;
				timerRoutine = StartCoroutine(Timer());
                
				GorillaTagger.Instance.StartVibration(hand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
				HandTriggered();
			}
		}

		private void FixedUpdate()
        {
			EasyInput.UpdateInput();
			if (EasyInput.FaceButtonX)
            {
				if (triggered) return;
				triggered = true;
				timerRoutine = StartCoroutine(TimerOpen());
				OpenMenu();
			}
        }
		private IEnumerator Timer()
		{
			yield return new WaitForSeconds(1.5f);
			triggered = false;
			timerRoutine = null;
		}

		private IEnumerator TimerOpen()
		{
			yield return new WaitForSeconds(0.5f);
			triggered = false;
			timerRoutine = null;
		}

		protected virtual void HandTriggered() { }
		protected virtual void OpenMenu() { }
	}
}
