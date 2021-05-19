using System;
using System.Collections.Generic;
using System.Linq;

namespace LumberCalculator
{
    public class StoreLumber
    {
        public Guid Identifier { get; set; }
        public LumberDimension Dimensions { get; set; }
        public decimal Length { get; set; }
        public decimal Price { get; set; }
        public List<CutDimension> CutLengths { get; set; } = new List<CutDimension>();
        public decimal ScrapLength => CutLengths.Any() ? Length - CutLengths.Select(o => o.Length).Aggregate((a, d) => a + d) : Length;
        public decimal TotalCutLength => Length - ScrapLength;
        public string Description => $"{Dimensions.Name} - {TotalCutLength} total - {ScrapLength} scrap";
    }
}