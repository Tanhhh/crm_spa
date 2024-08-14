using Erp.Domain.Entities;
using System.Collections.Generic;

namespace Erp.BackOffice.Administration.Models
{
    public class ListUsersTypeModel
    {
        public IEnumerable<vwUserType> UserTypes { get; set; }
    }
}