using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace safetys4.App_Code
{
    public class Permission
    {

        public static bool checkPermision(string permission,ArrayList per)
        {
           
            bool result = false;

            if(per!=null)
            {
               
                if (per.IndexOf(permission) > -1)
                {
                   result = true;
                }

            
            }
           
            return result;
        }


    }
}