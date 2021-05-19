namespace LumberCalculator
{
    public class LumberDimension
    {
        public string Name { get; set; }
        public decimal ActualHeight { get; set; }
        public decimal ActualWidth { get; set; }

        public LumberDimension(string name, decimal actualHeight, decimal actualWidth)
        {
            Name = name;
            ActualHeight = actualHeight;
            ActualWidth = actualWidth;
        }
    }
}