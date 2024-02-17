using Microsoft.AspNetCore.Mvc;
using Productos.Cliente.Models;
using Newtonsoft.Json;
using Productos.Server.Models;
using System.Text;

namespace Productos.Cliente.Controllers
{
    public class ProductoController : Controller
    {
        /// Inyecta el cliente HTTP en el controlador para poder hacer peticiones a la API
        private readonly HttpClient _httpClient;

        /// Constructor del controlador
        /// La instancia IHttpClientFactory facilita la creación y administración de instancias de HttpClient
        public ProductoController(IHttpClientFactory httpClientFactory)
        {
            /// Crea el cliente HTTP
            _httpClient = httpClientFactory.CreateClient();
            /// Establece la URL base de la API usando la url de la API crud
            _httpClient.BaseAddress = new Uri("https://localhost:7245/api");
        }

        /// Método para obtener la lista de productos, puede manejar de manera asincrona la petición y devolver una tarea
        public async Task<IActionResult> Index()
        {
            /// Hace una petición GET a la API para obtener la lista de productos
            /// La URL se concatena con la URL base de la API establecida en el constructor
            /// y forma la URL completa a la que haremos la petición
            var response = await _httpClient.GetAsync("/api/Productos/lista");
            /// Si la petición es exitosa
            if (response.IsSuccessStatusCode)
            {
                /// Leemos el contenido de la respuesta y lo deserializamos a una lista de productos
                var content = await response.Content.ReadAsStringAsync();
                /// Deserializamos el contenido a una lista de productos
                var productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(content);

                /// Retornamos la vista con la lista de productos
                return View("Index", productos);
            }

            /// Si la petición no es exitosa, retornamos la vista con un mensaje de error y una lista vacía
            return View(new List<ProductoViewModel>());
        }

        /// Método para crear un nuevo producto
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                /// Serializamos el producto a JSON
                var json = JsonConvert.SerializeObject(producto);
                /// Creamos el contenido de la petición con el JSON serializado
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                /// Hacemos una petición POST a la API para crear un nuevo producto
                var response = await _httpClient.PostAsync("/api/Productos/crear", content);
                /// Si la petición es exitosa
                if (response.IsSuccessStatusCode)
                {
                    /// Redirigimos al usuario a la lista de productos
                    return RedirectToAction("Index");
                }
                else
                {
                    /// Si la petición no es exitosa, mostramos un mensaje de error
                    ModelState.AddModelError(string.Empty, "Error al crear el producto");
                }

            }
            return View(producto);
        }

        /// Metodo para editar un producto
        public async Task<IActionResult> Edit(int id)
        {
            /// Hacemos una petición GET a la API para obtener el producto con el id especificado
            /// 
            var response = await _httpClient.GetAsync($"/api/Productos/ver?id={id}");
            /// Si la petición es exitosa
            if (response.IsSuccessStatusCode)
            {
                /// Guarda el objeto producto en la variable content
                var content = await response.Content.ReadAsStringAsync();
                /// Deserializa el contenido a un objeto producto
                var producto = JsonConvert.DeserializeObject<ProductoViewModel>(content);
                /// Retorna la vista con los datos precargados del producto
                return View(producto);
            }
            else
            {
                /// Si la petición no es exitosa, redirige al usuario a la lista de productos
                return RedirectToAction("Details");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            /// Si el modelo es válido
            if (ModelState.IsValid)
            {
                /// Serializamos el producto a JSON
                var json = JsonConvert.SerializeObject(producto);
                /// Encriptamos el contenido de la petición con el JSON serializado
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                /// Hacemos una petición PUT a la API para editar el producto
                var response = await _httpClient.PutAsync($"/api/Productos/editar?id={id}", content);

                /// Si la petición es exitosa
                if (response.IsSuccessStatusCode)
                {
                    /// Redirigimos al usuario a la lista de productos con el id del producto
                    return RedirectToAction("Index", new { id });
                }
                else
                {
                    /// Si la petición no es exitosa, mostramos un mensaje de error
                    ModelState.AddModelError(string.Empty, "Error al actualizar el producto");
                }
            }
            return View(producto);

        }

        public async Task<IActionResult> Details(int id)
        {
            /// Realiza una petición GET a la API para obtener el producto con el id especificado
            var response = await _httpClient.GetAsync($"/api/Productos/ver?id={id}");
            /// Si la petición es exitosa
            if (response.IsSuccessStatusCode)
            {
                /// Guarda el objeto producto en la variable content
                var content = await response.Content.ReadAsStringAsync();
                /// Deserializa el contenido a un objeto producto
                var producto = JsonConvert.DeserializeObject<ProductoViewModel>(content);
                /// Retorna la vista con los datos del producto
                return View(producto);
            }
            else
            {
                /// Si la petición no es exitosa, redirige al usuario a la lista de productos
                return RedirectToAction("Details");
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            /// Realiza una petición DELETE a la API para eliminar el producto con el id especificado
            var response = await _httpClient.DeleteAsync($"/api/Productos/eliminar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el producto";
                return RedirectToAction("Index");
            }
        }
    }
}
