using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OneMessenger.Core;

namespace OneMessenger.Server
{
    internal class DirtyWord: IDirtyWord
    {
        public XDocument GetXML()
        {
            return new XDocument();
        }
        public bool IsDirtyWord() { return false; }
        public string GetSafeWord(string input) {  return ""; }
    }
}
