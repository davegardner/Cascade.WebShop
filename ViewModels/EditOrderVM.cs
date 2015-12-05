using Cascade.WebShop.Models;

namespace Cascade.WebShop.ViewModels
{
    public class EditOrderVM
    {

        public int Id { get; set; }
        public OrderStatus Status { get; set; }

        public EditOrderVM()
        {
        }

        public EditOrderVM(OrderRecordPart order)
        {
            Id = order.Id;
            Status = order.Status;
        }
    }
}