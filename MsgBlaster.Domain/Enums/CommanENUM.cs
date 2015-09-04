using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MsgBlaster.Domain
{
        public enum PartnerType
        {
            Admin, Partner, User
        }

        public enum Gender
        {
            Male, Female
        }

        public enum GatewayType
        {
            Transactional, Promotional
        }

        public enum MaritalStatus
        {
            Unmarried, Married, Divorced
        }

        public enum PaymentMode
        {
            Card, Cheque, BankDeposit //Cash,
        }

        public enum Macros
        {
            FirstName, LastName, Code, BirthDate, AnniversaryDate, Email, MobileNumber, Gender, ExpiresOn
        }

        public enum UserType
        {
            Admin, Redeem, Normal
        }

        public enum CouponExpireType
        {
            Day, Week, Month
        }

        public enum CampaignStatus
        {
            Unsend, Sent, Cancelled
        }
}
