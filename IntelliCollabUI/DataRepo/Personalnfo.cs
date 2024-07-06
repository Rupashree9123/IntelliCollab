using IntelliCollabUI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace IntelliCollabUI.DataRepo
{
    public class Personalnfo
    {
        DbConnect conn = null;
        public Personalnfo()
        {
            conn = new DbConnect();
        }

        //public bool SavePersonalInfo(FormCollection collection)
        //{
        //    try
        //    {
                
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}