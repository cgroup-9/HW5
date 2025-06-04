using hw4.Project;
using Microsoft.AspNetCore.Mvc;

namespace hw4.Project
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Active { get; set; }

        private readonly DBservices db = new DBservices();

        public Users() { }

        public Users(int id, string name, string email, string password, bool active)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Active = active;
        }

        public int Register()
        {
            return db.InsertUser(this);
        }
        public static List<Users> Read()
        {
            DBservices db = new DBservices();
            return db.ReadAllUsers();
        }

        public Users? Login(string email, string password)
        {
            return db.LoginUser(email, password);
        }

        public int Update()
        {
            return db.UpdateUser(this);
        }

        public int SoftDeleteByEmail(string email)
        {
            return db.SoftDeleteUserByEmail(email);
        }
    }
}
