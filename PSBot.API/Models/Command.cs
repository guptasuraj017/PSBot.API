using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSBot.API.Models
{
    public class Command
    {
        public long CommandId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public List<CommandParameter> Params { get; set; }
    }
}
