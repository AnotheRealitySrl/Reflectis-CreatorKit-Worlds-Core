using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [AddComponentMenu("")]//For hide from add component menu.
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Variables))]
    public class SyncedVariables : MonoBehaviour
    {
        public override string ToString()
        {
            //Data.ToString();
            string value = "";
            foreach (Data data in variableSettings)
            {
                if (data.isSynced)
                {
                    value = value + data.ToString() + "\n";
                }
            }
            return value;
        }

        [System.Serializable]
        public class Data
        {
            public override string ToString()
            {
                string value = "name: " + name + "value: " + DeclarationValue + " { hasChanged: " + hasChanged + " + AreSynced: " + AreValuesSynced + " }";
                return value;
            }

            [HideInInspector]
            public byte id;
            public string name;
            public bool saveThroughSessions;
            /// <summary>
            /// Keeps track of wheter or not the value has been changed on the network
            /// if the variable is not synced it is always false
            /// </summary>
            public bool hasChanged = false;
            /// <summary>
            /// The value of the variable used to check if the variables in VS are changed
            /// it is used to check if we have to invoke event nodes
            /// </summary>
            public object previousValue;
            /// <summary>
            /// Whter or not the variable is sinchronized on network
            /// </summary>
            [HideInInspector]
            public bool isSynced;
            /// <summary>
            /// The VS related variable
            /// </summary>
            public VariableDeclaration declaration { get; set; }

            public object DeclarationValue
            {
                get
                {
                    return declaration.value;
                }
            }

            public bool AreValuesSynced { get { return previousValue.Equals(DeclarationValue); } }

            public void SyncValues()
            {
                previousValue = DeclarationValue;
                // if the synced variable is not synced we do not update the sync change state in the dictionary
                hasChanged = isSynced;
            }
        }

        public List<Data> variableSettings = new List<Data>();
        //public Dictionary<string, int> variableDictLock = new Dictionary<string, int>();

        public void VariableSet()
        {
            foreach (Data data in variableSettings)
            {
                if (data.declaration == null)
                {
                    var declarations = GetComponentInChildren<Variables>(true).declarations;
                    data.declaration = declarations.GetDeclaration(data.name);
                }
                data.previousValue = data.DeclarationValue;
            }
        }

    }
}
