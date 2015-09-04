using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MsgBlaster.DTO
{
    public class MessageLogDTO
    {
        #region " Declration "

        private int _mMessageLogID;
        private string _mMessageText;
        private string _mMessageDateTime;
        private string _mDateFrom;
        private string _mDateTo;
        private int _mCount;
        private string _mSenderNumber;
        private int _mMessageTemplateID;
        private string _mMessageTemplateType;
        private string _mRecipientType;
        private string _mRecipients;
        private string _mMessageScheduledDateTime;
        private string _mScheduledMessageCampaign;
        public Dictionary<string, object> _fieldCollection = new Dictionary<string, object>();
        #endregion

        #region " Properties "

        public string ScheduledMessageCampaign
        {
            get { return _mScheduledMessageCampaign; }
            set
            {
                _mScheduledMessageCampaign = value;
                if (_fieldCollection.ContainsKey("ScheduledMessageCampaign"))
                    _fieldCollection["ScheduledMessageCampaign"] = value;
                else
                    _fieldCollection.Add("ScheduledMessageCampaign", value);
            }
        }

        public string Recipients
        {
            get { return _mRecipients; }
            set
            {
                _mRecipients = value;
                if (_fieldCollection.ContainsKey("Recipients"))
                    _fieldCollection["Recipients"] = value;
                else
                    _fieldCollection.Add("Recipients", value);
            }
        }

        public string RecipientType
        {
            get { return _mRecipientType; }
            set
            {
                _mRecipientType = value;
                if (_fieldCollection.ContainsKey("RecipientType"))
                    _fieldCollection["RecipientType"] = value;
                else
                    _fieldCollection.Add("RecipientType", value);
            }
        }

        public int MessageLogID
        {
            get { return _mMessageLogID; }
            set { _mMessageLogID = value; }
        }
        public string MessageText
        {
            get { return _mMessageText; }
            set
            {
                _mMessageText = value;
                if (_fieldCollection.ContainsKey("MessageText"))
                    _fieldCollection["MessageText"] = value;
                else
                    _fieldCollection.Add("MessageText", value);
            }
        }
        public string MessageDateTime
        {
            get { return _mMessageDateTime; }
            set
            {
                _mMessageDateTime = value;
                if (_fieldCollection.ContainsKey("MessageDateTime"))
                    _fieldCollection["MessageDateTime"] = value;
                else
                    _fieldCollection.Add("MessageDateTime", value);
            }
        }
        public int Count
        {
            get { return _mCount; }
            set
            {
                _mCount = value;
                if (_fieldCollection.ContainsKey("Count"))
                    _fieldCollection["Count"] = value;
                else
                    _fieldCollection.Add("Count", value);
            }
        }

        public string SenderNumber
        {
            get { return _mSenderNumber; }
            set
            {
                _mSenderNumber = value;
                if (_fieldCollection.ContainsKey("SenderNumber"))
                    _fieldCollection["SenderNumber"] = value;
                else
                    _fieldCollection.Add("SenderNumber", value);
            }
        }

        public int MessageTemplateID
        {
            get { return _mMessageTemplateID; }
            set
            {
                _mMessageTemplateID = value;
                if (_fieldCollection.ContainsKey("MessageTemplateID"))
                    _fieldCollection["MessageTemplateID"] = value;
                else
                    _fieldCollection.Add("MessageTemplateID", value);
            }
        }

        public string MessageTemplateType
        {
            get { return _mMessageTemplateType; }
            set
            {
                _mMessageTemplateType = value;
                if (_fieldCollection.ContainsKey("MessageTemplateType"))
                    _fieldCollection["MessageTemplateType"] = value;
                else
                    _fieldCollection.Add("MessageTemplateType", value);
            }
        }

        public string MessageScheduledDateTime
        {
            get { return _mMessageScheduledDateTime; }
            set
            {
                _mMessageScheduledDateTime = value;
                if (_fieldCollection.ContainsKey("MessageScheduledDateTime"))
                    _fieldCollection["MessageScheduledDateTime"] = value;
                else
                    _fieldCollection.Add("MessageScheduledDateTime", value);
            }
        }

        public string DateFrom
        {
            get { return _mDateFrom; }
            set { _mDateFrom = value; }
        }
        public string DateTo
        {
            get { return _mDateTo; }
            set
            {
                _mDateTo = value;
            }
        }

        #endregion

        //#region " Methods "

        //public int InsertIntoMessageLog(MessageLog objClsMessageLog)
        //{
        //    return dbMessageLog.Insert(objClsMessageLog);
        //}
        //public DataTable GetMessageLog()
        //{
        //    return dbMessageLog.SelectMessageLog();
        //}
        //public DataTable GetMessageQueue()
        //{
        //    return dbMessageLog.SelectMessageQueue();
        //}

        //public DataTable GetLogBetweenDate(DateTime from, DateTime to)
        //{
        //    return dbMessageLog.SelectLogBetDate(from, to);
        //}
        //#endregion
    }
}
