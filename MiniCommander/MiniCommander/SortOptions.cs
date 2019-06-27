using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommander
{
    class SortOptions
    {
        private bool ascendingSortName = true;

        private bool ascendingSortDate = false;

        public bool AscendingSortDate
        {
            get { return ascendingSortDate; }
            set { ascendingSortDate = value; }
        }


        public bool AscendingSortName
        {
            get { return ascendingSortName; }
            set
            {
                ascendingSortName = value;                
            }
        }

    }
}
