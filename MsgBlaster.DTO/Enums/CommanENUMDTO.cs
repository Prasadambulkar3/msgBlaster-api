using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsgBlaster.DTO.Enums
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

    #region " Enum for Error handling "
        public enum ErrorFlag
        {
            Success,            // If Web Method Executed Successfully & data is saved in the admin database.
            Failure,            //If Web Method Executed but if there are any problem during data saving to the server.
            InvalidUser,        // If CDKey & MacineID not matches when user request for web method execution.
            DataCorrupted,      // If checksum not matching.
            FailedToWriteData,  // If xml file path not found while writing sms in the xml file.
            SqlException,       // If there are any problem while executing sql query.
            RequestCanceled,    // Web Exception
            ProtocolError,      // Web Exception
            Timeout,             // Web Exception
            ConnectFailure,      // Web Exception
            ConnectionClosed,        // Web Exception
            UnknownError,                // Web Exception
            CrossedMaxRegSenderNumber,      // If Sender number registration of the requested CDKey is crossed the max limit that is 5.
            InValidNumberOrValidationKey,       // If validation key is not enterd correctly while registation of sender number.
            InValidSerialKey,                   // If while registration of MsgBlaster CDKey not matching.
            ReRegister,                          //  If user request for re - registartion of MsgBlaster
            BadXml,              // If xmlpacket is not in required formate.
            InsufficientCredits, // If required credits to send this message is less
            CreditRequestFailed, // IF Credit request not set
            RegisteredMobileNo,  // If Mobile No is Already registerd
            InvalidString,  // If Sender string is already registered
            NotRegisteredYet, //CDKey & MachineID is valid but IsRegistered is False
            NotNeedToUnRegister //Register Count is greater than 0
        }

        //public ErrorFlag GetErrorFlag { get; set; }

        #endregion

}
