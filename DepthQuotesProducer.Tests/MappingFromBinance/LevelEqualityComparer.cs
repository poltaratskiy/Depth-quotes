using Abstractions.CommonObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DepthQuotesProducer.Tests.MappingFromBinance
{
    internal class LevelEqualityComparer : IEqualityComparer<Level>
    {
        public bool Equals(Level? x, Level? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            if (x.Price != y.Price || x.Quantity != y.Quantity)
            {
                return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] Level obj)
        {
            return HashCode.Combine(obj.Price, obj.Quantity);
        }
    }
}
