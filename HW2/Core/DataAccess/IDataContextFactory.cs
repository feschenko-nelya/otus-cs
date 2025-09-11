
using LinqToDB.Data;

namespace HW2.Core.DataAccess
{
    public interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
    {
        TDataContext CreateDataContext();
    }
}
