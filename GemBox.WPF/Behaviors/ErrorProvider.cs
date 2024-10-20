using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GemBox.WPF.Controls;

namespace GemBox.WPF.Behaviors;

///<summary>
/// Fournit une indication visuelle d'une erreur de validation sur un contrôle
///</summary>
public static class ErrorProvider
{
    /// <summary>
    /// Obtient la valeur de la propriété attachée ErrorMessage pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on veut obtenir la valeur de la propriété</param>
    /// <returns>Le message d'erreur associé à ce contrôle</returns>
    public static string GetErrorMessage(this FrameworkElement obj)
    {
        return (string)obj.GetValue(ErrorMessageProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée ErrorMessage pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on définit la valeur de la propriété</param>
    /// <param name="value">Le message d'erreur à associer à ce contrôle</param>
    public static void SetErrorMessage(this FrameworkElement obj, string value)
    {
        obj.SetValue(ErrorMessageProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée ErrorMessage
    /// </summary>
    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.RegisterAttached(
            "ErrorMessage",
            typeof(string),
            typeof(ErrorProvider),
            new UIPropertyMetadata(
                null,
                ErrorMessageChanged));

    /// <summary>
    /// Obtient la valeur de la propriété attachée IconSource pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on veut obtenir la valeur de la propriété</param>
    /// <returns>L'icône d'erreur utilisée sur ce contrôle</returns>
    public static ImageSource GetIconSource(FrameworkElement obj)
    {
        return (ImageSource)obj.GetValue(IconSourceProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée IconSource pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on définit la valeur de la propriété</param>
    /// <param name="value">L'icône d'erreur à utiliser sur ce contrôle</param>
    public static void SetIconSource(FrameworkElement obj, ImageSource value)
    {
        obj.SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée IconSource
    /// </summary>
    public static readonly DependencyProperty IconSourceProperty =
        DependencyProperty.RegisterAttached(
            "IconSource",
            typeof(ImageSource),
            typeof(ErrorProvider),
            new FrameworkPropertyMetadata(GetDefaultErrorIcon(), FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Obtient la valeur de la propriété attachée IconHorizontalAlignment pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on veut obtenir la valeur de la propriété</param>
    /// <returns>L'alignement horizontal de l'icône d'erreur</returns>
    public static HorizontalAlignment GetIconHorizontalAlignment(FrameworkElement obj)
    {
        return (HorizontalAlignment)obj.GetValue(IconHorizontalAlignmentProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée IconHorizontalAlignment pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on définit la valeur de la propriété</param>
    /// <param name="value">L'alignement horizontal de l'icône d'erreur</param>
    public static void SetIconHorizontalAlignment(FrameworkElement obj, HorizontalAlignment value)
    {
        obj.SetValue(IconHorizontalAlignmentProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée IconHorizontalAlignment
    /// </summary>
    public static readonly DependencyProperty IconHorizontalAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "IconHorizontalAlignment",
            typeof(HorizontalAlignment),
            typeof(ErrorProvider),
            new FrameworkPropertyMetadata(
                HorizontalAlignment.Right,
                FrameworkPropertyMetadataOptions.Inherits));

    /// <summary>
    /// Obtient la valeur de la propriété attachée IconVerticalAlignment pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on veut obtenir la valeur de la propriété</param>
    /// <returns>L'alignement vertical de l'icône d'erreur</returns>
    public static VerticalAlignment GetIconVerticalAlignment(DependencyObject obj)
    {
        return (VerticalAlignment)obj.GetValue(IconVerticalAlignmentProperty);
    }

    /// <summary>
    /// Définit la valeur de la propriété attachée IconVerticalAlignment pour le contrôle
    /// </summary>
    /// <param name="obj">Contrôle pour lequel on définit la valeur de la propriété</param>
    /// <param name="value">L'alignement vertical de l'icône d'erreur</param>
    public static void SetIconVerticalAlignment(DependencyObject obj, VerticalAlignment value)
    {
        obj.SetValue(IconVerticalAlignmentProperty, value);
    }

    /// <summary>
    /// Identifiant de la propriété attachée IconVerticalAlignment
    /// </summary>
    public static readonly DependencyProperty IconVerticalAlignmentProperty =
        DependencyProperty.RegisterAttached(
            "IconVerticalAlignment",
            typeof(VerticalAlignment),
            typeof(ErrorProvider),
            new FrameworkPropertyMetadata(
                VerticalAlignment.Center,
                FrameworkPropertyMetadataOptions.Inherits));


    private static void ErrorMessageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var element = o as FrameworkElement;
        if (element == null)
            return;

        if (element is FormField)
            return;

        var oldValue = (string)e.OldValue;
        var newValue = (string)e.NewValue;

        if (!string.IsNullOrEmpty(oldValue) && string.IsNullOrEmpty(newValue))
        {
            element.DoWhenLoaded(RemoveErrorAdorner);
        }
        else if (!string.IsNullOrEmpty(newValue) && string.IsNullOrEmpty(oldValue))
        {
            element.DoWhenLoaded(AddErrorAdorner);
        }
    }

    private static void RemoveErrorAdorner(UIElement uiElement)
    {
        var layer = AdornerLayer.GetAdornerLayer(uiElement);
        if (layer == null)
            return;

        var adorners = layer.GetAdorners(uiElement);
        if (adorners == null)
            return;

        var adorner = adorners.OfType<ErrorAdorner>().FirstOrDefault();
        if (adorner != null)
            layer.Remove(adorner);
    }

    private static void AddErrorAdorner(UIElement uiElement)
    {
        var layer = AdornerLayer.GetAdornerLayer(uiElement);
        if (layer == null)
            return;

        var adorner = new ErrorAdorner(uiElement);
        layer.Add(adorner);
    }

    private static ImageSource GetDefaultErrorIcon()
    {
        return new BitmapImage(
            new Uri("pack://application:,,,/GemBox.WPF;component/Images/error.png"));
    }

    private class ErrorAdorner : Adorner
    {
        private readonly Border _errorBorder;

        public ErrorAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _errorBorder = new Border();
            _errorBorder.BorderThickness = new Thickness(2);
            _errorBorder.BorderBrush = Brushes.Red;
            Image img = new Image();
            img.Stretch = Stretch.None;
            BindToAdornedElement(img, Image.SourceProperty, IconSourceProperty);
            BindToAdornedElement(img, ToolTipProperty, ErrorMessageProperty);
            BindToAdornedElement(img, HorizontalAlignmentProperty, IconHorizontalAlignmentProperty);
            BindToAdornedElement(img, VerticalAlignmentProperty, IconVerticalAlignmentProperty);
            _errorBorder.Child = img;
            this.AddLogicalChild(_errorBorder);
            this.AddVisualChild(_errorBorder);
        }

        void BindToAdornedElement(FrameworkElement target, DependencyProperty targetProperty,
            DependencyProperty sourceProperty)
        {
            Binding binding = new Binding
            {
                Source = AdornedElement,
                Path = new PropertyPath(sourceProperty)
            };
            target.SetBinding(targetProperty, binding);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            AdornedElement.Measure(constraint);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _errorBorder.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _errorBorder;
            throw new ArgumentOutOfRangeException("index");
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get { yield return _errorBorder; }
        }
    }
}
