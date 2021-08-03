using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ClientTask
{
    public class Form
    {
        public readonly string Name;
        public readonly List<FormElement> Elements;
        public readonly string PostMessage;

        public Form(string json)
        {
            Elements = new List<FormElement>();
            var jObject = JObject.Parse(json);
            var jForm = jObject["form"];
            Name = (string) jForm["name"];
            PostMessage = (string) jForm["postmessage"];
            var items = jForm["items"].Children<JObject>();
            foreach (var item in items)
            {
                Elements.Add(new FormElement(item));
            }
        }
    }
}