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
            return XDocument.Load("./DirtyWords.xml");
        }
        public bool IsDirtyWord(string word){
            var dirties = this.GetXML().Root.Descendants("Word").Select(x=> x.Value).Where(y => !string.IsNullOrEmpty(y));
            return dirties.Any(x=> x == word);
        }
        public string GetSafeWord(string input) {
            var is_dirty = this.IsDirtyWord(input);
            var safe_word = input;
            Enumerable.Range(0,input.Length).ToList().ForEach(x => safe_word.Replace(safe_word[x],'*'));
            return safe_word;
        }
    }
}
