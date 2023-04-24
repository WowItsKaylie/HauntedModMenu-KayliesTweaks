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

		/* original code for tracking the button press or something i think...........,,, sowwy :3
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

			if (canTrigger && hand.isLeftHand != leftHand) {
				triggered = true;
				handCollider = collider;

				GorillaTagger.Instance.StartVibration(hand.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);

				timerRoutine = StartCoroutine(Timer());
				HandTriggered();
			}	
		}
		*/

		// FIXME: none of the buttons can be pressed and the status of all mods gets inverted every time the menu is opened
		// i believe this is because we dont have any of the hand collider stuff here anymore but we cant use it since its not referenced at all now that ontriggerenter is no longer used
		
		void FixedUpdate()
        {
			EasyInput.UpdateInput();
			if (EasyInput.FaceButtonX)
            {
				
				if (triggered)
					return;
				triggered = true;
				timerRoutine = StartCoroutine(Timer());
				HandTriggered();
			}
        }

		private IEnumerator Timer()
		{
			yield return new WaitForSeconds(0.3f);
			triggered = false;
			timerRoutine = null;
		}

		protected virtual void HandTriggered() { }
	}
}
