using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public interface IConnectable
    {
        /// <summary>
        /// Function to trigger the action of a connected object.
        /// </summary>
        void TriggerAction();

        /// <summary>
        /// Coroutine to start an action time based.
        /// </summary>
        void StartTimerAction(GameObject obj = null);

        IEnumerator StartTimerActionForDisableObject(GameObject obj = null);
    }
}
