using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEnglish
{
    internal class Category
    {
        private readonly uint _id;
        private readonly string _name;

        public Category(uint id, string name)
        {
            _id = id;
            _name = name;
        }

        public uint GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }
    }
}
