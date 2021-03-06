﻿using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using ECollegeAPI.Model.Boilerplate;

namespace ECollegeAPI.Model
{
    public class DropboxMessage
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public string Comments { get; set; }
        public User SubmissionStudent { get; set; }
        public User Author { get; set; }
        public List<DropboxAttachment> Attachments { get; set; }
    }
}
