// ============================================================
// LocalizedUITypesI2Loc.cs
//
// Belongs to Virtuademy.SDK.Environments.I2Loc — compiled only when
// I2LOC is defined (I2Loc package present in the project).
//
// Provides the full localized versions of every UI Toolkit
// element with [TermsPopup] for Inspector key selection and
// live locale-change subscriptions via I2Loc events.
// ============================================================
using I2.Loc;
using static I2.Loc.LocalizationManager;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Virtuademy.LocalizedComponents
{

// ============================================================
// LocalizationHelper — shared attach/detach + translate logic
// ============================================================
public static class LocalizationHelper
{
    public static void Setup(VisualElement el, OnLocalizeCallback updateAction)
    {
        el.RegisterCallback<AttachToPanelEvent>(_ =>
        {
            updateAction();
            OnLocalizeEvent += updateAction;
        });
        el.RegisterCallback<DetachFromPanelEvent>(_ =>
        {
            OnLocalizeEvent -= updateAction;
        });
    }

    public static string Translate(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        return LocalizationManager.GetTranslation(key);
    }
}

// ============================================================
// TEXT ELEMENTS
// ============================================================

[UxmlElement]
public partial class LocalizedLabel : Label
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedLabel() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedButton : Button
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedButton() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedToggle : Toggle
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedToggle() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedRadioButton : RadioButton
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedRadioButton() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedFoldout : Foldout
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedFoldout() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedGroupBox : GroupBox
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedGroupBox() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) text = t; }
}

[UxmlElement]
public partial class LocalizedProgressBar : ProgressBar
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
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
    [UxmlAttribute] [TermsPopup] public string locKeyLabel { get => _kl; set { _kl = value; Apply(); } }
    [UxmlAttribute] [TermsPopup] public string locKeyPlaceholder { get => _kp; set { _kp = value; Apply(); } }
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
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedIntegerField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedFloatField : FloatField
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedFloatField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedLongField : LongField
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedLongField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedDoubleField : DoubleField
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
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
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedSlider() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedSliderInt : SliderInt
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedSliderInt() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedMinMaxSlider : MinMaxSlider
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
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
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedVector2Field() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedVector3Field : Vector3Field
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedVector3Field() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

[UxmlElement]
public partial class LocalizedVector4Field : Vector4Field
{
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
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
    [UxmlAttribute] [TermsPopup] public string locKeyLabel { get => _kl; set { _kl = value; Apply(); } }
    [UxmlAttribute] [TermsPopup] public string locKeyChoices { get => _kc; set { _kc = value; Apply(); } }
    string _kl, _kc;

    public LocalizedDropdownField() => LocalizationHelper.Setup(this, Apply);

    void Apply()
    {
        var tl = LocalizationHelper.Translate(_kl);
        if (tl != null) label = tl;

        if (!string.IsNullOrEmpty(_kc))
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
    [UxmlAttribute] [TermsPopup] public string locKey { get => _k; set { _k = value; Apply(); } }
    string _k;
    public LocalizedEnumField() => LocalizationHelper.Setup(this, Apply);
    void Apply() { var t = LocalizationHelper.Translate(_k); if (t != null) label = t; }
}

} // namespace Virtuademy.LocalizedComponents
