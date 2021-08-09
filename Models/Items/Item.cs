namespace Ecommerce_API.Models.Item
{
    public class Item
    {
        public int ID{get;set;}
        public string Name{get;set;}
        public int Price{get;set;}
        public string Description{get;set;}
        public int CategoryID{get;set;}
        public int CreationUserID{get;set;}
        public string CreationTimestamp {get;set;}
        public int LastUpdateUserID{get;set;}
        public int LastUpdateTimestamp{get;set;}
        public int Status{get;set;}

    }
}