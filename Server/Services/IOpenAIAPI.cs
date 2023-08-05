using AIChefRemsey.Shared;

namespace AIChefRemsey.Server.Services
{
    public interface IOpenAIAPI
    {
        Task<List<Idee>> MaakReceptenIdee(string maaltijdmoment, List<string> ingredientenLijst);

        Task<Recept?> MaakRecept(string title, List<string> ingredientenLijst);

        Task<ReceptAfbeelding?> MaakReceptAfbeelding(string receptTitle);
        
    }
}