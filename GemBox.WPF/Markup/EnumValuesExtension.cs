using System.Windows.Markup;

namespace GemBox.WPF.Markup;

/// <summary>
/// Renvoie un tableau des valeurs possibles d'une énumération
/// </summary>
[MarkupExtensionReturnType(typeof(Array))]
public class EnumValuesExtension : MarkupExtension
{
    /// <summary>
    /// Initialise une nouvelle instance de EnumValuesExtension
    /// </summary>
    public EnumValuesExtension()
    {
    }

    /// <summary>
    /// Initialise une nouvelle instance de EnumValuesExtension pour le type d'énumération spécifié
    /// </summary>
    /// <param name="enumType">Type d'énumération</param>
    public EnumValuesExtension(Type enumType)
    {
        this.EnumType = enumType;
    }

    /// <summary>
    /// Type de l'énumération dont la markup extension doit renvoyer les valeurs
    /// </summary>
    [ConstructorArgument("enumType")]
    public Type? EnumType { get; set; }

    /// <summary>
    /// Renvoie la liste des valeurs possibles de l'énumération du type spécifié
    /// </summary>
    /// <param name="serviceProvider">Objet qui peut fournir des services pour la markup extension</param>
    /// <returns>La liste des valeurs possibles de l'énumération</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType is null)
            return Array.Empty<object>();
        return Enum.GetValues(EnumType);
    }
}
