using System.Collections.Generic;
using System.Linq;
using MegaZord.Library.Interfaces;

namespace MegaZord.Library.Command
{ 
    public class MZCommandResults : IMZCommandResults
    {
        private readonly List<IMZCommandResult> _results = new List<IMZCommandResult>();

        public void AddResult(IMZCommandResult result)
        {
            _results.Add(result);
        }

        public IMZCommandResult[] Results
        {
            get
            {
                return _results.ToArray();
            }
        }

        public bool Success
        {
            get
            {
                return _results.All<IMZCommandResult>(result => result.Success);
            }
        }
    }
}

