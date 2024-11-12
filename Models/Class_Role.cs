using System;
using System.Xml.Serialization;

namespace Cliente_REST_CRUD_SQL.Models
{
    public class Class_Role
    {
        [XmlElement("RoleID")]
        public int RoleID { get; set; }

        [XmlElement("RoleName")]
        public string RoleName { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("F_Created")]
        public DateTime F_Created { get; set; }

        [XmlElement("F_Updated")]
        public DateTime? F_Updated { get; set; }

        [XmlElement("IsActive")]
        public bool IsActive { get; set; }
    }
}
