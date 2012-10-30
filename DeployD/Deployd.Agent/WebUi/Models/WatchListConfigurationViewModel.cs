using System.Collections.Generic;

namespace Deployd.Agent.WebUi.Models
{
    public class WatchListConfigurationViewModel
    {
        public IEnumerable<WatchListItemViewModel> Groups { get; set; }

        public IEnumerable<WatchListItemViewModel> Packages { get; set; }
    }
}