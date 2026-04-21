using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace _90s_Minimalism_CMS_Project.Models
{
    [Serializable]
    public class FashionItem : INotifyPropertyChanged
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        public string Designer { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public string RtfFilePath { get; set; }
        public DateTime DateAdded { get; set; }
        [XmlIgnore]
        public string FullImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath)) return null;
                try { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath); }
                catch { return ImagePath; }
            }
        }
        [XmlIgnore]
        private bool _isSelected;

        [XmlIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public FashionItem() { }

        public FashionItem(string name, string designer, double price,
            string category, string imagePath, string rtfFilePath, DateTime dateAdded)
        {
            Name = name;
            Designer = designer;
            Price = price;
            Category = category;
            ImagePath = imagePath;
            RtfFilePath = rtfFilePath;
            DateAdded = dateAdded;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
