using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMarkting.Models
{
    public class ViewSpecifVM
    { 
        // product
        public int Id_Product { get; set; }
        public string Name_Product { get; set; }
        public string Image_Product { get; set; }
        public string Description_Product { get; set; }
        public string Price_Product { get; set; }
        public Nullable<int> Cat_id { get; set; }
        public Nullable<int> User_id { get; set; }

        // category
        public int Id_Category { get; set; }
        public string Name_Category { get; set; }
        // user
        public string Name_User { get; set; }
        public string Image_User { get; set; }
        public string Contact_User { get; set; }
    }
}