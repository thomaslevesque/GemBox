using System.Windows;
using System.Windows.Controls;

namespace GemBox.WPF.Behaviors;

/// <summary>
/// Fournit des propriétés attachées pour ajouter des fonctionnalités aux contrôles PasswordBox
/// </summary>
public static class PasswordBoxBehavior
{
    /// <summary>
    /// Obtient la valeur de la propriété attachée BindPassword
    /// </summary>
    /// <param name="obj">L'objet pour lequel on veut obtenir la valeur</param>
    /// <returns>true si le binding du mot de passe est activé, sinon false</returns>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static bool GetBindPassword(PasswordBox obj)
    {
        return (bool)obj.GetValue(BindPasswordProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée BindPassword
    /// </summary>
    /// <param name="obj">L'objet pour lequel on veut définir la valeur</param>
    /// <param name="value">true pour activer le binding du mot de passe, sinon false</param>
    public static void SetBindPassword(PasswordBox obj, bool value)
    {
        obj.SetValue(BindPasswordProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée BindPassword
    /// </summary>
    public static readonly DependencyProperty BindPasswordProperty =
        DependencyProperty.RegisterAttached(
          "BindPassword",
          typeof(bool),
          typeof(PasswordBoxBehavior),
          new UIPropertyMetadata(
            false,
            BindPasswordChanged));

    private static void BindPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var pb = o as PasswordBox;
        if (pb == null)
            return;
        var oldValue = (bool)e.OldValue;
        var newValue = (bool)e.NewValue;
        if (oldValue && !newValue)
        {
            pb.PasswordChanged -= PasswordBoxPasswordChanged;
        }
        else if (newValue && !oldValue)
        {
            SetPassword(pb, pb.Password);
            pb.PasswordChanged += PasswordBoxPasswordChanged;
        }
    }

    /// <summary>
    /// Obtient la valeur de la propriété attachée Password
    /// </summary>
    /// <param name="obj">L'objet pour lequel on veut obtenir la valeur</param>
    /// <returns>Le mot de passe de cette PasswordBox</returns>
    [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
    public static string GetPassword(PasswordBox obj)
    {
        return (string)obj.GetValue(PasswordProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée Password
    /// </summary>
    /// <param name="obj">L'objet pour lequel on veut définir la valeur</param>
    /// <param name="value">Le nouveau mot de passe pour cette PasswordBox</param>
    public static void SetPassword(PasswordBox obj, string value)
    {
        obj.SetValue(PasswordProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée Password
    /// </summary>
    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
          "Password",
          typeof(string),
          typeof(PasswordBoxBehavior),
          new FrameworkPropertyMetadata(
            null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            PasswordChanged));

    private static void PasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var pb = o as PasswordBox;
        if (pb == null)
            return;

        var newValue = (string)e.NewValue;

        if (GetBindPassword(pb) && pb.Password != newValue)
            pb.Password = newValue;
    }

    static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        var pb = sender as PasswordBox;
        if (pb == null)
            return;
        SetPassword(pb, pb.Password);
    }
}
