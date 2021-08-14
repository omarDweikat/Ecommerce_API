using System.Collections.Generic;
namespace Ecommerce_API.Models.Item
{
    public class GalleryItem
    {
        public int ID{get;set;}
        public int Price{set;get;}

        public string Name{get;set;}
        public string Description{get;set;}
        public int CategoryID{get;set;}
        public int CreationUserID{get;set;}
        public string CreationTimestamp{get;set;}
        public int LastUpdateUserID {get;set;}
        public string LastUpdateTimestamp{get;set;}       
        public string FileName{get;set;}       
        public int Status{get;set;}
        
    }
}