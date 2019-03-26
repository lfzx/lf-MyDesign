using System;
using System.Collections.Generic;
using System.Text;

namespace PostMatch.Core.Entities
{
    public class Student : Entity
    {
        public string Name { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }
    }
}
