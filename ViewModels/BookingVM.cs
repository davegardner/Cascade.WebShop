using Cascade.WebShop.Models;
using Orchard.Core.Common.ViewModels;
using System;

namespace Cascade.WebShop.ViewModels
{
    public class BookingVM
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool Morning { get; set; }
        public bool Afternoon { get; set; }
        public bool Evening { get; set; }

        public DateTimeEditor SpecificDateTime { get; set; }

        public string Notes { get; set; }

        public BookingVM() 
        {
            SpecificDateTime = new DateTimeEditor { ShowDate = true, ShowTime = true };
        }

        public BookingVM(OrderPart order = null)
        {
            SpecificDateTime = new DateTimeEditor { ShowDate = true, ShowTime = true };

            if (order != null)
            {
                Monday = order.Monday;
                Tuesday = order.Tuesday;
                Wednesday = order.Wednesday;
                Thursday = order.Thursday;
                Friday = order.Friday;
                Saturday = order.Saturday;
                Sunday = order.Sunday;
                Morning = order.Morning;
                Afternoon = order.Afternoon;
                Evening = order.Evening;
                SpecificDateTime.Date = order.SpecificDateTime.HasValue ? order.SpecificDateTime.Value.ToString("dd/mm/yyyy") : string.Empty;
                SpecificDateTime.Time = order.SpecificDateTime.HasValue ? order.SpecificDateTime.Value.ToString("hh:MM") : string.Empty;
                Notes = order.Notes;
            }
        }
    }
}