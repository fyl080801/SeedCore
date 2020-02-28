using Microsoft.EntityFrameworkCore.Design;

namespace SeedCore.Data.Migrations
{
    public class OperationReportHandler : IOperationReportHandler
    {
        public int Version => 0;

        public void OnError(string message)
        {

        }

        public void OnInformation(string message)
        {

        }

        public void OnVerbose(string message)
        {

        }

        public void OnWarning(string message)
        {

        }
    }
}