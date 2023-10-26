namespace SQLNaturalBlzSrv.Data;

public class AiQueryService
{
  
    public async Task<SuperDataSet> GetSuperDataSet(string naturalQuery)
    {
        SuperDataSet result = new SuperDataSet();
        await result.Get(naturalQuery);
        return result;
    }
}
