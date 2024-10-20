using System.Windows;
using System.Windows.Controls;

namespace GemBox.WPF.Controls;

/// <summary>
/// Contrôle permettant d'afficher ou modifier les propriétés d'un objet sous forme d'un formulaire
/// </summary>
public class FormView : ItemsControl
{
    static FormView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FormView),
            new FrameworkPropertyMetadata(typeof(FormView)));
    }

    /// <summary>
    /// Initialise une nouvelle instance de FormView
    /// </summary>
    public FormView()
    {

    }

    /// <summary>
    /// Obtient ou définit une valeur qui indique si le formulaire est en mode édition
    /// </summary>
    public bool IsInEditMode
    {
        get => (bool)GetValue(IsInEditModeProperty);
        set => SetValue(IsInEditModeProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété IsInEditMode
    /// </summary>
    public static readonly DependencyProperty IsInEditModeProperty =
        DependencyProperty.Register(nameof(IsInEditMode), typeof(bool), typeof(FormView), new UIPropertyMetadata(false));
}
