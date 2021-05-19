namespace LumberCalculator
{
    public class CutListLumber
    {
        public int Identifier { get; set; }
        public StoreLumber SelectedStoreLumber { get; set; }
        public decimal Length { get; set; }
        public int Quantity { get; set; }
        public string Name => $"{Identifier} - {SelectedStoreLumber.Dimensions.Name} - {Length} in. ({Quantity})";
    }
}