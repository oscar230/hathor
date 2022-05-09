﻿using Hathor.Api.Exceptions;
using Hathor.Api.Models;
using Hathor.Api.Services.TrackRepositoryServices;
using Microsoft.AspNetCore.Mvc;

namespace Hathor.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        private readonly ILogger<TracksController> _logger;
        private readonly List<ITrackRepositoryService> _trackRepositoryServices;

        public TracksController(ILogger<TracksController> logger, SliderTrackRepositoryService sliderTrackRepositoryService)
        {
            _logger = logger;
            _trackRepositoryServices = new List<ITrackRepositoryService>();
            _trackRepositoryServices.Add(sliderTrackRepositoryService);
        }

        [HttpGet("Query")]
        public async Task<ActionResult<List<Track>>> Query([FromQuery] string? query)
        {
            var tracks = new List<Track>();
            try
            {
                foreach (var trackRepositoryService in _trackRepositoryServices)
                {
                    try
                    {
                        var tracksFound = await trackRepositoryService.Query(query);
                        tracks.AddRange(tracksFound);
                        _logger.LogDebug($"Found {tracksFound?.Count()} tracks in {trackRepositoryService.Repository.DisplayName}.");
                    }
                    catch (TrackQueryNotFoundInThisRepositoryException ex)
                    {
                        _logger.LogDebug($"{ex}");
                    }
                }
                if (tracks.Count == 0)
                {
                    throw new TrackQueryNotFoundInAnyRepositoryException(query, _trackRepositoryServices);
                }
                return Ok(tracks);
            }
            catch (TrackQueryNotFoundInAnyRepositoryException ex)
            {
                _logger.LogDebug($"{ex}");
                return NotFound(ex.UserMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected query exception: {ex}");
                return BadRequest();
            }
        }
    }
}