using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using periode.Data.DbContexts;
using periode.Models;

namespace periode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeriodeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        public PeriodeController(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostEvaluation([FromBody] Evaluation evaluation)
        {
            var errorMessages = new List<string>();

            // Dictionnaire des validations
            var validationRules = new Dictionary<Func<Evaluation, bool>, string>
            {
                { eval => eval.eval_annee < 2000 || eval.eval_annee > 2100, "L'année d'évaluation doit être entre 2000 et 2100." }
            };

            foreach (var rule in validationRules)
            {
                if (rule.Key(evaluation))
                {
                    errorMessages.Add(rule.Value);
                }
            }

            if (errorMessages.Count > 0)
            {
                return BadRequest(new 
                { 
                    Success = false, 
                    Errors = errorMessages 
                });
            }

            _dbContext.evaluation.Add(evaluation);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Évaluation ajoutée avec succès." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evaluation>>> GetEvaluation()
        {
            var evaluations = await _dbContext.evaluation.Include(e => e.etat).ToListAsync();
            return Ok(evaluations);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvaluation(int id, [FromBody] Evaluation evaluation)
        {
            if (id != evaluation.eval_id)
            {
                return BadRequest("L'ID de l'évaluation ne correspond pas.");
            }

            // Vérifier si l'évaluation existe
            var existingEvaluation = await _dbContext.evaluation.FindAsync(id);
            if (existingEvaluation == null)
            {
                return NotFound("Évaluation non trouvée.");
            }
            existingEvaluation.eval_annee = evaluation.eval_annee;
            existingEvaluation.fixation_objectif = evaluation.fixation_objectif;
            existingEvaluation.mi_parcours = evaluation.mi_parcours;
            existingEvaluation.final = evaluation.final;
            existingEvaluation.etat_id = evaluation.etat_id;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationExists(id))
                {
                    return NotFound("L'évaluation n'existe plus.");
                }
                else
                {
                    throw; // Renvoyer l'exception si une erreur inattendue survient
                }
            }

            return NoContent(); // Renvoie un statut 204 NoContent après la mise à jour
        }

        private bool EvaluationExists(int id)
        {
            return _dbContext.evaluation.Any(e => e.eval_id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluation(int id)
        {
            var evaluation = await _dbContext.evaluation.FindAsync(id);
            if (evaluation == null)
            {
                return NotFound("Évaluation non trouvée.");
            }

            // Supprimer l'évaluation
            _dbContext.evaluation.Remove(evaluation);
            await _dbContext.SaveChangesAsync();

            // Renvoyer un statut 204 NoContent après la suppression
            return NoContent();
        }

        [HttpPut("{id}/etat")]
        public async Task<IActionResult> UpdateEtat(int id, [FromBody] int etat_id)
        {
            // Vérifier si l'évaluation existe
            var evaluation = await _dbContext.evaluation.FindAsync(id);
            if (evaluation == null)
            {
                return NotFound("Évaluation non trouvée.");
            }

            // Mettre à jour uniquement l'état
            evaluation.etat_id = etat_id;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationExists(id))
                {
                    return NotFound("L'évaluation n'existe plus.");
                }
                else
                {
                    throw; // Renvoyer l'exception si une erreur inattendue survient
                }
            }

            return Ok(new { Success = true, Message = "État mis à jour avec succès." });
        }
    }
}