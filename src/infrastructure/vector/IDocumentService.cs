using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.vector
{
    public interface IDocumentService
    {
        public  Task SaveAsync();
    }
}
