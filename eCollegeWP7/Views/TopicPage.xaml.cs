﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ECollegeAPI.Model;

namespace eCollegeWP7.Views
{
    public partial class TopicPage : PhoneApplicationPage
    {
        public TopicPage()
        {
            InitializeComponent();
        } 

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            this.DataContext = new TopicViewModel(parameters["topicId"]);
        }

        private void BtnResponse_Click(object sender, RoutedEventArgs e)
        {
            var rheader = (sender as Button).DataContext as DiscussionResponseHeader;
            this.NavigationService.Navigate(new Uri("/Views/TopicResponsePage.xaml?responseId=" + rheader.Response.ID, UriKind.Relative));
        }
    }
}