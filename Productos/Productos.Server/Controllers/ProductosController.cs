using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Productos.Server.Models;

namespace Productos.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosContext _context;

        /// Con esta configuración, tendremos acceso a la base de datos desde todos los métodos del controlador.
        public ProductosController(ProductosContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> CrearProducto(Producto producto)
        {
            /// Agregamos el producto a la base de datos.
            await _context.Productos.AddAsync(producto);
            /// Guardamos los cambios en la base de datos.
            await _context.SaveChangesAsync();
            /// Retornamos un código 200 (OK) para indicar que la operación fue exitosa.
            return Ok();
        }

        [HttpGet]
        [Route("lista")]
        public async Task<ActionResult<IEnumerable<Producto>>> ListaProductos()
        {
            /// Obtenemos la lista de productos de la base de datos.
            var productos = await _context.Productos.ToListAsync();
            /// Retornamos la lista de productos.
            return Ok(productos);
        }

        [HttpGet]
        [Route("ver")]
        public async Task<IActionResult> VerProducto(int id)
        {
            /// Buscamos el producto en la base de datos.
            Producto producto = await _context.Productos.FindAsync(id);
            /// Si el producto no existe, retornamos un código 404 (Not Found).
            if (producto == null)
            {
                return NotFound();
            }
            /// Si el producto existe, lo retornamos.
            return Ok(producto);
        }

        [HttpPut]
        [Route("editar")]
        public async Task<IActionResult> ActualizarProducto(int id, Producto objeto)
        {
            /// Buscamos el producto existente en la base de datos.
            var productoExistente = await _context.Productos.FindAsync(id);
            /// Una vez que lo encuentra, actualiza los valores del producto existente con los valores del objeto.
            productoExistente!.Nombre = objeto.Nombre;
            productoExistente.Descripcion = objeto.Descripcion;
            productoExistente.Precio = objeto.Precio;

            /// Guardamos los cambios en la base de datos.
            await _context.SaveChangesAsync();
            /// Retornamos un código 200 (OK) para indicar que la operación fue exitosa.
            return Ok();
        }

        [HttpDelete]
        [Route("eliminar")]

        public async Task<IActionResult> EliminarProducto(int id)
        {             
            /// Buscamos el producto en la base de datos.
            var productoBorrar = await _context.Productos.FindAsync(id);
            /// Cuando lo encuentra, lo elimina de la base de datos.
            _context.Productos.Remove(productoBorrar);
            /// Guardamos los cambios en la base de datos.
            await _context.SaveChangesAsync();
            /// Retornamos un código 200 (OK) para indicar que la operación fue exitosa.
            return Ok();
        }

    }
}
