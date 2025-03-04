using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot
{
    public class AibotItemReferenceViewModel
    {
        public string? Name { get; set; }
        public Type? Type { get; set; }

        public ActionType ActionType { get; set; }

        public ActionViewType ActionViewType { get; set; }
    }
}
