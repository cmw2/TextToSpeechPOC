using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace TextToSpeechPOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        [HttpGet("{fileName}")]
        public IActionResult GetAudioFile(string fileName)
        {
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "audio/wav", fileName);
        }
    }
}