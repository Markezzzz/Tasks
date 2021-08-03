using System;
using System.Collections.Generic;
using System.Text;

namespace ClientTask
{
    public class FormGenerator
    {
        private readonly Dictionary<string, Func<FormElement, StringBuilder>> _elements;

        public FormGenerator()
        {
            _elements = new Dictionary<string, Func<FormElement, StringBuilder>>
            {
                {"filler", GenerateFiller},
                {"text", GenerateText},
                {"textarea", GenerateTextArea},
                {"checkbox", GenerateCheckbox},
                {"button", GenerateButton},
                {"select", GenerateSelect},
                {"radio", GenerateRadio}
            };
        }

        public string Generate(string json)
        {
            var form = new Form(json);
            var result = new StringBuilder();
            result.Append($"<form name=\"{form.Name}\">");

            foreach (var element in form.Elements)
            {
                result.Append(GenerateElement(element));
            }

            result.Append("\n</form>");
            return result.ToString();
        }

        private StringBuilder GenerateElement(FormElement element)
        {
            var generationMethod = _elements[(string) element.Attributes["type"]];
            var sb = new StringBuilder();
            sb.Append(generationMethod(element));
            return sb;
        }

        private static StringBuilder GenerateFiller(FormElement element)
        {
            var sb = new StringBuilder();
            sb.Append("\n<div");
            if (element.Attributes.ContainsKey("class"))
                sb.Append($" class=\"{element.Attributes["class"]}\"");
            sb.Append(">\n");
            sb.Append(element.Attributes["message"]);
            sb.Append("\n</div>");
            return sb;
        }

        private static StringBuilder GenerateText(FormElement element)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateLabel(element));
            sb.Append("\n<input");

            foreach (var (attribute, value) in element.Attributes)
            {
                switch (attribute)
                {
                    case "type" when element.Attributes.ContainsKey("validationRules"):
                        break;
                    case "label":
                        break;
                    case "disabled" or "required":
                        if ((string) value == "true")
                            sb.Append($" {attribute}");
                        break;
                    case "validationRules":
                        sb.Append($" type=\"{value["type"]}\"");
                        break;
                    default:
                        sb.Append($" {attribute}=\"{value}\"");
                        break;
                }
            }

            sb.Append("></input>");

            return sb;
        }

        private static string GenerateLabel(FormElement element)
        {
            var result = "";
            if (element.Attributes.ContainsKey("label"))
                result = $"\n<label>{element.Attributes["label"]}</label>";
            return result;
        }

        private static StringBuilder GenerateCheckbox(FormElement element)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateLabel(element));
            sb.Append("\n<input");

            foreach (var (attribute, value) in element.Attributes)
            {
                switch (attribute)
                {
                    case "type" when element.Attributes.ContainsKey("validationRules"):
                        continue;
                    case "label":
                        continue;
                    case "disabled" or "checked" or "required":
                        if ((string) value == "true")
                            sb.Append($" {attribute}");
                        break;
                    case "validationRules":
                        sb.Append($" type=\"{value["type"]}\"");
                        break;
                    default:
                        sb.Append($" {attribute}=\"{value}\"");
                        break;
                }
            }

            sb.Append("></input>");

            return sb;
        }

        private static StringBuilder GenerateRadio(FormElement element)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateLabel(element));

            var sb2 = new StringBuilder();
            foreach (var (attribute, value) in element.Attributes)
            {
                switch (attribute)
                {
                    case "type" when element.Attributes.ContainsKey("validationRules"):
                        continue;
                    case "label" or "items":
                        continue;
                    case "disabled" or "required":
                        if ((string) value == "true")
                            sb2.Append($" {attribute}");
                        break;
                    case "validationRules":
                        sb2.Append($" type=\"{value["type"]}\"");
                        break;
                    default:
                        sb2.Append($" {attribute}=\"{value}\"");
                        break;
                }
            }

            var radioAttributes = sb2.ToString();
            foreach (var item in element.Attributes["items"])
            {
                sb.Append($"\n<input {radioAttributes} value=\"{item["value"]}\""
                          + $" {((string) item["checked"] == "true" ? "checked" : "")}\"></input>");
                sb.Append($"\n<label>{item["label"]}</label>");
            }

            return sb;
        }

        private static StringBuilder GenerateTextArea(FormElement element)
        {
            var sb = new StringBuilder();
            sb.Append(GenerateLabel(element));
            sb.Append("\n<textarea");

            foreach (var (attribute, value) in element.Attributes)
            {
                switch (attribute)
                {
                    case "type" when element.Attributes.ContainsKey("validationRules"):
                        continue;
                    case "label":
                        continue;
                    case "disabled" or "required":
                        if ((string) value == "true")
                            sb.Append($" {attribute}");
                        break;
                    case "validationRules":
                        sb.Append($" type=\"{value["type"]}\"");
                        break;
                    default:
                        sb.Append($" {attribute}=\"{value}\"");
                        break;
                }
            }

            sb.Append("></textarea>");
            return sb;
        }

        private static StringBuilder GenerateButton(FormElement element)
        {
            var sb = new StringBuilder();
            sb.Append("\n<button");

            foreach (var (attribute, value) in element.Attributes)
            {
                if (attribute == "type")
                    continue;
                sb.Append($" {attribute}=\"{value}\"");
            }

            sb.Append("></button>");
            return sb;
        }

        private static StringBuilder GenerateSelect(FormElement element)
        {
            var sb = new StringBuilder();

            sb.Append(GenerateLabel(element));
            sb.Append("\n<select");

            foreach (var (attribute, value) in element.Attributes)
            {
                switch (attribute)
                {
                    case "type" when element.Attributes.ContainsKey("validationRules"):
                        continue;
                    case "label":
                        continue;
                    case "disabled" or "required":
                        if ((string) value == "true")
                            sb.Append($" {attribute}");
                        break;
                    case "options":
                        continue;
                    case "validationRules":
                        sb.Append($" type=\"{value["type"]}\"");
                        break;
                    default:
                        sb.Append($" {attribute}=\"{value}\"");
                        break;
                }
            }

            sb.Append(">\n");
            foreach (var option in element.Attributes["options"])
            {
                sb.Append(
                    $"<option value=\"{option["value"]}\" " +
                    $"{((string) option["selected"] == "true" ? "selected" : "")}\">{option["text"]}</option>\n");
            }

            sb.Append("</select>");

            return sb;
        }
    }
}