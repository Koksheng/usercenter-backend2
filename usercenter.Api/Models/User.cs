using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace usercenter.Api.Models
{
    public class User 
    {
        public int Id { get; set; }
        public string userName { get; set; }
        public string userAccount { get; set; }
        public string avatarUrl { get; set; }
        public int gender { get; set; }
        [JsonIgnore] // This property will be ignored during serialization and deserialization
        public string userPassword { get; set; }
        public string phone { get; set; }

        public string email { get; set; }
        public int userStatus { get; set; }
        public DateTime createTime { get; set; }
        public DateTime updateTime { get; set; }
        public bool isDelete { get; set; }
        public int userRole { get; set; }
        public string planetCode { get; set; }


        //public User()
        //{
        //    // Parameterless constructor
        //}

        public User(int id, string userName, string userAccount, string avatarUrl, int gender, string userPassword, string phone, string email, int userStatus, DateTime createTime, DateTime updateTime, bool isDelete, int userRole, string planetCode)
        {
            Id = id;
            this.userName = userName;
            this.userAccount = userAccount;
            this.avatarUrl = avatarUrl;
            this.gender = gender;
            this.userPassword = userPassword;
            this.phone = phone;
            this.email = email;
            this.userStatus = userStatus;
            this.createTime = createTime;
            this.updateTime = updateTime;
            this.isDelete = isDelete;
            this.userRole = userRole;
            this.planetCode = planetCode;
        }

        public User(int id, string userName, string userAccount, string avatarUrl, int gender, string phone, string email, int userStatus, DateTime createTime, DateTime updateTime, bool isDelete, int userRole, string planetCode)
        {
            Id = id;
            this.userName = userName;
            this.userAccount = userAccount;
            this.avatarUrl = avatarUrl;
            this.gender = gender;
            this.phone = phone;
            this.email = email;
            this.userStatus = userStatus;
            this.createTime = createTime;
            this.updateTime = updateTime;
            this.isDelete = isDelete;
            this.userRole = userRole;
            this.planetCode = planetCode;
        }

        public User()
        {
            // Parameterless constructor
        }
    }
}
    
