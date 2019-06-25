using System;
using System.Collections.Generic;
using Core.Models;

namespace Core.Objects {
    public class PettyCashObject {
        public List<PettyCash> PettyCash { get; set; }

        public PettyCashObject() {
            PettyCash = new List<PettyCash>();
        }
    }
}
