using API.Application;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Crée une nouvelle réservation pour un foodtruck.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request)
    {
        try
        {
            var response = await _reservationService.CreateAsync(request);

            return CreatedAtAction(nameof(GetActive), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Annule une réservation existante.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var success = await _reservationService.CancelAsync(id);

        if (!success)
        {
            return NotFound(new { message = $"Réservation avec l'ID {id} introuvable ou déjà annulée." });
        }

        return NoContent();
    }

    /// <summary>
    /// Récupère toutes les réservations actives (non annulées).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservationResponse>>> GetActive()
    {
        var reservations = await _reservationService.GetAllActiveAsync();
        return Ok(reservations);
    }

    /// <summary>
    /// Génère le rapport mensuel pour une période donnée.
    /// </summary>
    [HttpGet("report/{year:int}/{month:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonthlyReportResponse>> GetMonthlyReport(int year, int month)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest(new { message = "Mois invalide." });
        }

        var report = await _reservationService.GetMonthlyReportAsync(month, year);
        return Ok(report);
    }
}