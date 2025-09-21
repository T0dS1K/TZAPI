using TZAPI.Core;
using Microsoft.AspNetCore.Mvc;

namespace TZAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoreController : ControllerBase
    {
        private ICoreRepository ICoreRepository { get; }

        public CoreController(ICoreRepository ICoreRepository)
        {
            this.ICoreRepository = ICoreRepository;
        }

        [HttpGet("UpdateData")]
        public ActionResult UpdateData()
        {
            if (ICoreRepository.UpdateData(out string em))
            {
                return Ok(new { status = em });
            }
            
            return BadRequest(new { status = em });
        }

        [HttpGet("SearchData")]
        public ActionResult<List<string>> SearchData(string path)
        {
            var Data = ICoreRepository.SearchData(path);

            if (Data.Count != 0)
            {
                return Ok(Data);
            }

            return NotFound();
        }
    }
}
