using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using Reflectis.SDK.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "AnotheReality/Utilities/TeleportPlayerScriptableAction", fileName = "TeleportPlayerScriptableAction")]
    public class TeleportPlayerScriptableAction : ActionScriptable
    {
        [SerializeField]
        private string hookID = "teleportTarget";

        public override void Action(Action completedCallback)
        {
            if (!InteractableObjectReference.InteractionTarget)
            {
                Debug.LogError("No interaction target");
                return;
            }

            Transform target = InteractableObjectReference.InteractionTarget.transform;

            var hook = InteractableObjectReference.InteractionTarget.GetComponentsInChildren<GenericHookComponent>(true).FirstOrDefault((x) => x.Id == hookID);
            
            if(hook != null)
            {
                target = hook.transform;
            }

            SM.GetSystem<IFadeSystem>().FadeToBlack(() =>
            {
                SM.GetSystem<ICharacterControllerSystem>().MoveCharacter(new Pose(target.position, target.rotation));
                SM.GetSystem<IFadeSystem>().FadeFromBlack();
            });
        }
    }
}
