﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperV
{
   public interface ICollisionable
   {
      BoundingSphere SphèreDeCollision { get; }
      bool EstEnCollision(object autreObjet);
   }
}
