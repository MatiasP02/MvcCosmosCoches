using Microsoft.AspNetCore.Mvc;
using MvcCosmosCoches.Models;
using MvcCosmosCoches.Services;

namespace MvcCosmosCoches.Controllers
{
    public class CosmosController : Controller
    {
        ServiceCosmosDb service;
        public CosmosController(ServiceCosmosDb service)
        {
            this.service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(String accion)
        {
            await this.service.CrearBbddCocheAsync();
            await this.service.CrearColeccionCochesAsync();
            List<Coche> coches = this.service.CrearCoches();
            foreach (Coche v in coches)
            {
                await this.service.InsertarCoche(v);
            }
            ViewBag.mensaje = "CREADO";
            return View();
        }
        public IActionResult ListCoches()
        {
            return View(this.service.GetCoches());
        }
        public async Task<IActionResult> Detalles(String id)
        {
            return View(await this.service.FindCocheAsyn(id));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Coche coche, string motorCheckbox)
        {
            if (motorCheckbox == null)
            {
                coche.Motor = null;
            }
            await this.service.InsertarCoche(coche);
            return RedirectToAction("ListCoches");
        }
        public async Task<IActionResult> Delete(String id)
        {
            await this.service.EliminarCoche(id);
            return RedirectToAction("ListCoches");
        }
        public async Task<IActionResult> Editar(String id)
        {
            return View(await this.service.FindCocheAsyn(id));
        }
        [HttpPost]
        public async Task<IActionResult> Editar(Coche coche)
        {
            await this.service.ModificarCoche(coche);
            return RedirectToAction("ListCoches");
        }

    }
}
