using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class OrderTransaction
    {
        public int OrdertransactionId { get; set; }

        public int? OrdertransactionCustomerFk { get; set; }

        public int? OrdertransactionOrderinvoiceFk { get; set; }

        public int? OrdertransactionOrdertempFk { get; set; }

        public int OrdertransactionPaymentoptionFk { get; set; }

        public string OrdertransactionCardcvv2Code { get; set; }

        public string OrdertransactionCardname { get; set; }

        public string OrdertransactionCardnum { get; set; }

        public string OrdertransactionCardtype { get; set; }

        public string OrdertransactionIp { get; set; }

        public string OrdertransactionStatusmessage { get; set; }

        public string OrdertransactionEbayid { get; set; }

        public string OrdertransactionPaypalid { get; set; }

        public string OrdertransactionTransactionnum { get; set; }

        public decimal OrdertransactionAmount { get; set; }

        public int? OrdertransactionCardcvv2Type { get; set; }

        public int? OrdertransactionCardexpiremonth { get; set; }

        public int? OrdertransactionCardexpireyear { get; set; }

        public bool OrdertransactionError { get; set; }

        public DateTime OrdertransactionDatecreated { get; set; }

        public DateTime OrdertransactionLastmodified { get; set; }

        public int? OrdertransactionStorefrontFk { get; set; }

        public int? OrdertransactionPpUseradminFk { get; set; }

        public int? OrdertransactionChequereceived { get; set; }

        public DateTime? OrdertransactionChequereceiveddate { get; set; }

        public int? OrdertransactionChequeUseradminFk { get; set; }

        public decimal? OrdertransactionChequeAmount { get; set; }

        public string OrdertransactionChequeNumber { get; set; }

        public string OrderTransactionCardExpiry
        {
            get
            {
                if (OrdertransactionCardexpiremonth > 0 && OrdertransactionCardexpireyear > 0)
                {
                    return OrdertransactionCardexpiremonth.ToString().PadLeft(2,
                               Convert.ToChar("0")) + OrdertransactionCardexpireyear.ToString().Substring(2,
                               2);
                }

                return null;
            }
        }

        public int OrdertransactionStatusId { get; set; }

        public bool SetAvsCheck { get; set; }

        public string OrderTransactionPaymentAuthCode { get; set; }

        public int OrderTransactionPaymentTransactionType { get; set; }
    }
}
