namespace LumberCalculator
{
    public class CutDimension
    {
        public int Identifier { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string DimensionName => $"{Identifier}";
        public decimal ActualLength { get; set; }

        public CutDimension(decimal length, LumberDimension dimensions, decimal actualLength, int identifier)
        {
            Length = length;
            ActualLength = actualLength;
            Identifier = identifier;
            Width = dimensions.ActualWidth;
            Height = dimensions.ActualHeight;
        }
    }
}