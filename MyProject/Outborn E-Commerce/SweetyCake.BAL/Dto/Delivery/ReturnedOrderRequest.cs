using OutbornE_commerce.BAL.Dto.Delivery.DeliveryOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutbornE_commerce.BAL.Dto.Delivery
{
    public class ReturnedOrderRequest
    {
        // رقم المرجع الفريد للطلب
        public string Reference { get; set; }

        // رقم التتبع الداخلي الخاص بالطلب (اختياري)
        public string? TrackingNumber { get; set; }

        // معرف الموقع الذي سيتم استلام الطلب منه (اختياري)
        public int RetailerLocationIdentifier { get; set; }

        //// تفاصيل موقع الاستلام من التاجر (إذا لم يتم استخدام معرف الموقع)
        //public RetailerLocation? RetailerLocation { get; set; }

        public CustomerInfo CustomerInfo { get; set; }

        // هل هو طلب إرجاع؟ (تحديد إذا كان الطلب عبارة عن إرجاع)
        public bool IsReverse { get; set; } = true;

        // قائمة بالعناصر التي يتم إرجاعها، مع التفاصيل مثل الكمية والسعر والوصف
        public List<ProductItem> Items { get; set; }
    }

    public class RetailerLocation
    {
        // اسم موقع التاجر
        public string Name { get; set; }

        // رقم الهاتف الخاص بموقع التاجر
        public string Phone { get; set; }

        // مدينة موقع التاجر
        public string City { get; set; }

        // دولة موقع التاجر
        public string Country { get; set; }

        // رمز البلد لموقع التاجر
        public string CountryCode { get; set; }

        // العنوان الرئيسي لموقع التاجر
        public string AddressLine1 { get; set; }

        // العنوان الثانوي لموقع التاجر (اختياري)
        public string AddressLine2 { get; set; }

        // المنطقة التي يوجد بها موقع التاجر
        public string Area { get; set; }

        // اسم الشارع لموقع التاجر
        public string Street { get; set; }

        // اسم المبنى لموقع التاجر
        public string Building { get; set; }

        // المعلم القريب لموقع التاجر
        public string LandMark { get; set; }
    }
}