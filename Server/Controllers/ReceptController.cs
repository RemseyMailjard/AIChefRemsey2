using AIChefRemsey.Shared;
using AIChefRemsey.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIChefRemsey.Server.Data;
using AIChefRemsey.Server.Services;

namespace AIChefRemsey.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReceptController : ControllerBase
    { 
        private readonly IOpenAIAPI _openAIservice;

        public ReceptController(IOpenAIAPI openAIservice)
        {
            _openAIservice = openAIservice;
        }
    
        [HttpPost, Route("VerkrijgReceptIdee")]
        public async Task<ActionResult<List<Idee>>> VerkrijgReceptIdee(ReceptParameters receptParams)
        {
            string maaltijd = receptParams.Maaltijdmoment;
            List<string> ingrediënten = receptParams.Ingredients
                .Where(x => !string.IsNullOrEmpty(x.Beschrijving))
            .Select(x => x.Beschrijving!)
            .ToList();
           
       
            if(string.IsNullOrEmpty(maaltijd) )
            {
                maaltijd = "Breakfast";

            }
           var ideen = await _openAIservice.MaakReceptenIdee(maaltijd, ingrediënten);
         return ideen;
          //  return SampleData.ReceptIdeeën;
        }

        [HttpPost, Route("VerkrijgRecept")]
        public async Task<ActionResult<Recept?>> VerkrijgRecept(ReceptParameters receptParams)
        {
            List<string> ingredienten = receptParams.Ingredients
                .Where(x => !string.IsNullOrEmpty(x.Beschrijving))
                .Select(x => x.Beschrijving!)
                .ToList();

            string? title = receptParams.GeselecteerdeIdee;
            if(string.IsNullOrEmpty(title) ) 
            {
                return BadRequest();
            }
            var recept = await _openAIservice.MaakRecept(title, ingredienten);
            return recept;
          //  return SampleData.recept;
        }
        [HttpGet, Route("VerkrijgReceptAfbeelding")]
        public async Task<ReceptAfbeelding> VerkrijgReceptAfbeelding(string title)
        {
            var receptafbeelding = await _openAIservice.MaakReceptAfbeelding(title);

            return receptafbeelding ?? SampleData.Receptafbeelding;

            // SampleData.Receptafbeelding;
        }
    }
}
