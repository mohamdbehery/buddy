using App.Contracts.BusinessContracts;
using App.Data.LogicalModelsDTO;

namespace App.Contracts
{
    public interface IUnitOfWork
    {
        IMQMessageRepository<DemoMQMessageModel> MQMessageRepository { get; }
        IAppUserRepository<AppUserModel> AppUserRepository { get; }
        int Save();
    }
}
