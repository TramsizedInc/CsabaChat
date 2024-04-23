using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneMessenger.Core
{
    public interface IDirtyWord
    {
        XDocument GetXML();
        bool IsDirtyWord(string word);
        string GetSafeWord(string input);
    }
}
