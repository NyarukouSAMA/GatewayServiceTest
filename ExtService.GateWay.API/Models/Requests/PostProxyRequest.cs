﻿using System.ComponentModel.DataAnnotations;

namespace ExtService.GateWay.API.Models.Requests
{
    public class PostProxyRequest
    {
        [Required]
        public string MethodName { get; set; } = string.Empty;
        [Required]
        public string SubMethodName { get; set; } = string.Empty;
        [Required]
        public object RequestBody { get; set; }
    }
}
