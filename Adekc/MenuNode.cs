using System;
using System.Collections.Generic;
using System.Text;

namespace Adekc
{
    class MenuNode
    {
        public string name;
        public string rc;
        public List<MenuNode> childNodes;

        public MenuNode()
        {
            name = "";
            rc = "default.html";
            childNodes = null;
        }
    }
}
