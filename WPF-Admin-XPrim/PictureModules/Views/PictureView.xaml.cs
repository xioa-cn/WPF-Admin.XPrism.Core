using System.Windows.Controls;

namespace PictureModules.Views;

public partial class PictureView : Page
{
    public PictureView()
    {
        InitializeComponent();
        this.Loaded += (s, e) =>
        {
           this. PicControl1.LoadImages([
                "https://th.bing.com/th/id/R.9f00e054b22ea739eeee3af7d4379ac8?rik=pPR0disHb%2b7GdQ&pid=ImgRaw&r=0",
                "https://th.bing.com/th/id/OIP.uuMRp41SjL9ukaBDDBWz5wHaNK?rs=1&pid=ImgDetMain",
                "https://th.bing.com/th/id/R.fab24d0ad714e31a62cc9ea3947340a5?rik=Aqg7%2bGn5hgfgHQ&pid=ImgRaw&r=0",
                "https://th.bing.com/th/id/R.9f00e054b22ea739eeee3af7d4379ac8?rik=pPR0disHb%2b7GdQ&pid=ImgRaw&r=0",
                "https://th.bing.com/th/id/OIP.uuMRp41SjL9ukaBDDBWz5wHaNK?rs=1&pid=ImgDetMain",
                "https://th.bing.com/th/id/R.fab24d0ad714e31a62cc9ea3947340a5?rik=Aqg7%2bGn5hgfgHQ&pid=ImgRaw&r=0",
                "https://th.bing.com/th/id/R.9f00e054b22ea739eeee3af7d4379ac8?rik=pPR0disHb%2b7GdQ&pid=ImgRaw&r=0",
                "https://th.bing.com/th/id/OIP.uuMRp41SjL9ukaBDDBWz5wHaNK?rs=1&pid=ImgDetMain",
                "https://th.bing.com/th/id/R.fab24d0ad714e31a62cc9ea3947340a5?rik=Aqg7%2bGn5hgfgHQ&pid=ImgRaw&r=0",
            ]);
        };
    }
}