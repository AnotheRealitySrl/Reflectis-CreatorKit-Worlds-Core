using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{

    // T è il tipo di dato che ogni Toggle deve rappresentare (nel tuo caso, TenantDataSO)
    public class RadioToggleGroup<T> where T : class
    {
        private readonly List<Toggle> _toggles = new();
        private T _selectedValue;

        // Evento per notificare quando la selezione cambia
        public event Action<T> OnSelectionChanged;

        public T SelectedValue
        {
            get => _selectedValue;
            private set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnSelectionChanged?.Invoke(_selectedValue);
                }
            }
        }

        /// <summary>
        /// Registra un nuovo Toggle nel gruppo e imposta il suo valore iniziale.
        /// </summary>
        /// <param name="toggle">Il Toggle da aggiungere.</param>
        /// <param name="data">L'oggetto di tipo T associato a questo Toggle.</param>
        /// <param name="initialValue">Se il Toggle deve essere inizialmente selezionato.</param>
        public void Add(Toggle toggle, T data, bool initialValue)
        {
            // Memorizza l'oggetto dati nel Toggle
            toggle.userData = data;

            // Imposta lo stato iniziale (e aggiorna lo stato interno del gruppo se selezionato)
            toggle.value = initialValue;
            if (initialValue)
            {
                SelectedValue = data;
            }

            // Registra il callback di selezione
            toggle.RegisterValueChangedCallback(evt => OnToggleValueChanged(toggle, evt.newValue));

            _toggles.Add(toggle);
        }

        private void OnToggleValueChanged(Toggle sender, bool newValue)
        {
            if (newValue)
            {
                // 1. Aggiorna la variabile di stato e scatena l'evento
                SelectedValue = (T)sender.userData;

                // 2. Disattiva tutti gli altri toggle
                foreach (Toggle otherToggle in _toggles)
                {
                    if (otherToggle != sender)
                    {
                        // Impedisce un loop ricorsivo di callback:
                        // Quando disattivi un altro toggle, RegisterValueChangedCallback viene chiamato
                        // ma passa newValue=false, quindi non entra in questo 'if (newValue)'
                        otherToggle.value = false;
                    }
                }
            }
            else
            {
                // Se un toggle viene disattivato manualmente dall'utente e non da un altro toggle
                if (SelectedValue == (T)sender.userData)
                {
                    // In un radio button, la deselezione non dovrebbe essere consentita se non 
                    // dalla selezione di un altro elemento. 
                    // Per un Toggle singolo (checkbox), questa logica è utile, 
                    // ma per un Radio Group, potresti volerlo impedire.
                    // Lo lasciamo qui per permettere la deselezione totale, se desiderata.
                    SelectedValue = null;
                }
            }
        }
    }
}
