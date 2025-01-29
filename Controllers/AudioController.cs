using Microsoft.AspNetCore.Mvc;
using TextToSpeechPOC.Data;

namespace TextToSpeechPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private readonly FileService _fileService;

        public AudioController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{token}")]
        public IActionResult GetAudioFile(string token)
        {
            var fileBytes = _fileService.GetFileBytes(token);
            if (fileBytes == null)
            {
                return NotFound();
            }
            return File(fileBytes, "audio/wav", Path.GetFileName(token));
        }
    }
}