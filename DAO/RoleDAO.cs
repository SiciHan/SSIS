using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Team8ADProjectSSIS.Models;

namespace Team8ADProjectSSIS.DAO
{
    public class RoleDAO
    {
        private readonly SSISContext context;
        public RoleDAO()
        {
            this.context = new SSISContext();
        }

        public string FindRoleLabelById(int id)
        {
            return context.Roles.OfType<Role>().Where(r => r.IdRole == id).Select(r => new { r.Label }).OfType<string>().FirstOrDefault();
        }

    }
}