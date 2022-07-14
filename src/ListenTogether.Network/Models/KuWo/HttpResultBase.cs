﻿namespace ListenTogether.Network.Models.KuWo
{
    internal class HttpResultBase<T>
    {
        public int code { get; set; }
        public int status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
}
