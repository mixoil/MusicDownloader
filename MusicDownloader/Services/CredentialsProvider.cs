using MusicDownloader.Models;
using System.IO;
using System.Xml;
using System;
using System.Linq;
using System.Globalization;
using System.Reflection;

namespace MusicDownloader.Services
{
    /// <inheritdoc cref="ICredentialsProvider"/>
    public sealed class CredentialsProvider : ICredentialsProvider
    {
        private const string CredentialsFileName = "downloaderCredentials.xml";

        /// <inheritdoc/>
        public Credentials GetCredentials()
        {
            //TODO: stop app if some creds are not filled
            var doc = GetDocFromFile(CredentialsFileName);

            var nodeList = doc.GetElementsByTagName("property");

            var credentials = new Credentials();

            FillCredentialsProperties(credentials, nodeList);

            return credentials;
        }

        private XmlDocument GetDocFromFile(string path)
        {
            var doc = new XmlDocument();

            using var readStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            try
            {
                doc.Load(readStream);
            }
            catch (Exception e)
            {
                //TODO: add logger, log every exception.
            }

            return doc;
        }

        private void FillCredentialsProperties(Credentials credentials, XmlNodeList nodeList)
        {
            var fieldsDictionary = typeof(Credentials)
               .GetFields()
               .Where(field => !string.IsNullOrEmpty(field.GetCustomAttribute<CredentialsPropertyAttribute>()?.Name))
               .ToDictionary(field => field.GetCustomAttribute<CredentialsPropertyAttribute>()?.Name);

            for (var cc = 0; cc < nodeList.Count; cc++)
            {
                var node = nodeList[cc];

                var fieldName = node.Attributes["name"].InnerText;

                if (fieldsDictionary.TryGetValue(fieldName, out var field))
                {
                    SetValue(fieldName, node, field, credentials);
                }
            }
        }

        private void SetValue(string fieldName, XmlNode node, FieldInfo f, Credentials credentials)
        {

            object inner;
            
            var type = GetFieldTypeByFieldAndName(f, fieldName);

            if (type == typeof(XmlNodeList))
            {
                inner = node.ChildNodes;
            }
            else if (type == typeof(string))
            {
                inner = node.InnerText;
            }
            else if (type == typeof(byte))
            {
                inner = byte.Parse(node.InnerText);
            }
            else if (type == typeof(bool))
            {
                inner = bool.Parse(node.InnerText);
            }
            else if (type == typeof(DateTime))
            {
                try
                {
                    var formats = new[] { "dd.MM.yyyy H:mm:ss", "MM.dd.yyyy H:mm:ss" };
                    var datefrom = DateTime.ParseExact(node.InnerText, formats, new CultureInfo("ru-RU"), DateTimeStyles.None);
                    inner = datefrom;
                }
                catch (Exception)
                {
                    inner = DateTime.Parse(node.InnerText); //, new System.Globalization.CultureInfo("en-us"));
                }
            }
            else if (type == typeof(float))
            {
                inner = float.Parse(node.InnerText); //, new System.Globalization.CultureInfo("en-us"));
            }
            else if (type == typeof(decimal))
            {
                inner = decimal.Parse(node.InnerText); //, new System.Globalization.CultureInfo("en-us"));
            }
            else
            {
                inner = new object();
            }

            f.SetValue(credentials, inner);
        }

        protected Type GetFieldTypeByFieldAndName(FieldInfo fInfo, string fName)
        {
            try
            {
                foreach (Attribute fAttrib in fInfo.GetCustomAttributes(false))
                {
                    if (fAttrib is CredentialsPropertyAttribute fat)
                    {
                        if (fat.Name == fName)
                        {
                            return fat.PropType;
                        }
                    }
                }

            }
            catch (Exception e)
            {

            }

            return null;
        }
    }
}
