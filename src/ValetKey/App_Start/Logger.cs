namespace ValetKey.Web
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public class Logger
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public Logger(Func<IDictionary<string, object>, Task> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException("next");
            }
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            string method = GetValueFromEnvironment(environment, OwinConstants.RequestMethod);
            string path = GetValueFromEnvironment(environment, OwinConstants.RequestPath);

            string requestBody;
            Stream stream = (Stream)environment[OwinConstants.RequestBody];
            using (StreamReader sr = new StreamReader(stream))
            {
                requestBody = sr.ReadToEnd();
            }
            environment[OwinConstants.RequestBody] = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            Console.WriteLine("Entry\t{0}\t{1}\t{2}", method, path, requestBody);

            Stopwatch stopWatch = Stopwatch.StartNew();
            return _next(environment).ContinueWith(t =>
            {
                Console.WriteLine("Exit\t{0}\t{1}\t{2}\t{3}\t{4}", method, path, stopWatch.ElapsedMilliseconds,
                    GetValueFromEnvironment(environment, OwinConstants.ResponseStatusCode),
                    GetValueFromEnvironment(environment, OwinConstants.ResponseReasonPhrase));
                return t;
            });
        }

        private static string GetValueFromEnvironment(IDictionary<string, object> environment, string key)
        {
            object value;
            environment.TryGetValue(key, out value);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }

    internal static class OwinConstants
    {
        public const string Version = "owin.Version";

        public const string RequestBody = "owin.RequestBody";
        public const string RequestMethod = "owin.RequestMethod";
        public const string RequestPath = "owin.RequestPath";

        public const string ResponseStatusCode = "owin.ResponseStatusCode";
        public const string ResponseReasonPhrase = "owin.ResponseReasonPhrase";
    }
}