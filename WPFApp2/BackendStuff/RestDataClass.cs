using System;
using System.Collections.Generic;
using System.Text;

namespace TourFinder.BackendStuff.DB
{
    class RestDataClass
    {
        private static RestDataClass instance = null;

        public static RestDataClass Instance()
        {
            if (instance == null)
            {
                instance = new RestDataClass();
            }

            return instance;
        }

        private RestDataClass()
        {

        }

        ~RestDataClass()
        {

        }
    }
}
