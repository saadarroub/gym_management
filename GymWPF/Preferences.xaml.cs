﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace GymWPF
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Page
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private void BtnFacebook_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("www.facebook.com/ShiftUpIT");
        }

        private void BtnWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://shiftup-it.com");
        }
    }
}
