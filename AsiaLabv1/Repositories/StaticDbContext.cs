using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AsiaLabv1.Repositories
{
    public class StaticDbContext
    {
        protected static AsiaLabDbEntities1 Context = new AsiaLabDbEntities1();
        public static AsiaLabDbEntities1 context
        {
            get
            {
                return Context;
            }
        }
    }
}