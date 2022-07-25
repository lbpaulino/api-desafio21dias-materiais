using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using api_desafio21dias.Models;
using api_desafio21dias.Servicos;
using EntityFrameworkPaginateCore;
using System.Net.Http;
using web_renderizacao_server_side.Helpers;
using Microsoft.AspNetCore.Http;

namespace api_desafio21dias.Controllers
{
    [ApiController]
    [Logado]
    public class MateriaisController : ControllerBase
    {
        private readonly DbContexto _context;
        private const int QUANTIDADE_POR_PAGINA = 3;
        public MateriaisController(DbContexto context)
        {
            _context = context;
        }

        // GET: /materiais
        [HttpGet]
        [Route("/materiais")]
        public async Task<IActionResult> Index(int page = 1)
        {
            return StatusCode(200, await _context.Materiais.OrderBy(a => a.Id).PaginateAsync(page, QUANTIDADE_POR_PAGINA));
        }

        // GET: /materiais/5
        [HttpGet]
        [Route("/materiais/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materiais
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return StatusCode(200, material);
        }

        // POST: /materiais
        [HttpPost]
        [Route("/materiais")]
        public async Task<IActionResult> Create(Material material)
        {
            if (ModelState.IsValid)
            {
                var token = base.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                Console.WriteLine("========[" + token + "]=======");
                if(! (await AlunoServico.ValidarUsuario(material.AlunoId, token)) )
                    return StatusCode(400, new { Mensagem = "O usuário passado não é válido ou não está cadastrado" });

                _context.Add(material);
                await _context.SaveChangesAsync();
                return StatusCode(201, material);
            }
            return StatusCode(400, new { Mensagem = "O Material passado é inválido" });
        }

        // PUT: /materiais/5
        [HttpPut]
        [Route("/materiais/{id}")]
        public async Task<IActionResult> Edit(int id, Material material)
        {
            if (ModelState.IsValid)
            {
                var token = base.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if(! (await AlunoServico.ValidarUsuario(material.AlunoId, token)) )
                    return StatusCode(400, new { Mensagem = "O usuário passado não é válido ou não está cadastrado" });

                try
                {
                    material.Id = id;
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return StatusCode(200, material);
            }
            return StatusCode(200, material);
        }

        // DELETE: /materiais/5
        [HttpDelete]
        [Route("/materiais/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materiais.FindAsync(id);
            _context.Materiais.Remove(material);
            await _context.SaveChangesAsync();
            return StatusCode(204);
        }

        private bool MaterialExists(int id)
        {
            return _context.Materiais.Any(e => e.Id == id);
        }
    }
}
