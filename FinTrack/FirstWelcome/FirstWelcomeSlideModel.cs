using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace FinTrack.FirstWelcome
{
    public class FirstWelcomeSlideModel
    {
        public string ImagePath { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
