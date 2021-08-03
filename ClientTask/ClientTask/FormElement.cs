using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ClientTask
{
    public class FormElement
    {
        public readonly Dictionary<string, JToken> Attributes;

        public FormElement(JObject content)
        {
            Attributes = new Dictionary<string, JToken>(content);
        }
    }
}