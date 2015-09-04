using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Net;
using System.Configuration;
using System.IO;

using MsgBlaster.Domain;
using MsgBlaster.DTO;
using MsgBlaster.Repo;
using MsgBlaster.Service;
using System.Data.Entity;


namespace ReadAndUploadSQLiteData
{
    public class ReadSqliteData
    {
        public static void GetFolderList()
        {
            string[] folders = Directory.GetDirectories(ConfigurationManager.AppSettings["SQLiteDatabaseFolder"].ToString(), "*", System.IO.SearchOption.AllDirectories);
            int TotalFolders = folders.Count();

            try
            {
                foreach (var folder in folders)
                {
                    try
                    {
                        string[] files = System.IO.Directory.GetFiles(folder, "*");
                        string folderpath = folder;
                        string[] words = folderpath.Split('\\');
                        int ClientId = 0;
                        foreach (string word in words)
                        {
                            try
                            {
                                ClientId = Convert.ToInt32(word);
                            }
                            catch
                            {
                                continue;
                            }
                        }                        
                        try
                        {
                            foreach (var file in files)
                            {
                                string filepath = file;

                                // Check ClientPresentOrNot
                                ClientDTO ClientDTO = new ClientDTO();
                                ClientDTO = ClientService.GetById(ClientId);
                                if (ClientDTO != null)
                                { 
                                    DataTable dtContacts = GetAllContacts(filepath);
                                   // DataTable dtClients = GetClients(filepath);
                                   
                                    
                                    ReadSqlite(ClientId, filepath);
                                    SaveOtherContact(ClientId, filepath);

                                    if (File.Exists(filepath))
                                    {
                                        File.Delete(filepath);
                                    }
                                }

                               
                              


                            }
                        }
                        catch
                        { 
                            continue;
                        }
                    }
                    catch 
                    { 
                        continue; 
                    }
                }
            }
            catch
            {
                 
            }
        }

        public static void ReadSqlite(int ClientId, string sqlitefile) 
        {            
            int NewGroupId = 0;
            int GrpId = 0;
            try
            {

                DataTable dtGroupContacts = GetGroupContacts(sqlitefile);
                int GroupId = 0;
                for (int i = 0; (i <= (dtGroupContacts.Rows.Count - 1)); i++)
                {
                    try
                    {
                        int ContactId = 0;

                        if (!Convert.IsDBNull(dtGroupContacts.Rows[i]["GroupID"].ToString()))
                        {
                            GroupId = Convert.ToInt32( dtGroupContacts.Rows[i]["GroupID"].ToString());
                        }
                        else
                        {
                            GroupId = 0;
                        }

                        if (!Convert.IsDBNull(dtGroupContacts.Rows[i]["ContactID"].ToString()))
                        {
                            ContactId = Convert.ToInt32(dtGroupContacts.Rows[i]["ContactID"].ToString());
                        }
                        else
                        {
                            ContactId = 0;
                        }

                        DataTable dtGroups = GetGroups(sqlitefile, GroupId);
                        for (int j = 0; (j <= (dtGroups.Rows.Count - 1)); j++)
                        {
                            try
                            {
                                //int chkGroupid = 0;
                                    string Groupname = "";
                                    if (!Convert.IsDBNull(dtGroups.Rows[j]["GroupName"].ToString()))
                                    {
                                        Groupname = dtGroups.Rows[j]["GroupName"].ToString();
                                    }
                                    else
                                    {
                                        Groupname = "";
                                    }

                                    
                                    //if (!Convert.IsDBNull(dtGroups.Rows[j]["GroupID"].ToString()))
                                    //{
                                    //    GrpId = Convert.ToInt32(dtGroups.Rows[j]["GroupID"].ToString());
                                    //}
                                    //else
                                    //{
                                    //    GrpId = 0;
                                    //}


                                    if (GroupId != GrpId)
                                    {
                                        // Check group name already present or not
                                        GroupDTO GroupDTOPresent = new GroupDTO();
                                        GroupDTOPresent = GroupService.GetGroupByNameAndClientId(Groupname, ClientId);
                                        if (GroupDTOPresent.Id == 0)
                                        {
                                            ////CreateGroup
                                            GroupDTO GroupDTO = new GroupDTO();
                                            GroupDTO.Name = Groupname;
                                            GroupDTO.ClientID = ClientId;
                                            NewGroupId = GroupService.Create(GroupDTO);
                                        }
                                        else 
                                        {
                                            NewGroupId = GroupDTOPresent.Id;
                                        }

                                    }

                                    DataTable dtContacts = GetContactsById(sqlitefile, ContactId);
                              
                                for (int k = 0; (k <= (dtContacts.Rows.Count - 1)); k++)                                
                                {
                                    try 
                                    {
                                         string FirstName;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["FirstName"].ToString()))
                                         {
                                             FirstName = dtContacts.Rows[k]["FirstName"].ToString();
                                         }
                                         else
                                         {
                                             FirstName = "";
                                         }

                                         string LastName;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["LastName"].ToString()))
                                         {
                                             LastName = dtContacts.Rows[k]["LastName"].ToString();
                                         }
                                         else
                                         {
                                             LastName = "";
                                         }

                                         string FullName = FirstName + " " + LastName;

                                         string Mobile;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["MobileNumber"].ToString()))
                                         {
                                             Mobile = dtContacts.Rows[k]["MobileNumber"].ToString();
                                             if (Mobile == "") { continue; }
                                         }
                                         else
                                         {
                                             Mobile = "";
                                         }


                                         string Gender;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["Gender"].ToString()))
                                         {
                                             Gender = dtContacts.Rows[k]["Gender"].ToString();
                                             if (Gender == "") 
                                             { 
                                                 Gender = "Male"; 
                                             }
                                         }
                                         else
                                         {
                                             Gender = "Male";
                                         }


                                         string BirthDate;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["BirthDate"].ToString()))
                                         {
                                             BirthDate = dtContacts.Rows[k]["BirthDate"].ToString();
                                         }
                                         else
                                         {
                                             BirthDate =  null;
                                         }


                                         string Anniversary;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["Anniversary"].ToString()))
                                         {
                                             Anniversary = dtContacts.Rows[k]["Anniversary"].ToString();
                                         }
                                         else
                                         {
                                             Anniversary =  null;
                                         }


                                         string Email;
                                         if (!Convert.IsDBNull(dtContacts.Rows[k]["EmailID"].ToString()))
                                         {
                                             Email = dtContacts.Rows[k]["EmailID"].ToString();
                                         }
                                         else
                                         {
                                             Email = null;
                                         }


                                          


                                         //Check Contact Present or not
                                         ContactDTO ContactDTO = new ContactDTO();
                                         ContactDTO = ContactService.GetContactByMobileNumberAndClientId(Mobile, ClientId);
                                         if (ContactDTO.Id == 0)
                                         {
                                             ////Create Contact
                                             ContactDTO ContactDTONew = new ContactDTO();
                                             ContactDTONew.Name = FullName;
                                             if (CommonService.IsDate(BirthDate))
                                             {
                                                DateTime? bdate = Convert.ToDateTime( BirthDate);
                                                 ContactDTONew.BirthDate = bdate;
                                             }

                                             if (CommonService.IsDate(Anniversary))
                                             {
                                                 DateTime? anniversarydate = Convert.ToDateTime(Anniversary);
                                                 ContactDTONew.BirthDate = anniversarydate;
                                             }                           
                                             ContactDTONew.Email = Email;
                                             ContactDTONew.ClientId = ClientId;

                                             ContactDTONew.Gender = Gender;
                                             ContactDTONew.MobileNumber = Mobile;

                                             ContactDTONew.FirstName = FirstName;
                                             ContactDTONew.LastName = LastName;
                                             GroupDTO GroupDTO = GroupService.GetById(NewGroupId);
                                             List<GroupDTO> Groups = new List<GroupDTO>();                                            
                                             Groups.Add(GroupDTO);
                                             ContactDTONew.Groups = Groups;
                                          
                                             if (ContactDTONew.FirstName.Length > 50)
                                             { 
                                                 continue; 
                                             }

                                             if (ContactDTONew.LastName.Length > 50)
                                             { 
                                                 continue; 
                                             }
                                             ContactService.Create(ContactDTONew);
                                    
                                         }
                                    }
                                    catch 
                                    { 
                                        continue; 
                                    }

                                }

                                GrpId = GroupId;
                            }
                            catch
                            {
                                continue;
                            }                            
                        }
                        


                    }
                    catch 
                    { 
                        continue; 
                    }
                }

               
                
               
                
            }                
            catch(Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "ReadSqlite" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                }
            }            
             
        }

        public static DataTable GetGroupContacts(string sqlitefile)
        {
            DataTable dt = new DataTable("GroupContacts");
            try
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + sqlitefile);
                
                dt = ExecuteNonQueryDt("Select * From GroupContact Order by GroupID", con);
                return dt;
            }
            catch (Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "GetGroupContacts" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                } 
                return dt;
            }
        }

        public static DataTable GetGroups(string sqlitefile, int GroupId)
        {
            DataTable dt = new DataTable("Group");
            try
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + sqlitefile);
                
                // dt = ExecuteNonQueryDt("Select * From [Group] Where GroupId != 1 AND (FilterCondition == '' or FilterCondition is NULL)", con);
                dt = ExecuteNonQueryDt("Select * From [Group] Where GroupId =" + GroupId, con);
                return dt;
            }
            catch (Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "GetGroupContacts" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                }
                return dt;
            }
        }

        public static DataTable GetContactsById(string sqlitefile, int ContactId)
        {
            DataTable dt = new DataTable("Contact");
            try
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + sqlitefile);                
                dt = ExecuteNonQueryDt("Select * From Contact Where ContactID=" + ContactId, con);
                return dt;
            }
            catch (Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "GetGroupContacts" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                }
                return dt;
            }
        }

        public static DataTable GetAllContacts(string sqlitefile)
        {
            DataTable dt = new DataTable("Contact");
            try 
            { 
            SQLiteConnection con = new SQLiteConnection("data source=" + sqlitefile);
            
            dt = ExecuteNonQueryDt("Select * From Contact Order By ContactID", con);
            return dt;
            }
            catch(Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "GetAllContacts" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                }

                if (File.Exists(sqlitefile))
                {
                   File.Delete(sqlitefile);
                } 
                return dt;
            }
        }

        public static DataTable ExecuteNonQueryDt(string cmdText, SQLiteConnection con)
        {
            DataTable dt = new DataTable("Table");
            try
            {
                
                using (con)
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmdText, con);
                    con.Open();
                    da.Fill(dt);
                    con.Close();
                }
                return dt;
            }
            catch (Exception ex)
            {
                using (FileStream file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + System.DateTime.Now.Date.ToString("dd-MMM-yyyy") + "_Log.txt", FileMode.Append, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(System.DateTime.Now + " - " + "ExecuteNonQueryDt" + " - " + ex.Message.ToString());
                    streamWriter.Close();
                }
                return dt;
            }

        }




        public static DataTable GetClients(string sqlitefile)
        {
            SQLiteConnection con = new SQLiteConnection("data source=" + sqlitefile);
            DataTable dt = new DataTable("CompanyInfo");
            dt = ExecuteNonQueryDt("SELECT * FROM CompanyInfo", con);
            return dt;
        }


        public static void SaveOtherContact(int Clientid, string sqlitefile)
        {
            DataTable dtContacts = GetAllContacts(sqlitefile);
            for (int k = 0; (k <= (dtContacts.Rows.Count - 1)); k++)
            {
                try
                {

                    

                    string FirstName;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["FirstName"].ToString()))
                    {
                        FirstName = dtContacts.Rows[k]["FirstName"].ToString();
                    }
                    else
                    {
                        FirstName = "";
                    }

                    string LastName;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["LastName"].ToString()))
                    {
                        LastName = dtContacts.Rows[k]["LastName"].ToString();
                    }
                    else
                    {
                        LastName = "";
                    }

                    string FullName = FirstName + " " + LastName;

                    string Mobile;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["MobileNumber"].ToString()))
                    {
                        Mobile = dtContacts.Rows[k]["MobileNumber"].ToString();
                        if (Mobile == "") { continue; }
                    }
                    else
                    {
                        Mobile = "";
                    }


                    string Gender;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["Gender"].ToString()))
                    {
                        Gender = dtContacts.Rows[k]["Gender"].ToString();
                        if (Gender == "") 
                        { 
                            Gender = "Male"; 
                        }
                    }
                    else
                    {
                        Gender = "Male";
                    }


                    string BirthDate;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["BirthDate"].ToString()))
                    {
                        BirthDate = dtContacts.Rows[k]["BirthDate"].ToString();
                    }
                    else
                    {
                        BirthDate = null;
                    }


                    string Anniversary;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["Anniversary"].ToString()))
                    {
                        Anniversary = dtContacts.Rows[k]["Anniversary"].ToString();
                    }
                    else
                    {
                        Anniversary = null;
                    }


                    string Email;
                    if (!Convert.IsDBNull(dtContacts.Rows[k]["EmailID"].ToString()))
                    {
                        Email = dtContacts.Rows[k]["EmailID"].ToString();
                    }
                    else
                    {
                        Email = null;
                    }


                    


                    //Check Contact Present or not
                    ContactDTO ContactDTO = new ContactDTO();
                    ContactDTO = ContactService.GetContactByMobileNumberAndClientId(Mobile, Clientid);
                    if (ContactDTO.Id == 0)
                    {
                        ////Create Contact
                        ContactDTO ContactDTONew = new ContactDTO();
                        ContactDTONew.Name = FullName;
                        if (CommonService.IsDate(BirthDate))
                        {
                            DateTime? bdate = Convert.ToDateTime(BirthDate);
                            ContactDTONew.BirthDate = bdate;
                        }

                        if (CommonService.IsDate(Anniversary))
                        {
                            DateTime? anniversarydate = Convert.ToDateTime(Anniversary);
                            ContactDTONew.BirthDate = anniversarydate;
                        }

                        ContactDTONew.FirstName = FirstName;
                        ContactDTONew.LastName = LastName;
                        ContactDTONew.Email = Email;
                        ContactDTONew.ClientId = Clientid;
                        ContactDTONew.Gender = Gender;
                        ContactDTONew.MobileNumber = Mobile;

                        if (ContactDTONew.FirstName.Length > 50)
                        {
                            continue;
                        }

                        if (ContactDTONew.LastName.Length > 50)
                        {
                            continue;
                        }

                        ContactService.Create(ContactDTONew);

                    }
                }
                catch
                {
                    continue;
                }

            }
         
        }


         
    }
}
