namespace EnhanceClub.Domain.LogEntities
{
    public class LogUserAction
    {
        public int LogUserActionTypeFk { get; set; }

        public string LogUserActionName { get; set; }

        public string LogUserActionParams { get; set; }

        public string LogUserActionQuery { get; set; }

        public int LogUserActionStoreFrontFk { get; set; }

        public string LogActionStoreFrontName { get; set; }

        public string LogActionUserIp { get; set; }

        public int LogActionCustomerFk { get; set; }

        public string LogActionCustomerName { get; set; }

        public int LogActionOrderInvoiceFk { get; set; }

        public int LogActionShippingInvoiceFk { get; set; }

        public string LogActionCustomerEmail { get; set; }

        public string LogUserActionDatabaseException { get; set; }

        public int LogUserActionErrorFk { get; set; }
    }
}
