using Reflectis.SDK.Interaction;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

using Photon.Pun;

using Virtuademy.Placeholders;

using static Virtuademy.Placeholders.InteractableOwnershipPlaceholder;

public class InteractableOwnershipMapper : MonoBehaviour, IRuntimeComponent
{
    public virtual Task Init(SceneComponentPlaceholderBase placeholder)
    {
        InteractableOwnershipPlaceholder ownershipPlaceholder = placeholder as InteractableOwnershipPlaceholder;


        ownershipPlaceholder.grabbableItem.gameObject.AddComponent<InteractableOwnershipManager>();
        ownershipPlaceholder.grabbableItem.gameObject.GetComponent<InteractableOwnershipManager>().SetOwnershipWithoutSync(ownershipPlaceholder.ownershipMask);

        return Task.CompletedTask;

    }
}
