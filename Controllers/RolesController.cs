using Cliente_REST_CRUD_SQL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace Cliente_REST_CRUD_SQL.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _httpClient;

        public RolesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7211/");
        }

        public async Task<IActionResult> Index()
        {
            var xmlData = await GetRolesFromApi();
            var roles = DeserializeRoles(xmlData);
            return View(roles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await GetRoleFromApi(id);
            return View(role);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Class_Role role)
        {
            if (ModelState.IsValid)
            {
                await CreateRoleInApi(role);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> Edit(int id)   
        {
            var role = await GetRoleFromApi(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Class_Role role)
        {
            if (id != role.RoleID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // Agregamos una depuración para verificar si IsActive es el valor esperado

                Debug.WriteLine($"Role IsActive: {role.IsActive}");

                // Guardar los cambios en la API
                var response = await _httpClient.PutAsJsonAsync($"api/Roles/{id}", role);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index)); // Redirecciona si se guardan los cambios
                }
            }

            // Si llegamos aquí, algo falló, devuelve el modelo a la vista
            return View(role);
        }


        public async Task<IActionResult> Delete(int id)
        {
            // Obtenemos el rol desde la API
            var role = await GetRoleFromApi(id);

            if (role == null)
            {
                return NotFound(); // En caso de que el rol no exista
            }

            return View(role); // Devolvemos el rol para confirmar la eliminación
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await DeleteRoleFromApi(id); // Llamamos a la API para eliminar el rol
            Debug.WriteLine("Redirecionando a Index....");
            return RedirectToAction(nameof(Index)); // Redirigimos al índice de roles
        }


        private async Task<string> GetRolesFromApi()
        {
            var response = await _httpClient.GetAsync("api/Roles");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<Class_Role> GetRoleFromApi(int id)
        {
            var response = await _httpClient.GetAsync($"api/Roles/{id}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync(); // Leemos el contenido como cadena
            var serializer = new XmlSerializer(typeof(Class_Role)); // Creamos el serializador para la clase

            using (var reader = new StringReader(xmlContent))
            {
                return (Class_Role)serializer.Deserialize(reader); // Deserializamos el XML
            }
        }


        private async Task CreateRoleInApi(Class_Role role)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Roles", role);
            response.EnsureSuccessStatusCode();
        }

        private async Task UpdateRoleInApi(Class_Role role)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Roles/{role.RoleID}", role);
            response.EnsureSuccessStatusCode();
        }

        private async Task DeleteRoleFromApi(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Roles/{id}");
            response.EnsureSuccessStatusCode();
        }

        private Class_RoleList DeserializeRoles(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return new Class_RoleList { Class_Roles = new List<Class_Role>() };
            }

            var serializer = new XmlSerializer(typeof(Class_RoleList));
            using (var reader = new StringReader(xml))
            {
                return (Class_RoleList)serializer.Deserialize(reader);
            }
        }

        public async Task<IActionResult> Buscar(string query)
        {
            // Primero, obtenemos los roles desde el XML
            var xmlData = await GetRolesFromApi();
            var roles = DeserializeRoles(xmlData);

            // Si hay un término de búsqueda, filtramos los roles
            if (!string.IsNullOrWhiteSpace(query))
            {
                roles.Class_Roles = roles.Class_Roles
                    .Where(r => r.RoleName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                r.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Retornamos la vista con los roles filtrados
            return View("Index", roles);
        }



    }
}
