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

		/*
		 this doesnt work atm
		 if i have GorillaTriggerColliderHandIndicator hand = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>(); in fixedupdate the menu doesnt open at all and it throws a NullReferenceException
		 if i dont it just inverts the status of every mod and breaks the buttons when opened
		 i think i need to use fixedupdate + easyinput and ontriggerenter in tandem or something idrk that much c#
		 this is very much wip dont laugh at me if my code sucks which it probably does lol
		 */

		private void OnTriggerEnter(Collider collider)
		{
			GorillaTriggerColliderHandIndicator hand = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (hand == null) 
				return;
			handCollider = collider;
		}
		
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
			yield return new WaitForSeconds(0.5f);
			triggered = false;
			timerRoutine = null;
		}

		protected virtual void HandTriggered() { }
	}
}
