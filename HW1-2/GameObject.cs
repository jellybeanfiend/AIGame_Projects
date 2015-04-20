using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AIClass
{
    class GameObject : Game
    {
        public GameObject parent;

        public GameObject(GameObject parent)
        {
            this.parent = parent;
        }
    }
}
