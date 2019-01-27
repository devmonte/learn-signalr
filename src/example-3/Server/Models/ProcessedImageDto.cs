using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace Server.Models
{
    [MessagePackObject]
    public class ProcessedImageDto
    {
        [Key(0)]
        public Guid Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Author { get; set; }
        [Key(3)]
        public string MyProperty { get; set; } = new string('A', 100_024);
    }
}
