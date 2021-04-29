using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TourFinder.Models
{
    public class TourAddUtility 
    {
        string _Name;
        string _StartLocation;
        string _EndLocation;
        string _Description = " ";
        string _ImagePath;
        float _Distance;

        public string Name {
            get{
                return this._Name;
            }
            set
            {
                if(this._Name != value)
                {
                    this._Name = value;
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
                }
            } 
        }
        public string Description 
        {
            get
            {
                return this._Description;
            }
            set
            {
                if (this._Description != value)
                {
                    this._Description = value;
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
                }
            }
        }
        public float Distance {
            get 
            { 
                return this._Distance; 
            }
            set 
            {
                if (_Distance != value) 
                {
                    this._Distance = value;
                }
            }
        }
    }
}
