using App.Contracts.BusinessContracts;
using App.Contracts.Core;
using App.Data.EFCore;
using App.Data.EFCore.ConceptualModels;
using App.Data.LogicalModelsDTO;
using Buddy.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


namespace App.Business.BusinessObjects
{
    public class MQMessageRepository: Repository<DemoMQMessage>, IMQMessageRepository<DemoMQMessageModel>
    {
        readonly Helper helper = Helper.CreateInstance();
        DbContext _context;

        public MQMessageRepository(DbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<DemoMQMessageModel> GetByCriteria(DemoMQMessageModel entityModel)
        {
            List<DemoMQMessage> messages;
            if (entityModel.Id > 0)
                messages = Find(x => x.Id == entityModel.Id).ToList();
            else
                messages = Find(x => (string.IsNullOrEmpty(entityModel.MSBatchID) || x.MSBatchID == entityModel.MSBatchID)
                && x.IsActive == entityModel.IsActive).ToList();

            return helper.MapObjects<DemoMQMessage, DemoMQMessageModel>(messages);
        }

        public IEnumerable<DemoMQMessageModel> FetchNewMessages(int count)
        {
            List<DemoMQMessage> messages = Find(x => !string.IsNullOrEmpty(x.MSBatchID) && x.IsActive).Take(count).ToList();
            return helper.MapObjects<DemoMQMessage, DemoMQMessageModel>(messages);
        }
    }
}
