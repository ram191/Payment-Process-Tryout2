﻿using System;
namespace PaymentService.Model
{
    public class BaseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class CommandDTO<T>
    {
        public Data<T> Data { get; set; }
    }

    public class Data<T>
    {
        public T Attributes { get; set; }
    }
}
