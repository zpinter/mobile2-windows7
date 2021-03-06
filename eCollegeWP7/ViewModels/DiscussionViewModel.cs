﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ECollegeAPI.Model;
using eCollegeWP7.Util;
using ECollegeAPI.Services.Discussions;
using eCollegeWP7.Util.Converters;

namespace eCollegeWP7.ViewModels
{
    public enum DiscussionType
    {
        TopicAndResponses,
        ResponseAndResponses
    }

    public class DiscussionViewModel : ViewModelBase
    {
        private string _UserTopicID;
        public string UserTopicID
        {
            get { return _UserTopicID; }
            set { _UserTopicID = value; this.OnPropertyChanged(() => this.UserTopicID); }
        }

        private long _TopicID;
        public long TopicID
        { 
            get { return _TopicID; }
            set { _TopicID = value; this.OnPropertyChanged(()=>this.TopicID); }
        }

        private UserDiscussionTopic _UserTopic;
        public UserDiscussionTopic UserTopic
        {
            get { return _UserTopic; }
            set { _UserTopic = value; this.OnPropertyChanged(() => this.UserTopic); }
        }

        private long _ResponseID;
        public long ResponseID
        {
            get { return _ResponseID; }
            set { _ResponseID = value; this.OnPropertyChanged(() => this.ResponseID); }
        }

        private string _UserResponseID;
        public string UserResponseID
        {
            get { return _UserResponseID; }
            set { _UserResponseID = value; this.OnPropertyChanged(() => this.UserResponseID); }
        }

        private UserDiscussionResponse _UserResponse;
        public UserDiscussionResponse UserResponse
        {
            get { return _UserResponse; }
            set { _UserResponse = value; this.OnPropertyChanged(() => this.UserResponse); }
        }

        private ObservableCollection<DiscussionViewModel> _Responses;
        public ObservableCollection<DiscussionViewModel> Responses
        {
            get { return _Responses; }
            set { _Responses = value; this.OnPropertyChanged(() => this.Responses); }
        }

        private DiscussionType _CurrentDiscussionType;
        public DiscussionType CurrentDiscussionType
        {
            get { return _CurrentDiscussionType; }
            set { _CurrentDiscussionType = value; this.OnPropertyChanged(() => this.CurrentDiscussionType); }
        }

        private DateTime? _DiscussionDate;
        public DateTime? DiscussionDate
        {
            get { return _DiscussionDate; }
            set { _DiscussionDate = value; this.OnPropertyChanged(() => this.DiscussionDate); }
        }

        private string _DiscussionTitle;
        public string DiscussionTitle
        {
            get { return _DiscussionTitle; }
            set { _DiscussionTitle = value; this.OnPropertyChanged(() => this.DiscussionTitle); }
        }

        private string _DiscussionDescription;
        public string DiscussionDescription
        {
            get { return _DiscussionDescription; }
            set { _DiscussionDescription = value; this.OnPropertyChanged(() => this.DiscussionDescription); }
        }

        private long _DiscussionResponseCount;
        public long DiscussionResponseCount
        {
            get { return _DiscussionResponseCount; }
            set { _DiscussionResponseCount = value; this.OnPropertyChanged(() => this.DiscussionResponseCount); }
        }

        private string _TotalResponsesLine;
        public string TotalResponsesLine
        {
            get { return _TotalResponsesLine; }
            set { _TotalResponsesLine = value; this.OnPropertyChanged(() => this.TotalResponsesLine); }
        }
        
        private string _MyResponsesLine;
        public string MyResponsesLine
        {
            get { return _MyResponsesLine; }
            set { _MyResponsesLine = value; this.OnPropertyChanged(() => this.MyResponsesLine); }
        }

        private long _UnreadResponseCount;
        public long UnreadResponseCount
        {
            get { return _UnreadResponseCount; }
            set { _UnreadResponseCount = value; this.OnPropertyChanged(() => this.UnreadResponseCount); }
        }

        private string _IconTemplate;
        public string IconTemplate
        {
            get { return _IconTemplate; }
            set { _IconTemplate = value; this.OnPropertyChanged(() => this.IconTemplate); }
        }

        private string _AuthorName;
        public string AuthorName
        {
            get { return _AuthorName; }
            set { _AuthorName = value; this.OnPropertyChanged(() => this.AuthorName); }
        }
        
        public string NavigationPath { get; set; }

        protected void SetupFromTopic(UserDiscussionTopic ud)
        {
            this.UserTopic = ud;
            this.DiscussionDate = null;//no date for topics
            this.DiscussionTitle = HttpUtility.HtmlDecode(ud.Topic.Title);
            this.DiscussionDescription = HtmlToTextConverter.StripHtml(ud.Topic.Description);
            this.DiscussionResponseCount = ud.ChildResponseCounts.TotalResponseCount;
            this.UnreadResponseCount = ud.ChildResponseCounts.UnreadResponseCount;
            this.TotalResponsesLine = CalculateTotalResponsesLine(this.DiscussionResponseCount);
            this.MyResponsesLine = CalculateMyResponsesLine(ud.ChildResponseCounts.PersonalResponseCount);
            this.NavigationPath = "/Views/DiscussionPage.xaml?topicId=" + ud.Topic.ID;
            this.IconTemplate = CalculateIconFromResponseCounts(ud.ChildResponseCounts);
        }

        protected  void SetupFromResponse(UserDiscussionResponse ud)
        {
            this.UserResponse = ud;
            this.DiscussionDate = ud.Response.PostedDate;
            this.DiscussionTitle = HttpUtility.HtmlDecode(ud.Response.Title);
            this.DiscussionDescription = HtmlToTextConverter.StripHtml(ud.Response.Description);
            this.DiscussionResponseCount = ud.ChildResponseCounts.TotalResponseCount;
            this.UnreadResponseCount = ud.ChildResponseCounts.UnreadResponseCount;
            this.TotalResponsesLine = CalculateTotalResponsesLine(this.DiscussionResponseCount);
            this.MyResponsesLine = CalculateMyResponsesLine(ud.ChildResponseCounts.PersonalResponseCount);
            this.AuthorName = ud.Response.Author.DisplayName;
            this.NavigationPath = "/Views/DiscussionPage.xaml?responseId=" + ud.Response.ID;
            this.IconTemplate = CalculateIconFromResponseCounts(ud.ChildResponseCounts);
        }

        protected  string CalculateIconFromResponseCounts(ResponseCount rc)
        {
            if (rc.Last24HourResponseCount >= 10)
            {
                return "IconDiscussionsHotTopic";
            }
            if (rc.TotalResponseCount == 0)
            {
                return "IconDiscussionsNoResponses";
            }
            return "IconDiscussionsResponses";
        }

        protected string CalculateTotalResponsesLine(long count)
        {
            if (count == 0)
            {
                return "No responses";
            } else if (count == 1)
            {
                return "1 total response";
            } else
            {
                return count + " total responses";
            }
        }

        protected string CalculateMyResponsesLine(long count)
        {
            if (count == 0)
            {
                return null;
            }
            else if (count == 1)
            {
                return "1 response by you";
            }
            else
            {
                return count + " responses by you";
            }
        }

        public DiscussionViewModel(UserDiscussionTopic ut)
        {
            SetupFromTopic(ut);
        }
        
        public DiscussionViewModel(UserDiscussionResponse ur)
        {
            SetupFromResponse(ur);
        }

        public DiscussionViewModel(long discussionId, DiscussionType dt)
        {
            CurrentDiscussionType = dt;

            if (CurrentDiscussionType == DiscussionType.TopicAndResponses)
            {
                this.TopicID = discussionId;
                this.UserTopicID = AppViewModel.CurrentUser.ID + "-" + this.TopicID;

                App.BuildService(new FetchMyDiscussionTopicByIdService(UserTopicID)).Execute(service =>
                {
                    SetupFromTopic(service.Result);
                });
            }

            if (CurrentDiscussionType == DiscussionType.ResponseAndResponses)
            {
                this.ResponseID = discussionId;
                this.UserResponseID = AppViewModel.CurrentUser.ID + "-" + this.ResponseID;

                App.BuildService(new FetchMyDiscussionResponseByIdService(UserResponseID)).Execute(service =>
                {
                    SetupFromResponse(service.Result);
                    this.MarkAsRead();
                });
            }
        }

        public void PostResponse(string responseTitle, string responseText)
        {
            if (CurrentDiscussionType == DiscussionType.TopicAndResponses)
            {
                App.BuildService(new PostMyResponseToTopicService(this.TopicID, responseTitle, responseText)).
                    Execute(service =>
                    {
                        this.UserTopic.ChildResponseCounts.Last24HourResponseCount++;
                        this.UserTopic.ChildResponseCounts.PersonalResponseCount++;
                        this.UserTopic.ChildResponseCounts.TotalResponseCount++;
                        App.InvalidateCache(new FetchMyDiscussionTopicByIdService(UserTopicID));
                        FetchResponses(true);
                    });
            }
            else if (CurrentDiscussionType == DiscussionType.ResponseAndResponses)
            {
                App.BuildService(new PostMyResponseToResponseService(this.ResponseID, responseTitle, responseText)).
                    Execute(service =>
                    {
                        this.UserResponse.ChildResponseCounts.Last24HourResponseCount++;
                        this.UserResponse.ChildResponseCounts.PersonalResponseCount++;
                        this.UserResponse.ChildResponseCounts.TotalResponseCount++;
                        App.InvalidateCache(new FetchMyDiscussionResponseByIdService(UserResponseID));
                        FetchResponses(true);
                    });
            }
        }
        
        public void MarkAsRead()
        {
            if (CurrentDiscussionType == DiscussionType.ResponseAndResponses && UserResponse.MarkedAsRead != true)
            {
                App.BuildService(new UpdateResponseReadStatusService(ResponseID, true)).Execute(service =>
                {
                    UserResponse.MarkedAsRead = true;
                    App.InvalidateCache(new FetchMyDiscussionResponseByIdService(UserResponseID));
                });
            }
        }

        public void FetchResponses()
        {
            FetchResponses(false);
        }

        public void FetchResponses(bool ignoreCache)
        {
            if (CurrentDiscussionType == DiscussionType.TopicAndResponses)
            {
                var call = App.BuildService(new FetchMyDiscussionResponsesByTopicService(TopicID));
                if (ignoreCache) call.NoCacheRead();

                call.Execute(service =>
                {
                    var formattedResult = new ObservableCollection<DiscussionViewModel>();
                    foreach (var r in service.Result) formattedResult.Add(new DiscussionViewModel(r));
                    this.Responses = formattedResult;
                });

            }
            else if (CurrentDiscussionType == DiscussionType.ResponseAndResponses)
            {
                var call = App.BuildService(new FetchMyDiscussionResponsesByResponseService(ResponseID));
                if (ignoreCache) call.NoCacheRead();

                call.Execute(service =>
                {
                    var formattedResult = new ObservableCollection<DiscussionViewModel>();
                    foreach (var r in service.Result) formattedResult.Add(new DiscussionViewModel(r));
                    this.Responses = formattedResult;
                });


            }

        }

    }
}