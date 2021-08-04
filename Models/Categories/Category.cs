namespace Ecommerce_API.Models.Categories
{
    public class Category
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public int ParentID{get;set;}
        public int UserID{get;set;}
        public int Status{get;set;}

    }
}