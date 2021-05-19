using System;

namespace LumberCalculator
{
    public static class Extensions
    {
        public static bool Equals(this LumberDimension source, LumberDimension compare)
        {
            return source.ActualHeight == compare.ActualHeight && source.ActualWidth == compare.ActualWidth;
        }

        public static StoreLumber Clone(this StoreLumber original, Guid identifier)
        {
            return new StoreLumber
            {
                Identifier = identifier,
                Dimensions = original.Dimensions,
                Length = original.Length,
                Price = original.Price,
            };
        }
    }
}