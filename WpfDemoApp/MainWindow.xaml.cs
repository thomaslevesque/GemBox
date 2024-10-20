using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EssentialMVVM;

namespace WpfDemoApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Movie = new Movie
        {
            Title = "The Shawshank Redemption",
            Director = "Frank Darabont",
            Rating = 4.5,
            InStock = true,
            MediaType = MediaType.BluRay,
            ReleaseDate = new DateTime(1994, 9, 25)
        };
        InitializeComponent();
    }

    public Movie Movie { get; set; }
}

public class Movie
{
    public string Title { get; set; }
    public MediaType MediaType { get; set; }
    public string Director { get; set; }
    public bool InStock { get; set; }
    public DateTime ReleaseDate { get; set; }
    public double Rating { get; set; }
}

public enum MediaType
{
    DVD,
    BluRay,
    VHS,
    Streaming
}
