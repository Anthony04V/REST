using Cliente_REST_CRUD_SQL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace Cliente_REST_CRUD_SQL.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var xmlData = await GetRolesFromApi(); // Método que obtiene el XML
            var roles = DeserializeRoles(xmlData); // Deserializa el XML

            return View(roles); // Pasa el objeto deserializado a la vista
        }

        private async Task<string> GetRolesFromApi()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7211/");
                var response = await client.GetAsync("api/Roles");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        private Class_RoleList DeserializeRoles(string xml)
        {
            var serializer = new XmlSerializer(typeof(Class_RoleList));

            using (var reader = new StringReader(xml))
            {
                return (Class_RoleList)serializer.Deserialize(reader);
            }
        }
    }
}
