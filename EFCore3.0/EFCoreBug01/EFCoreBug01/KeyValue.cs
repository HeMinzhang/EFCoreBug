using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFCoreBug01
{
    public class KeyValue
    {
        [Key]
        public string Id { get; set; }
        public string Key { get; set; }
        public string KeyId { get; set; }
    }
}
