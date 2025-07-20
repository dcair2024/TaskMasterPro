using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskMasterPro__v3._0_.Data;
using TaskMasterPro__v3._0_.Models;
using Microsoft.AspNetCore.Identity; // Necessário para o Identity
using System.Security.Claims;      // Necessário para pegar o ID do usuário logado
using Microsoft.AspNetCore.Authorization; // <-- Adicionado para proteger o controlador

namespace TaskMasterPro__v3._0_.Controllers
{
    [Authorize] // <-- Adicionado: Agora, só quem está logado pode acessar qualquer coisa aqui!
    public class TarefasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tarefas
        public async Task<IActionResult> Index()
        {
            // Pega o ID do usuário logado (a 'etiqueta de dono' do usuário atual)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Busca no banco de dados SOMENTE as tarefas que têm a 'etiqueta de dono' igual ao usuário logado
            var tarefasDoUsuario = await _context.Tarefas
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return View(tarefasDoUsuario);
        }

        // GET: Tarefas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Garante que o usuário só veja os detalhes da SUA tarefa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarefa = await _context.Tarefas
                .Where(t => t.Id == id && t.UserId == userId) // <-- Adicionado filtro por UserId
                .FirstOrDefaultAsync();

            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: Tarefas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tarefas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                tarefa.UserId = userId;

                _context.Add(tarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tarefa);
        }


        // GET: Tarefas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Garante que o usuário só edite a SUA tarefa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarefa = await _context.Tarefas
                .Where(t => t.Id == id && t.UserId == userId) // <-- Adicionado filtro por UserId
                .FirstOrDefaultAsync();

            if (tarefa == null)
            {
                return NotFound();
            }
            return View(tarefa);
        }

        // POST: Tarefas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarefa tarefa) // <-- REMOVIDO o [Bind("...")] aqui!
        {
            if (id != tarefa.Id)
            {
                return NotFound();
            }

            // Antes de atualizar, precisamos garantir que a tarefa pertence ao usuário logado
            // e que o UserId não seja alterado por um ataque de overposting.
            // O ideal é carregar a tarefa do banco e atualizar apenas os campos permitidos.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarefaToUpdate = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (tarefaToUpdate == null)
            {
                return NotFound(); // Tarefa não encontrada ou não pertence ao usuário logado
            }

            // Atualiza apenas as propriedades que podem ser alteradas pelo usuário
            tarefaToUpdate.Titulo = tarefa.Titulo;
            tarefaToUpdate.Descricao = tarefa.Descricao;
            tarefaToUpdate.Data = tarefa.Data;
            tarefaToUpdate.Concluida = tarefa.Concluida;

            // Não atualize tarefaToUpdate.UserId aqui! Ele deve ser mantido.

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarefaToUpdate); // Atualiza a tarefa carregada do banco
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TarefaExists(tarefa.Id)) // Use tarefa.Id aqui, não tarefaToUpdate.Id
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarefa); // Retorna a view com a tarefa original para mostrar erros de validação
        }

        // GET: Tarefas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Garante que o usuário só possa apagar a SUA tarefa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarefa = await _context.Tarefas
                .Where(t => t.Id == id && t.UserId == userId) // <-- Adicionado filtro por UserId
                .FirstOrDefaultAsync();

            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: Tarefas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Garante que o usuário só apague a SUA tarefa
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId); // <-- Adicionado filtro por UserId

            if (tarefa != null)
            {
                _context.Tarefas.Remove(tarefa);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TarefaExists(int id)
        {
            // Garante que a verificação de existência também considere o usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.Tarefas.Any(e => e.Id == id && e.UserId == userId); // <-- Adicionado filtro por UserId
        }
    }
}

