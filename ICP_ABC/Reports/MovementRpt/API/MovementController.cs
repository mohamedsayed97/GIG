using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ICP_ABC.Models;


namespace ICP_ABC.Reports.MovementRpt.API
{
    public class MovementController : ApiController
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        //Context _db = new Context();
        //[HttpGet]
        [Route("GetEName/{Code}")]
        public IHttpActionResult CustomerName(string Code)
        {
            var ename = "";
            if (Code == null || Code == "")
                ename = "";
            else
            {
                var Customer = _db.Customers.Where(name=>name.ArName == Code || name.EnName == Code).FirstOrDefault() ;
                if (Customer == null)
                    ename = "";
                else
                    ename = Customer.CustomerID;
            }

            return Ok(ename);
        }
    }
}
