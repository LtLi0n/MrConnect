using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using WoT.Shared;
using LionLibrary.Framework;

namespace WoT.Server
{
    public class CharacterWorkService : IWoTService
    {
        public ILogService Logger { get; }
        public event EventHandler<CharacterWorkFinishedEventArgs> WorkFinished;

        public CharacterWorkService(ILogService logger)
        {
            Logger = logger;
        }
        
        public async Task UpdateAsync(IServiceProvider services)
        {
            WoTDbContext db = services.GetService<WoTDbContext>();
            IQueryable<CharacterWork> workers = db.CharactersWork.Where(x => x.IsWorking);

            foreach(CharacterWork worker in workers)
            {
                await HandleWorker(db, worker);
            }
        }

        ///<summary>Finish work if it's due to be done.</summary>
        private async Task HandleWorker(WoTDbContext db, CharacterWork worker)
        {
            var args = await worker.UpdateAsync(db, db.Users, db.Characters);

            if(args != null)
            {
                WorkFinished?.Invoke(worker, args);
                Logger?.LogLine(this, args.ToString(), LogSeverity.Verbose);
            }
        }
    }
}
