using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;

namespace msgBlasterCampaignSendbywihz
{
    public class General
    {
        #region "Public methods"

        /// <summary>
        /// Method to maintain Application Log.
        /// </summary>
        /// <param name="sErrorMessage"></param>
        public static void WriteInFile(string sErrorMessage)
        {
            try
            {
                FileStream fs = new FileStream(ConfigurationManager.AppSettings["LOGQDIR_PATH"].ToString() + System.DateTime.Now.Day + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Year + "Log.txt", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.BaseStream.Seek(0, SeekOrigin.End);
                if (!string.IsNullOrEmpty(sErrorMessage))
                {
                    sw.WriteLine(sErrorMessage);
                }
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
