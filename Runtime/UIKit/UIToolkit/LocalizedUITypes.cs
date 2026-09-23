// ============================================================
// LocalizedUITypes.cs
//
// Belongs to Virtuademy.SDK.Environments.
//
// When I2Loc is NOT present (I2LOC undefined), this file
// provides stub versions of every LocalizedXxx element:
// the locKey attributes exist and are serializable, but no
// translation is applied at runtime.
//
// When I2Loc IS present (I2LOC defined), this file is excluded
// and LocalizedUITypesI2Loc.cs (Virtuademy.SDK.Environments.I2Loc
// assembly) provides the full versions with [TermsPopup] and
// live localization support.
// ============================================================
#if !I2LOC
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Virtuademy.LocalizedComponents
{

// ============================================================
// TEXT ELEMENTS
// ============================================================

[UxmlElement]
public partial class LocalizedLabel : Label
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedButton : Button
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedToggle : Toggle
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedRadioButton : RadioButton
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedFoldout : Foldout
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedGroupBox : GroupBox
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedProgressBar : ProgressBar
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

// ============================================================
// INPUT FIELDS
// ============================================================

[UxmlElement]
public partial class LocalizedTextField : TextField
{
    [UxmlAttribute] public string locKeyLabel { get => _kl; set => _kl = value; }
    [UxmlAttribute] public string locKeyPlaceholder { get => _kp; set => _kp = value; }
    string _kl, _kp;
}

[UxmlElement]
public partial class LocalizedIntegerField : IntegerField
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedFloatField : FloatField
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedLongField : LongField
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedDoubleField : DoubleField
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

// ============================================================
// SLIDERS
// ============================================================

[UxmlElement]
public partial class LocalizedSlider : Slider
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedSliderInt : SliderInt
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedMinMaxSlider : MinMaxSlider
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

// ============================================================
// VECTOR FIELDS
// ============================================================

[UxmlElement]
public partial class LocalizedVector2Field : Vector2Field
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedVector3Field : Vector3Field
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

[UxmlElement]
public partial class LocalizedVector4Field : Vector4Field
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

// ============================================================
// DROPDOWN / ENUM
// ============================================================

[UxmlElement]
public partial class LocalizedDropdownField : DropdownField
{
    [UxmlAttribute] public string locKeyLabel { get => _kl; set => _kl = value; }
    [UxmlAttribute] public string locKeyChoices { get => _kc; set => _kc = value; }
    string _kl, _kc;
}

[UxmlElement]
public partial class LocalizedEnumField : EnumField
{
    [UxmlAttribute] public string locKey { get => _k; set => _k = value; }
    string _k;
}

} // namespace Virtuademy.LocalizedComponents
#endif
