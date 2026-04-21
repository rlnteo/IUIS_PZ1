using _90s_Minimalism_CMS_Project.Helpers;
using _90s_Minimalism_CMS_Project.Models;
using Microsoft.Win32;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace _90s_Minimalism_CMS_Project.Pages
{
    public partial class AddEditItemPage : Page
    {
        private MainWindow _mainWindow;
        private FashionItem _editingItem;
        private bool _isEditMode => _editingItem != null;

        private const string NamePlaceholder = "e.g. Slip Dress in Silk Charmeuse";
        private const string DesignerPlaceholder = "e.g. Calvin Klein";
        private const string PricePlaceholder = "e.g. 450.00";

        private bool _suppressSelectionFeedback = false;
        private bool _suppressToolbarFeedback = false;

        private static readonly List<string> Categories = new List<string>
        {
            "Outerwear", "Dresses", "Trousers", "Tops", "Knitwear", "Skirts",
            "Footwear", "Accessories", "Underwear / Intimates", "Tailoring"
        };

        public AddEditItemPage(FashionItem itemToEdit = null)
        {
            InitializeComponent();
            _mainWindow = (MainWindow)Application.Current.MainWindow;
            _editingItem = itemToEdit;

            InitiatePlaceholders();
            InitiateComboBoxes();

            if (_isEditMode)
            {
                PageTitleText.Text = "EDIT PIECE";
                SaveButton.Content = "SAVE CHANGES";
                PopulateFormForEdit();
            }
        }

        private void InitiatePlaceholders()
        {
            SetPlaceholder(NameTextBox, NamePlaceholder);
            SetPlaceholder(DesignerTextBox, DesignerPlaceholder);
            SetPlaceholder(PriceTextBox, PricePlaceholder);
        }

        private void InitiateComboBoxes()
        {
            CategoryComboBox.ItemsSource = Categories;
            var fonts = Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(f => f)
                .ToList();
            FontFamilyComboBox.ItemsSource = fonts;
            FontFamilyComboBox.SelectedItem = "Gill Sans MT";

            var sizes = new List<int> { 8, 9, 10, 11, 12, 13, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72 };
            FontSizeComboBox.ItemsSource = sizes;
            FontSizeComboBox.SelectedItem = 13;

            FontColorComboBox.ItemsSource = typeof(Colors)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(Color))
                .Select(p => new ColorItem
                {
                    Name = p.Name,
                    Brush = new SolidColorBrush((Color)p.GetValue(null))
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        private void PopulateFormForEdit()
        {
            ClearPlaceholder(NameTextBox, _editingItem.Name);
            ClearPlaceholder(DesignerTextBox, _editingItem.Designer);
            ClearPlaceholder(PriceTextBox, _editingItem.Price.ToString("F2"));

            CategoryComboBox.SelectedItem = _editingItem.Category;

            if (!string.IsNullOrEmpty(_editingItem.ImagePath))
            {
                string fullPath = Path.IsPathRooted(_editingItem.ImagePath)
                    ? _editingItem.ImagePath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _editingItem.ImagePath);
                if (File.Exists(fullPath))
                {
                    ImagePathTextBox.Text = _editingItem.ImagePath;
                    ImagePathTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
                    LoadImagePreview(fullPath);
                }
            }

            if (!string.IsNullOrEmpty(_editingItem.RtfFilePath))
            {
                string fullRtf = Path.IsPathRooted(_editingItem.RtfFilePath)
                    ? _editingItem.RtfFilePath
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _editingItem.RtfFilePath);
                if (File.Exists(fullRtf))
                {
                    _suppressSelectionFeedback = true;
                    using (FileStream fs = new FileStream(fullRtf, FileMode.Open, FileAccess.Read))
                    {
                        TextRange range = new TextRange(
                            DescriptionRichTextBox.Document.ContentStart,
                            DescriptionRichTextBox.Document.ContentEnd);
                        range.Load(fs, DataFormats.Rtf);
                    }
                    _suppressSelectionFeedback = false;
                }
            }
        }

        private void SetPlaceholder(TextBox tb, string placeholder)
        {
            tb.Text = placeholder;
            tb.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            tb.Tag = placeholder;
        }

        private void ClearPlaceholder(TextBox tb, string realValue)
        {
            tb.Text = realValue;
            tb.Foreground = (Brush)Application.Current.FindResource("BrushInk");
        }

        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text == NamePlaceholder)
            {
                NameTextBox.Text = string.Empty;
                NameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
            }
        }

        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameTextBox.Text = NamePlaceholder;
                NameTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            }
        }
        private void DesignerTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DesignerTextBox.Text == DesignerPlaceholder)
            {
                DesignerTextBox.Text = string.Empty;
                DesignerTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
            }
        }

        private void DesignerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DesignerTextBox.Text))
            {
                DesignerTextBox.Text = DesignerPlaceholder;
                DesignerTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            }
        }

        private void PriceTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PriceTextBox.Text == PricePlaceholder)
            {
                PriceTextBox.Text = string.Empty;
                PriceTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
            }
        }

        private void PriceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PriceTextBox.Text))
            {
                PriceTextBox.Text = PricePlaceholder;
                PriceTextBox.Foreground = (Brush)Application.Current.FindResource("BrushMidtone");
            }
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Select an image",
                Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All files|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                string imagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Images");
                Directory.CreateDirectory(imagesFolderPath);

                string fileName = Path.GetFileName(dlg.FileName);
                string destinationPath = Path.Combine(imagesFolderPath, fileName);

                if (!File.Exists(destinationPath))
                {
                    File.Copy(dlg.FileName, destinationPath);
                }

                string relativePath = GetRelativePath(destinationPath);
                ImagePathTextBox.Text = relativePath;
                ImagePathTextBox.Foreground = (Brush)Application.Current.FindResource("BrushInk");
                LoadImagePreview(destinationPath);
                ImageError.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadImagePreview(string fullPath)
        {
            try
            {
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(fullPath, UriKind.Absolute);
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.EndInit();

                ImagePreview.Source = bmp;
                ImagePreview.Visibility = Visibility.Visible;
                ImagePreviewPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch
            {
                ImagePreview.Visibility = Visibility.Collapsed;
                ImagePreviewPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem == null || !DescriptionRichTextBox.IsLoaded)
            {
                return;
            }
            string fontName = FontFamilyComboBox.SelectedItem.ToString();
            DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(fontName));
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem == null || !DescriptionRichTextBox.IsLoaded)
            {
                return;
            }
            double size = Convert.ToDouble(FontSizeComboBox.SelectedItem);
            DescriptionRichTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
        }

        private void FontColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontColorComboBox.SelectedItem == null || !DescriptionRichTextBox.IsLoaded)
            {
                return;
            }
            ColorItem selected = FontColorComboBox.SelectedItem as ColorItem;
            if (selected == null)
            {
                return;
            }
            DescriptionRichTextBox.Selection.ApplyPropertyValue(
                TextElement.ForegroundProperty, selected.Brush);
        }

        private void DescriptionRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_suppressSelectionFeedback)
            {
                return;
            }

            _suppressToolbarFeedback = true;

            object fontWeight = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldToggleButton.IsChecked = fontWeight != DependencyProperty.UnsetValue
                && (FontWeight)fontWeight == FontWeights.Bold;

            object fontStyle = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicToggleButton.IsChecked = fontStyle != DependencyProperty.UnsetValue && (FontStyle)fontStyle == FontStyles.Italic;

            // Underline feedback
            object textDecorations = DescriptionRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (textDecorations != DependencyProperty.UnsetValue && textDecorations is TextDecorationCollection tdc)
            {
                UnderlineToggleButton.IsChecked = tdc.Count > 0
                    && tdc[0].Location == TextDecorationLocation.Underline;
            }
            else
            {
                UnderlineToggleButton.IsChecked = false;
            }

            object fontFamily = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            if (fontFamily != DependencyProperty.UnsetValue && fontFamily is FontFamily ff)
            {
                string fontName = ff.Source;
                if (FontFamilyComboBox.Items.Contains(fontName))
                    FontFamilyComboBox.SelectedItem = fontName;
            }

            object fontSize = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue && fontSize is double fs)
            {
                int sizeAsInt = (int)fs;
                if (FontSizeComboBox.Items.Contains(sizeAsInt))
                    FontSizeComboBox.SelectedItem = sizeAsInt;
            }

            object foreground = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.ForegroundProperty);
            if (foreground != DependencyProperty.UnsetValue && foreground is SolidColorBrush selBrush)
            {
                var match = FontColorComboBox.Items.OfType<ColorItem>()
                    .FirstOrDefault(c => c.Brush.Color == selBrush.Color);
                if (match != null)
                    FontColorComboBox.SelectedItem = match;
            }

            _suppressToolbarFeedback = false;
        }

        private void DescriptionRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = new TextRange(
                DescriptionRichTextBox.Document.ContentStart,
                DescriptionRichTextBox.Document.ContentEnd).Text;

            int wordCount = string.IsNullOrWhiteSpace(text) ? 0 :
                text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            WordCountText.Text = $"{wordCount} WORD{(wordCount != 1 ? "S" : "")}";
            // Font family feedback
            object fontFamily = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            if (fontFamily != DependencyProperty.UnsetValue && fontFamily is FontFamily ff)
            {
                string fontName = ff.Source;
                if (FontFamilyComboBox.Items.Contains(fontName))
                    FontFamilyComboBox.SelectedItem = fontName;
            }

            // Font size feedback
            object fontSize = DescriptionRichTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (fontSize != DependencyProperty.UnsetValue && fontSize is double fs)
            {
                int sizeAsInt = (int)fs;
                if (FontSizeComboBox.Items.Contains(sizeAsInt))
                    FontSizeComboBox.SelectedItem = sizeAsInt;
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text == NamePlaceholder)
            {
                NameError.Text = "Name is required.";
                NameError.Visibility = Visibility.Visible;
                NameTextBox.BorderBrush = (Brush)Application.Current.FindResource("BrushError");
                isValid = false;
            }
            else
            {
                NameError.Visibility = Visibility.Collapsed;
                NameTextBox.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(DesignerTextBox.Text) || DesignerTextBox.Text == DesignerPlaceholder)
            {
                DesignerError.Text = "Designer name is required.";
                DesignerError.Visibility = Visibility.Visible;
                DesignerTextBox.BorderBrush = (Brush)Application.Current.FindResource("BrushError");
                isValid = false;
            }
            else
            {
                DesignerError.Visibility = Visibility.Collapsed;
                DesignerTextBox.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
            }

            if (string.IsNullOrWhiteSpace(PriceTextBox.Text)
                || PriceTextBox.Text == PricePlaceholder
                || !double.TryParse(PriceTextBox.Text, out double parsedPrice)
                || parsedPrice <= 0)
            {
                PriceError.Text = "Price must be a positive number (e.g. 450.00).";
                PriceError.Visibility = Visibility.Visible;
                PriceTextBox.BorderBrush = (Brush)Application.Current.FindResource("BrushError");
                isValid = false;
            }
            else
            {
                PriceError.Visibility = Visibility.Collapsed;
                PriceTextBox.ClearValue(System.Windows.Controls.Control.BorderBrushProperty);
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                CategoryError.Text = "Please select a category.";
                CategoryError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                CategoryError.Visibility = Visibility.Collapsed;
            }

            if (string.IsNullOrWhiteSpace(ImagePathTextBox.Text) || ImagePathTextBox.Text == "No image selected")
            {
                ImageError.Text = "Please select an image file.";
                ImageError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                ImageError.Visibility = Visibility.Collapsed;
            }

            string rtfText = new TextRange(
                DescriptionRichTextBox.Document.ContentStart,
                DescriptionRichTextBox.Document.ContentEnd).Text.Trim();
            if (string.IsNullOrWhiteSpace(rtfText))
            {
                DescriptionError.Text = "Editorial notes cannot be empty.";
                DescriptionError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                DescriptionError.Visibility = Visibility.Collapsed;
            }

            return isValid;
        }

        private string SaveRtfFile()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Rtf");
            Directory.CreateDirectory(folderPath);

            string fileName = _isEditMode && !string.IsNullOrEmpty(_editingItem.RtfFilePath)
                ? Path.GetFileName(_editingItem.RtfFilePath)
                : Guid.NewGuid().ToString() + ".rtf";

            string fullPath = Path.Combine(folderPath, fileName);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                TextRange range = new TextRange(
                    DescriptionRichTextBox.Document.ContentStart,
                    DescriptionRichTextBox.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }

            return GetRelativePath(fullPath);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToDataTable();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            double.TryParse(PriceTextBox.Text, out double price);
            string rtfPath = SaveRtfFile();

            if (_isEditMode)
            {
                _editingItem.Name = NameTextBox.Text.Trim();
                _editingItem.Designer = DesignerTextBox.Text.Trim();
                _editingItem.Price = price;
                _editingItem.Category = CategoryComboBox.SelectedItem.ToString();
                _editingItem.ImagePath = ImagePathTextBox.Text;
                _editingItem.RtfFilePath = rtfPath;

                _mainWindow.ShowToast(new ToastNotification("Updated",
                    $"'{_editingItem.Name}' has been updated.",
                    NotificationType.Success));
            }
            else
            {
                FashionItem newItem = new FashionItem(
                    name: NameTextBox.Text.Trim(),
                    designer: DesignerTextBox.Text.Trim(),
                    price: price,
                    category: CategoryComboBox.SelectedItem.ToString(),
                    imagePath: ImagePathTextBox.Text,
                    rtfFilePath: rtfPath,
                    dateAdded: DateTime.Now);

                _mainWindow.FashionItems.Add(newItem);
                _mainWindow.ShowToast(new ToastNotification("Added",
                    $"'{newItem.Name}' has been added to the collection.",
                    NotificationType.Success));
            }

            _mainWindow.SaveData();
            _mainWindow.NavigateToDataTable();
        }

        private string GetRelativePath(string absolutePath)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                Uri baseUri = new Uri(basePath);
                Uri fileUri = new Uri(absolutePath);
                return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString())
                           .Replace('/', Path.DirectorySeparatorChar);
            }
            catch
            {
                return absolutePath;
            }
        }
    }
}
