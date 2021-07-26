namespace Ecommerce_API.Models.Users
{
    public class User
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public string Username{get;set;}
        public string Password{get;set;}
        public string Token{get;set;}
        public bool isAdmin{get;set;}
        public bool isSeller {get;set;}
        public bool isDelivery{get;set;}
        public string Email{get;set;}
        public string Address{get;set;}
        public string Phone{get;set;}
        public string Mobile{get;set;}
        public int ParentID{get;set;}
        public int Status{get;set;}

    }
}