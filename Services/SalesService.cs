using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.Services
{
    public interface ISalesService
    {
        public List<PettyCash> GetPettyCash(DateTime from, DateTime to, string code);
    }
}
