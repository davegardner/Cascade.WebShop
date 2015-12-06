using Cascade.WebShop.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Cascade.WebShop.Helpers
{
    public static class OrderDetailSerializer
    {
        private const char FieldBreak = '\t';
        private const char RecordBreak = '\r';

        public static string Serialize(IEnumerable<OrderDetail> details)
        {
            string raw = String.Empty;
            if (details != null && details.Count() > 0)
            {
                foreach (var detail in details)
                {
                    raw += detail.Id.ToString() + FieldBreak +
                        detail.OrderRecord_Id.ToString(CultureInfo.InvariantCulture) + FieldBreak +
                        detail.ProductPartRecord_Id.ToString(CultureInfo.InvariantCulture) + FieldBreak +
                        detail.Quantity.ToString(CultureInfo.InvariantCulture) + FieldBreak +
                        detail.UnitPrice.ToString(CultureInfo.InvariantCulture) + FieldBreak +
                        detail.GSTRate.ToString(CultureInfo.InvariantCulture) + FieldBreak +
                        StringEncode(detail.Description) + FieldBreak +
                        StringEncode(detail.Sku)
                        + RecordBreak;
                }
            }
            return raw;
        }

        public static IList<OrderDetail> Deserialize(string raw)
        {
            var details = new List<OrderDetail>();
            if (!String.IsNullOrWhiteSpace(raw))
            {
                var lines = raw.Split(new[] { RecordBreak }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var fields = line.Split(FieldBreak);
                    OrderDetail detail = new OrderDetail
                    {
                        Deleted = false,
                        Id = Convert.ToInt32(fields[0], CultureInfo.InvariantCulture),
                        OrderRecord_Id = Convert.ToInt32(fields[1], CultureInfo.InvariantCulture),
                        ProductPartRecord_Id = Convert.ToInt32(fields[2], CultureInfo.InvariantCulture),
                        Quantity = Convert.ToInt32(fields[3], CultureInfo.InvariantCulture),
                        UnitPrice = Convert.ToDecimal(fields[4], CultureInfo.InvariantCulture),
                        GSTRate = Convert.ToDecimal(fields[5], CultureInfo.InvariantCulture),
                        Description = StringDecode(fields[6]),
                        Sku = StringDecode(fields[7])
                    };
                    details.Add(detail);
                }
            }
            return details;
        }
        private static string StringEncode(string s)
        {
            var result = s.Replace("\t", "\\t");
            return result.Replace("\r", "\\r");
        }
        private static string StringDecode(string s)
        {
            var result = s.Replace("\\t", "\t");
            return result.Replace("\\r", "\r");
        }

    }
}