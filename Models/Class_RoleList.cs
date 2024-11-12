using System.Collections.Generic;
using System.Xml.Serialization;

namespace Cliente_REST_CRUD_SQL.Models
{
    [XmlRoot("ArrayOfClass_Role")]
    public class Class_RoleList
    {
        [XmlElement("Class_Role")]
        public List<Class_Role> Class_Roles { get; set; } = new List<Class_Role>(); // Inicialización de lista
    }
}
