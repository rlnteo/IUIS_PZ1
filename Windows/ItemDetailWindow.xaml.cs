using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _90s_Minimalism_CMS_Project.Models;
using System.IO;
using Path = System.IO.Path;

namespace _90s_Minimalism_CMS_Project.Windows
{
    public partial class ItemDetailWindow : Window
    {
        public ItemDetailWindow(FashionItem item)
        {
            InitializeComponent();
            PopulateDetails(item);

        }

        private void PopulateDetails(FashionItem item)
        {
            DetailNameText.Text = item.Name?.ToUpper();
            DetailDesignerText.Text = item.Designer?.ToUpper();
            DetailCategoryText.Text = item.Category?.ToUpper();

            DetailPriceText.Text = $"${item.Price:N2}";
            DetailCategoryMetaText.Text = item.Category;
            DetailDesignerMetaText.Text = item.Designer;
            DetailDateText.Text = item.DateAdded.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(item.ImagePath))
            {
                try
                {
                    string fullPath = Path.IsPathRooted(item.ImagePath)
                        ? item.ImagePath
                        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.ImagePath);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bmp.CacheOption = BitmapCacheOption.OnLoad;
                    bmp.EndInit();
                    DetailImage.Source = bmp;
                }
                catch
                {
                    //Image could not be loaded, set to default or leave blank
                }
            }
           
            if (!string.IsNullOrEmpty(item.RtfFilePath))
            {
                try
                {
                    string fullRtfPath = Path.IsPathRooted(item.RtfFilePath)
                        ? item.RtfFilePath
                        : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item.RtfFilePath);

                    FlowDocument doc = new FlowDocument();
                    using (FileStream stream = new FileStream(fullRtfPath, FileMode.Open, FileAccess.Read))
                    {
                        TextRange range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        range.Load(stream, DataFormats.Rtf);
                    }
                    DescrtiptionViewer.Document = doc;
                }
                catch
                {
                    FlowDocument fallbackDoc = new FlowDocument(
                        new Paragraph(new Run("Descrtiption file could not be loaded.")));
                    DescrtiptionViewer.Document = fallbackDoc;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
