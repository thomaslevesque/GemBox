using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GemBox.WPF.Controls;

/// <summary>
/// Represents a control for rating something.
/// </summary>
[TemplatePart(Name = "PART_RatingHost", Type = typeof(FrameworkElement))]
public class RatingControl : Control
{
    #region Private data

    private FrameworkElement? _ratingHost;

    #endregion

    #region Constructors

    static RatingControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RatingControl), new FrameworkPropertyMetadata(typeof(RatingControl)));
    }

    #endregion

    #region Dependency properties

    /// <summary>
    /// Identifiant de la propriété ImageOn
    /// </summary>
    public static readonly DependencyProperty ImageOnProperty =
        DependencyProperty.Register(nameof(ImageOn), typeof(ImageSource), typeof(RatingControl), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété ImageOff
    /// </summary>
    public static readonly DependencyProperty ImageOffProperty =
        DependencyProperty.Register(nameof(ImageOff), typeof(ImageSource), typeof(RatingControl), new UIPropertyMetadata(null));

    /// <summary>
    /// Identifiant de la propriété Stretch
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(RatingControl), new UIPropertyMetadata(Stretch.Uniform));

    /// <summary>
    /// Identifiant de la propriété Value
    /// </summary>
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(RatingControl),
            new FrameworkPropertyMetadata(
                0.0,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceRatingValue));

    /// <summary>
    /// Identifiant de la propriété RatingMode
    /// </summary>
    public static readonly DependencyProperty RatingModeProperty =
        DependencyProperty.Register(
            nameof(RatingMode),
            typeof(RatingMode),
            typeof(RatingControl),
            new UIPropertyMetadata(
                RatingMode.Integer,
                OnRatingModeChanged));

    /// <summary>
    /// Identifiant de la propriété IsReadOnly
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(RatingControl), new UIPropertyMetadata(false));

    public static readonly DependencyPropertyKey HoverValuePropertyKey =
        DependencyProperty.RegisterReadOnly("HoverValue", typeof(double), typeof(RatingControl), new UIPropertyMetadata(0.0));

    #endregion

    #region Routed events

    /// <summary>
    /// Identifiant de l'évènement ValueChanged
    /// </summary>
    public static readonly RoutedEvent ValueChangedEvent =
        EventManager.RegisterRoutedEvent(
            "ValueChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>),
            typeof(RatingControl));

    #endregion

    #region Public properties

    /// <summary>
    /// Obtient ou définit l'image utilisée pour afficher les points "allumés"
    /// </summary>
    public ImageSource ImageOn
    {
        get => (ImageSource)GetValue(ImageOnProperty);
        set => SetValue(ImageOnProperty, value);
    }

    /// <summary>
    /// Obtient ou définit l'image utilisée pour afficher les points "éteints"
    /// </summary>
    public ImageSource ImageOff
    {
        get => (ImageSource)GetValue(ImageOffProperty);
        set => SetValue(ImageOffProperty, value);
    }

    /// <summary>
    /// Obtient ou définit une valeur indiquant comment le contrôle doit être étiré
    /// pour remplir le rectangle de destination.
    /// </summary>
    /// <value>Une des valeurs de Stretch. La valeur par défaut est Uniform.</value>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Obtient ou définit la valeur courante du contrôle
    /// </summary>
    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Obtient ou définit l'aperçu de la valeur quand la souris survole le contrôle
    /// </summary>
    /// <remarks>Cette propriété est utile pour définir un template personnalisé pour le contrôle</remarks>
    public double HoverValue => (double)GetValue(HoverValuePropertyKey.DependencyProperty);

    /// <summary>
    /// Obtient ou définit le mode de notation (entier, demi-point ou décimal)
    /// </summary>
    public RatingMode RatingMode
    {
        get => (RatingMode)GetValue(RatingModeProperty);
        set => SetValue(RatingModeProperty, value);
    }

    /// <summary>
    /// Obtient ou définit une valeur indiquant si le contrôle est en lecture seule.
    /// </summary>
    /// <value>true si le contrôle est en lecture seule, false sinon. La valeur par défaut est false.</value>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    #endregion

    #region Public events

    /// <summary>
    /// Se produit quand la valeur change
    /// </summary>
    public event RoutedPropertyChangedEventHandler<double> ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    #endregion

    #region Private methods

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = d as RatingControl;
        if (ctl == null)
            return;

        double oldValue = (double)e.OldValue;
        double newValue = ctl.Value;
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (oldValue != newValue)
        {
            var args = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent);
            ctl.OnValueChanged(args);
        }
    }

    private static object CoerceRatingValue(DependencyObject d, object basevalue)
    {
        var ctl = d as RatingControl;
        if (ctl == null)
            return basevalue;

        double value = (double)basevalue;
        return ctl.CoerceRatingValue(value);
    }

    private static void OnRatingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = d as RatingControl;
        if (ctl == null)
            return;

        ctl.CoerceValue(ValueProperty);
    }

    private double CoerceRatingValue(double value)
    {
        switch (RatingMode)
        {
            case RatingMode.Integer:
                value = Math.Ceiling(value);
                break;
            case RatingMode.HalfPoint:
                value = Math.Round(value * 2) / 2;
                break;
        }
        return value;
    }

    private double GetValue(MouseEventArgs e)
    {
        if (_ratingHost is null)
            return 0;
        double pos = e.GetPosition(_ratingHost).X;
        double totalWidth = _ratingHost.ActualWidth;
        if (pos < 0)
            pos = 0;
        if (pos > totalWidth)
            pos = totalWidth;
        double value = 5 * pos / totalWidth;
        return value;
    }

    void _ratingHost_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsReadOnly)
        {
            SetValue(ValueProperty, CoerceRatingValue(GetValue(e)));
        }
    }

    void _ratingHost_MouseMove(object sender, MouseEventArgs e)
    {
        if (!IsReadOnly)
        {
            SetValue(HoverValuePropertyKey, CoerceRatingValue(GetValue(e)));
        }
    }

    private void SubscribeEvents(FrameworkElement ratingHost)
    {
        ratingHost.MouseDown += _ratingHost_MouseDown;
        ratingHost.MouseMove += _ratingHost_MouseMove;
    }

    private void UnsubscribeEvents(FrameworkElement ratingHost)
    {
        ratingHost.MouseDown -= _ratingHost_MouseDown;
        ratingHost.MouseMove -= _ratingHost_MouseMove;
    }

    #endregion

    #region Virtual methods and overrides

    /// <summary>
    /// Génère l'arbre visuel pour le contrôle lorsqu'un nouveau modèle est appliqué.
    /// </summary>
    public override void OnApplyTemplate()
    {
        if (_ratingHost != null)
        {
            UnsubscribeEvents(_ratingHost);
        }

        base.OnApplyTemplate();

        _ratingHost = Template.FindName("PART_RatingHost", this) as FrameworkElement;
        if (_ratingHost != null)
        {
            SubscribeEvents(_ratingHost);
        }

    }

    /// <summary>
    /// Déclenche l'évènement ValueChanged
    /// </summary>
    /// <param name="e">Paramètres de l'évènement</param>
    protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> e)
    {
        this.RaiseEvent(e);
    }

    #endregion

    #region RatingWidthConverter

    /// <summary>
    /// Convertisseur utilisé pour calculer la largeur de la zone "allumée"
    /// </summary>
    public static readonly IMultiValueConverter WidthConverter = new RatingWidthConverter();

    private class RatingWidthConverter : IMultiValueConverter
    {
        private const double MaxValue = 5;

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2
                && values[0] is double && values[1] is double)
            {
                double rating = (double) values[0];
                double totalActualWidth = (double)values[1];
                return rating * totalActualWidth / MaxValue;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    #endregion
}

/// <summary>
/// Indique le mode de notation utilisée
/// </summary>
public enum RatingMode
{
    /// <summary>
    /// La note est arrondie à l'entier supérieur
    /// </summary>
    Integer,
    /// <summary>
    /// La note est arrondie au demi-point le plus proche
    /// </summary>
    HalfPoint,
    /// <summary>
    /// La note n'est pas arrondie
    /// </summary>
    Decimal
}
