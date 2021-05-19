﻿namespace LumberCalculator
{
    public static class Extensions
    {
        public static bool Equals(this LumberDimension source, LumberDimension compare)
        {
            return source.ActualHeight == compare.ActualHeight && source.ActualWidth == compare.ActualWidth;
        }

        public static StoreLumber Clone(this StoreLumber original)
        {
            return new StoreLumber
            {
                Dimensions = original.Dimensions,
                Length = original.Length,
                Price = original.Price,
            };
        }
    }
}