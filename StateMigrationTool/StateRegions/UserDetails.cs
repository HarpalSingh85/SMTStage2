using System;
using System.IO;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using StateMigrationBackend.Models;

namespace StateMigrationBackend.StateRegions
{
   public class UserDetails : IUserDetails
    {
        public async Task<Person> GetUserDetails(string _userID)
        {            
            Person objPerson = new Person();
                        
            objPerson = await Task.Factory.StartNew(() =>
            {
                Person oPerson = new Person();                

                try
                {
                    using (var context = new PrincipalContext(ContextType.Domain))
                    {
                        using (var user = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, _userID))

                        {
                            if (user != null)
                            {
                                DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + user.DistinguishedName);
                                oPerson.Name = $"Username : {dirEntry?.InvokeGet("Name").ToString().Substring(3)}";
                                var _company = dirEntry?.InvokeGet("COMPANY");
                                if (_company != null)
                                {
                                    oPerson.Company = $"Company : {dirEntry?.InvokeGet("COMPANY").ToString()}";
                                }
                                else
                                {
                                    oPerson.Company = $"Company : Generic";
                                }
                                var _userhomedir = dirEntry?.InvokeGet("homeDirectory");
                                if (_userhomedir != null)
                                {
                                    oPerson.ProfilePathDirectory = dirEntry?.InvokeGet("homeDirectory").ToString();
                                }
                                else
                                {
                                    oPerson.ProfilePathDirectory = null;
                                }
                                if (Directory.Exists($@"C:\Users\{_userID}"))
                                {
                                    oPerson.HomeDirectory = $@"C:\Users\{_userID}";
                                }
                                else
                                {
                                    oPerson.HomeDirectory = null;
                                }


                                dirEntry.Close();
                                user.Dispose();
                            }
                        }
                    }

                }
                

                catch(PrincipalServerDownException)
                {
                    oPerson.Name = $"Username : {Environment.UserName}";
                    oPerson.Company = $"Company : Not Identified";
                    oPerson.HomeDirectory =  Environment.GetEnvironmentVariable("USERPROFILE");
                    oPerson.ProfilePathDirectory = null;

                }

                catch(Exception)
                {
                    throw;
                }

                return oPerson;
            });
            
            return objPerson;
        }
           
    }
       
}

