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
        public List<SlidePage> slides { get; set; } = new List<SlidePage>();

        private void SlideCreate()
        {
            slides.Add(new SlidePage { title = string.Empty, content = string.Empty, image = string.Empty });
        }
    }

    public class SlidePage
    {
        public string title { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
    }
}
