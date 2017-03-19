using System.Collections.Generic;

namespace GameStats.Server.Reports
{
    internal class ReportsManager : IReportsManager
    {
        private readonly IDictionary<string, IBaseReport> _reports = new Dictionary<string, IBaseReport>();

        public void Register(string name, IBaseReport report)
        {
            _reports.Add(name, report);
        }

        public string Get(string name, int? count)
        {
            return _reports[name].Get(count);
        }

        public bool Contains(string name)
        {
            return _reports.ContainsKey(name);
        }
    }
}