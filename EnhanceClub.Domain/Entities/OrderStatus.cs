using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class OrderStatus : OrderDetailMinimal
    {

        public bool OrderInvoiceCpPaymentPending { get; set; }
        public bool OrderInvoiceActive { get; set; }
        public int OrderInvoiceCustomerFk { get; set; }

        public int ShippingInvoiceId { get; set; }
        public int ShippingInvoicePharmacyFk { get; set; }
        public bool ShippingInvoiceShipped { get; set; }
        public DateTime? ShippingInvoiceShippingDate { get; set; }
        public DateTime? ShippingInvoiceAlpsShippingDate { get; set; }
        public bool ShippingInvoicePharmacyExported { get; set; }
        public DateTime? ShippingInvoiceDeletedDate { get; set; }
        public int ShippingInvoiceProblemFk { get; set; }
        public bool ShippingInvoiceIncorrectShip { get; set; }
        public bool ShippingInvoiceDocumentReceived { get; set; }
        public DateTime? ShippingInvoiceDoctorApproveDate { get; set; }

        public int StoreFrontId { get; set; }
        public string StoreFrontNamePk { get; set; }

        public int CartCountForOrder { get; set; }
        public bool ShippingInvoiceHasRxItem { get; set; }

        public string OrderState { get; set; }
        public string OrderSubState { get; set; }
        public string OrderStatusColor { get; set; }
        public string OrderBackGroundPosition { get; set; }
        public bool ShippingInvoiceIsPrep { get; set; }
        public string OrderPmtStatus { get; set; }

        public string OrderStatusInfo { get; set; }
        public string OrderSubStatusInfo { get; set; }

        public int PrescriptionId { get; set; }

        public int PrescriptionApprovalStatus { get; set; }

        public int QuestionnaireId { get; set; }

        public int OrderinvoicePaymentTransactionTypeFk { get; set; }

        public string ShippingInvoiceTrackinCode { get; set; }

        public bool ShippingInvoiceDispensed { get; set; }

        public string OrderInvoiceShippingProvinceName { get; set; }

    }
}
