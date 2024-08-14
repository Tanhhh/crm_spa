namespace Erp.BackOffice.Sale.Models
{
    public class LeadByMobileModel
    {
        public int Id { get; set; }
        public string LeadName { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public int CreatedUserId { get; set; }
        public int AssignedUserId { get; set; }
    }
}