﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class UserSettingPage : ContentPage
    {
        public UserSettingPage()
        {
            InitializeComponent();
            Title = "User Setting";
            pDatabaseType.SelectedIndexChanged += (sender, e) =>
            {
                if (pDatabaseType.SelectedIndex == 1)
                {
                    
                }
                else
                {

                }
            };
            //scDatabaseType.
        }
       
    }
}
