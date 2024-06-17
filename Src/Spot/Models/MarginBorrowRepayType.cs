namespace Binance.Spot.Models
{
    public struct MarginBorrowRepayType
    {
        private MarginBorrowRepayType(string value)
        {
            this.Value = value;
        }

        public static MarginBorrowRepayType BORROW { get => new MarginBorrowRepayType("BORROW"); }
        public static MarginBorrowRepayType REPAY { get => new MarginBorrowRepayType("REPAY"); }

        public string Value { get; private set; }

        public static implicit operator string(MarginBorrowRepayType enm) => enm.Value;

        public override string ToString() => this.Value.ToString();
    }
}