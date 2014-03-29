using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNetCachingDemo
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NorthwindEFEntities entities = new NorthwindEFEntities();
            var customers = entities.Customers.ToList();
            this.GridView1.DataSource = customers;
        }
    }
}
