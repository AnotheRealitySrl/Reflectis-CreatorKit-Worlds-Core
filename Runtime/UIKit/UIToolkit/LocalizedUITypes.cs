// ============================================================
// LocalizedUITypes.cs
//
// Belongs to Virtuademy.SDK.Environments.
//
// The one and only definition of every LocalizedXxx UI Toolkit
// element. It has no dependency on any localization package, so
// the element types are identical in a creator project (no I2Loc)
// and in the Virtuademy app (I2Loc present): a UXML serialized in
// one resolves in the other, including UXML shipped in Addressables
// bundles.
//
// Translation is delegated to an ILocalizedTextProvider registered
// on LocalizationHelper. Without a provider the elements keep the
// text authored in the UXML and only store their keys. With I2Loc
// installed, I2LocalizedTextProvider (Virtuademy.SDK.Environments.I2Loc
// assembly) registers itself and the elements translate live.
// ============================================================
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace Virtuademy.LocalizedComponents
{

// ============================================================
// Localization contract
// ============================================================

/// <summary>
/// Translates localization keys for the LocalizedXxx elements. Implemented by the localization
/// backend (e.g. I2Loc) and registered through <see cref="LocalizationHelper.SetProvider"/>.
/// </summary>
public interface ILocalizedTextProvider
{
    /// <summary>Returns the translation of <paramref name="key"/>, or null when it has none.</summary>
    string Translate(string key);
}

/// <summary>
/// Marks a string as a localization key. Without a localization backend it is drawn as a plain
/// text field; the I2Loc editor assembly draws it as a dropdown of I2 terms.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LocalizationTermAttribute : PropertyAttribute
{
}

// ============================================================
// LocalizationHelper — shared attach/detach + translate logic
// ============================================================
public static class LocalizationHelper
{
    private static ILocalizedTextProvider provider;

    /// <summary>Raised when the language changes or a provider is (un)registered.</summary>
    public static event Action LanguageChanged;

    public static ILocalizedTextProvider Provider => provider;

    /// <summary>Registers the translation backend (null to remove it) and refreshes every attached element.</summary>
    public static void SetProvider(ILocalizedTextProvider newProvider)
    {
        provider = newProvider;
        NotifyLanguageChanged();
    }

    /// <summary>Called by the backend when the current language changes.</summary>
    public static void NotifyLanguageChanged() => LanguageChanged?.Invoke();

    public static void Setup(VisualElement el, Action updateAction)
    {
        el.RegisterCallback<AttachToPanelEvent>(_ =>
        {
            updateAction();
            LanguageChanged -= updateAction;
            LanguageChanged += updateAction;
        });
        el.RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            LanguageChanged -= updateAction;
        });
    }

    /// <summary>Returns the translation of <paramref name="key"/>, or null when there is no key, no provider or no translation.</summary>
    public static string Translate(string key)
    {
        if (string.IsNullOrEmpty(key) || provider == null) return null;
        try
        {
            string translated = provider.Translate(key);
            return string.IsNullOrEmpty(translated) ? null : translated;
        }
        catch (Exception e)
        {
            // The backend may not be ready yet (e.g. UI Builder preview): keep the authored text.
            Debug.LogWarning($"[{nameof(LocalizationHelper)}] Could not translate '{key}': {e.Message}");
            return null;
        }
    }
}

// ============================================================
// TEXT ELEMENTS
// ============================================================

[UxmlElement]
public partial class LocalizedLabel : Label
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedLabel() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedButton : Button
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedButton() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedToggle : Toggle
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedToggle() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedRadioButton : RadioButton
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedRadioButton() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedFoldout : Foldout
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedFoldout() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedGroupBox : GroupBox
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedGroupBox() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedProgressBar : ProgressBar
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedProgressBar() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) title = t; }
}

// ============================================================
// INPUT FIELDS
// ============================================================

[UxmlElement]
public partial class LocalizedTextField : TextField
{
    [UxmlAttribute] [LocalizationTerm] public string locKeyLabel { get => _kl; set { _kl = value; Apply(); } }
    [UxmlAttribute] [LocalizationTerm] public string locKeyPlaceholder { get => _kp; set { _kp = value; Apply(); } }
    string _kl, _kp;
    public LocalizedTextField() => LocalizationHelper.Setup(this, Apply);
    void Apply()
    {
        var tl = LocalizationHelper.Translate(_kl); if (tl != null) label = tl;
        var tp = LocalizationHelper.Translate(_kp); if (tp != null) textEdition.placeholder = tp;
    }
}

[UxmlElement]
public partial class LocalizedIntegerField : IntegerField
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedIntegerField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedFloatField : FloatField
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedFloatField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedLongField : LongField
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedLongField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedDoubleField : DoubleField
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedDoubleField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

// ============================================================
// SLIDERS
// ============================================================

[UxmlElement]
public partial class LocalizedSlider : Slider
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedSlider() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedSliderInt : SliderInt
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedSliderInt() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedMinMaxSlider : MinMaxSlider
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedMinMaxSlider() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

// ============================================================
// VECTOR FIELDS
// ============================================================

[UxmlElement]
public partial class LocalizedVector2Field : Vector2Field
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedVector2Field() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedVector3Field : Vector3Field
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedVector3Field() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedVector4Field : Vector4Field
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedVector4Field() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

// ============================================================
// DROPDOWN / ENUM
// ============================================================

[UxmlElement]
public partial class LocalizedDropdownField : DropdownField
{
    [UxmlAttribute] [LocalizationTerm] public string locKeyLabel { get => _kl; set { _kl = value; Apply(); } }
    // Comma-separated list of keys, one per choice: kept as plain text (a single term dropdown does not fit).
    [UxmlAttribute] public string locKeyChoices { get => _kc; set { _kc = value; Apply(); } }
    string _kl, _kc;

    public LocalizedDropdownField() => LocalizationHelper.Setup(this, Apply);

    void Apply()
    {
        var tl = LocalizationHelper.Translate(_kl);
        if (tl != null) label = tl;

        // Without a provider the choices authored in the UXML are left untouched.
        if (!string.IsNullOrEmpty(_kc) && LocalizationHelper.Provider != null)
        {
            var keys = _kc.Split(',');
            var translated = new List<string>();
            foreach (var k in keys)
            {
                var t = LocalizationHelper.Translate(k.Trim());
                translated.Add(t ?? k.Trim());
            }
            choices = translated;
        }
    }
}

[UxmlElement]
public partial class LocalizedEnumField : EnumField
{
    [UxmlAttribute] [LocalizationTerm] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedEnumField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

} // namespace Virtuademy.LocalizedComponents
