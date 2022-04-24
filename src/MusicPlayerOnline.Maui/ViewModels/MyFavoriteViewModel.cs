﻿using JiuLing.CommonLibs.ExtensionMethods;

namespace MusicPlayerOnline.Maui.ViewModels
{
    public class MyFavoriteViewModel : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private string _imageUrl;
        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged("ImageUrl");
                OnPropertyChanged("IsUseDefaultImage");
            }
        }
         
        public bool IsUseDefaultImage => ImageUrl.IsEmpty();

        private int _musicCount;
        public int MusicCount
        {
            get => _musicCount;
            set
            {
                _musicCount = value;
                OnPropertyChanged();
            }
        }
    }
}
