using System;
using System.Collections.Generic;
using System.Text;

namespace EFCoreBug01
{
    public enum BuildType
    {
        NoThing,
        Test
    }
    public class CdKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BuildType BuildType { get; set; }
    }
}
