using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GemBox.WPF.Controls;

/// <summary>
/// Représente un champ de formulaire lié aux données
/// </summary>
public abstract class BoundFormField : FormField
{
    private BindingBase _binding = new Binding();

    /// <summary>
    /// Obtient ou définit le Binding qui produit la valeur du champ
    /// </summary>
    public BindingBase Binding
    {
        get => _binding;
        set
        {
            _binding = value;
            SetBinding(ValueProperty, value);
        }
    }

    /// <summary>
    /// Obtient ou définit la valeur actuelle du champ
    /// </summary>
    /// <remarks>N'affectez pas explicitement une valeur à cette propriété;
    /// elle sera gérée automatiquement par la classe BoundFormField. Le seul
    /// cas où elle doit être utilisée est pour obtenir la valeur actuelle du
    /// champ dans les templates via un binding.</remarks>
    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le style à utiliser pour l'édition du champ
    /// </summary>
    public Style EditorStyle
    {
        get => (Style)GetValue(EditorStyleProperty);
        set => SetValue(EditorStyleProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le style à utiliser pour l'affichage du champ
    /// </summary>
    public Style DisplayStyle
    {
        get => (Style)GetValue(DisplayStyleProperty);
        set => SetValue(DisplayStyleProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété Value
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(BoundFormField),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    /// <summary>
    /// Identifiant de la propriété EditorStyle
    /// </summary>
    public static readonly DependencyProperty EditorStyleProperty =
        DependencyProperty.Register(nameof(EditorStyle), typeof(Style), typeof(BoundFormField), new PropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété DisplayStyle
    /// </summary>
    public static readonly DependencyProperty DisplayStyleProperty =
        DependencyProperty.Register(nameof(DisplayStyle), typeof(Style), typeof(BoundFormField), new PropertyMetadata(null));
}

/// <summary>
/// Représente un champ de formulaire qui affiche ou modifie une donnée textuelle
/// sous forme d'une boite de texte.
/// </summary>
[StyleTypedProperty(Property = "EditorStyle", StyleTargetType = typeof(TextBox))]
[StyleTypedProperty(Property = "DisplayStyle", StyleTargetType = typeof(TextBlock))]
public class TextFormField : BoundFormField
{
    static TextFormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(TextFormField),
            new FrameworkPropertyMetadata(typeof(TextFormField)));
    }
}

/// <summary>
/// Représente un champ de formulaire qui afficher ou modifie une donnée booléenne
/// sous forme d'une case à cocher.
/// </summary>
[StyleTypedProperty(Property = "EditorStyle", StyleTargetType = typeof(CheckBox))]
[StyleTypedProperty(Property = "DisplayStyle", StyleTargetType = typeof(CheckBox))]
public class CheckBoxFormField : BoundFormField
{
    static CheckBoxFormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckBoxFormField),
                                                 new FrameworkPropertyMetadata(typeof(CheckBoxFormField)));
    }

    /// <summary>
    /// Obtient ou définit une valeur indiquant si la case à cocher a 3 états (coché, non coché, indéterminé)
    /// </summary>
    public bool IsThreeState
    {
        get => (bool)GetValue(IsThreeStateProperty);
        set => SetValue(IsThreeStateProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété IsThreeState
    /// </summary>
    public static readonly DependencyProperty IsThreeStateProperty =
        DependencyProperty.Register(nameof(IsThreeState), typeof(bool), typeof(CheckBoxFormField), new UIPropertyMetadata(false));

}

/// <summary>
/// Représente un champ de formulaire qui affiche ou modifie une donnée à
/// choisir dans une liste, sous forme d'une liste déroulante.
/// </summary>
[StyleTypedProperty(Property = "EditorStyle", StyleTargetType = typeof(ComboBox))]
[StyleTypedProperty(Property = "DisplayStyle", StyleTargetType = typeof(ComboBox))]
public class ComboBoxFormField : BoundFormField
{
    static ComboBoxFormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxFormField),
                                                 new FrameworkPropertyMetadata(typeof(ComboBoxFormField)));
    }

    /// <summary>
    /// Obtient ou définit la liste des valeurs à afficher dans la liste déroulante.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété ItemsSource
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ComboBoxFormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Obtient ou définit le style des éléments de la liste déroulante
    /// </summary>
    public Style ItemContainerStyle
    {
        get => (Style)GetValue(ItemContainerStyleProperty);
        set => SetValue(ItemContainerStyleProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété ItemContainerStyle
    /// </summary>
    public static readonly DependencyProperty ItemContainerStyleProperty =
        DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(ComboBoxFormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Obtient ou définit le modèle pour les éléments de la liste déroulante
    /// </summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété ItemTemplate
    /// </summary>
    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ComboBoxFormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Obtient ou définit le membre à afficher pour les éléments de la liste déroulante
    /// </summary>
    public string DisplayMemberPath
    {
        get => (string)GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété DisplayMemberPath
    /// </summary>
    public static readonly DependencyProperty DisplayMemberPathProperty =
        DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(ComboBoxFormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Obtient ou définit le membre à utiliser comme valeur sélectionnée pour les éléments de la liste déroulante
    /// </summary>
    public string SelectedValuePath
    {
        get => (string)GetValue(SelectedValuePathProperty);
        set => SetValue(SelectedValuePathProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété SelectedValuePath
    /// </summary>
    public static readonly DependencyProperty SelectedValuePathProperty =
        DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(ComboBoxFormField), new PropertyMetadata(null));

}

/// <summary>
/// Représente un champ de formulaire qui affiche ou modifie un mot de passe.
/// </summary>
[StyleTypedProperty(Property = "EditorStyle", StyleTargetType = typeof(PasswordBox))]
[StyleTypedProperty(Property = "DisplayStyle", StyleTargetType = typeof(PasswordBox))]
public class PasswordBoxFormField : BoundFormField
{
    static PasswordBoxFormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordBoxFormField),
                                                 new FrameworkPropertyMetadata(typeof(PasswordBoxFormField)));
    }
}

public class DateFormField : BoundFormField
{
    static DateFormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(DateFormField),
            new FrameworkPropertyMetadata(typeof(DateFormField)));
    }
}
