using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedApp.Infrastructure.Email
{
    public class EmailSettings
    {
        public string? Host { get; set; } 
        public int Port { get; set; } 
        public bool UseStartTls { get; set; } 
        public bool UseSsl { get; set; }
        public string? Username { get; set; } 
        public string? Password { get; set; } 
        public string? FromName { get; set; } 
        public string? FromAddress { get; set; } 
    }
}
