namespace ServiceLayer.ViewModel
{
    public class CategoryPivotViewModel
    {
        public string CategoryName { get; set; }
        public int BookCount { get; set; }
        public double Percentage { get; set; }

        // وضعیت‌ها
        public int AvailableCount { get; set; }
        public int BorrowedCount { get; set; }
        public int LostCount { get; set; }

    }
}
