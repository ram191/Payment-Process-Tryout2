﻿using System;
using System.Collections.Generic;

namespace PaymentService.Model
{
    public class NotificationLogs
    {
        public class PostCommand
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public string Type { get; set; }
            public int From { get; set; }
            public List<TargetCommand> Targets { get; set; }
        }

        public class TargetCommand
        {
            public int Id { get; set; }
            public string Email_destination { get; set; }
        }
    }
}
