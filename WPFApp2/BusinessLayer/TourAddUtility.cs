using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TourFinder
{
    public class TourAddUtility : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _Name;
        string _StartLocation;
        string _EndLocation;
        string _Description;
        string _ImagePath;

        public string Name {
            get{
                return this._Name;
            }
            set
            {
                if(this._Name != value)
                {
                    this._Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string StartLocation {
            get 
            {
                return this._StartLocation;
            }
            set 
            { 
                if(this._StartLocation != value)
                {
                    this._StartLocation = value;
                    OnPropertyChanged(nameof(StartLocation));
                }
            }
        }
        public string EndLocation {
            get 
            {
                return this._EndLocation;
            } 
            set
            {
                if(this._EndLocation != value)
                {
                    this._EndLocation = value;
                    OnPropertyChanged(nameof(EndLocation));
                }
            } 
        }
        public string Description { 
            get 
            {
                return this._Description;
            } 
            set 
            {
                if (this._Description != value)
                {
                    this._Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string ImagePath { 
            get 
            {
                return this._ImagePath; 
            }
            set
            {
                if (_ImagePath != value)
                {
                    this._ImagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
