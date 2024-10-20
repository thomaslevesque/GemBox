using System.Windows;
using System.Windows.Controls;

namespace GemBox.WPF.Controls;

///<summary>
/// Représente un champ de formulaire (contrôle FormView)
///</summary>
[TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Display", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Editor", Type = typeof(FrameworkElement))]
[StyleTypedProperty(Property = "HeaderContainerStyle", StyleTargetType = typeof(ContentPresenter))]
public class FormField : Control
{
    static FormField()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FormField),
                                                 new FrameworkPropertyMetadata(typeof(FormField)));
    }

    /// <summary>
    /// Identifiant de la propriété Header
    /// </summary>
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(FormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété EditorTemplate
    /// </summary>
    public static readonly DependencyProperty EditorTemplateProperty =
        DependencyProperty.Register(nameof(EditorTemplate), typeof(DataTemplate), typeof(FormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété DisplayTemplate
    /// </summary>
    public static readonly DependencyProperty DisplayTemplateProperty =
        DependencyProperty.Register(nameof(DisplayTemplate), typeof(DataTemplate), typeof(FormField), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété HeaderTemplate
    /// </summary>
    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(FormField), new PropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété HeaderStyle
    /// </summary>
    public static readonly DependencyProperty HeaderContainerStyleProperty =
        DependencyProperty.Register(nameof(HeaderContainerStyle), typeof(Style), typeof(FormField), new PropertyMetadata(null));

    /// <summary>
    /// Obtient ou définit l'en-tête du champ du formulaire.
    /// </summary>
    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le modèle à utiliser pour l'édition du champ
    /// </summary>
    public DataTemplate EditorTemplate
    {
        get => (DataTemplate)GetValue(EditorTemplateProperty);
        set => SetValue(EditorTemplateProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le modèle à utiliser pour l'affichage du champ
    /// </summary>
    public DataTemplate DisplayTemplate
    {
        get => (DataTemplate)GetValue(DisplayTemplateProperty);
        set => SetValue(DisplayTemplateProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le modèle à utiliser pour l'en-tête du champ
    /// </summary>
    public DataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Obtient ou définit le style à utiliser pour l'en-tête du champ
    /// </summary>
    public Style HeaderContainerStyle
    {
        get => (Style)GetValue(HeaderContainerStyleProperty);
        set => SetValue(HeaderContainerStyleProperty, value);
    }
}
