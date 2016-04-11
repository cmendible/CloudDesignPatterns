namespace ValetKey
{
    using Microsoft.Owin.Hosting;
    using System;
    using Web;

    public class Program
    {
        public static void Main(string[] args)
        {
            string address = "http://localhost:5000/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: address))
            {
                Console.WriteLine(string.Format("WebApp is listeninng on: {0}", address));
                Console.WriteLine();
                Console.WriteLine("Client will ask WebApp for a SAS (Valet Key) and upload 10MB blob to an Azure storage container");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

                ValetKeyClient.UploadBlob();
            }
        }
    }
}
