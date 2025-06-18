using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.AI
{
    public interface ISignLanguageProcessor
    {
        Task<string> ProcessFrameAsync(string base64Frame);
    }
}
