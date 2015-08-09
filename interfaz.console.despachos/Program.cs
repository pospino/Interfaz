using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace interfaz.console.despachos
{
  class Program
  {
    public static void Log(string logMessage, TextWriter w)
    {
      w.Write("\r\nAccion Ejecutada: ");
      w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
      w.WriteLine("  :{0}", logMessage);
      w.WriteLine("-------------------------------");
    }
    static void Main(string[] args)
    {
      string Fechastr = DateTime.Now.ToString("yyyyMMdd")+ ".txt";
      string logfile = string.Concat("log", Fechastr );
      using (System.IO.StreamWriter w = File.AppendText(logfile))
      {
        Log("Inicio", w);
        Database database = new Database();
        string despacho_file = "despachos" + Fechastr;
        Program.Log("Buscando Archivo " + despacho_file, w);
        DownloadFile(despacho_file, w);
        w.Flush();
        w.Close();
        w.Dispose();
      }
    }
    static void DownloadFile(string Archivo, StreamWriter w)
    {
      NameValueCollection conf = ConfigurationManager.AppSettings;
      
      FtpWebRequest request = (FtpWebRequest)WebRequest.Create(conf["FTP"]+Archivo);
      request.Method = WebRequestMethods.Ftp.DownloadFile;

      // This example assumes the FTP site uses anonymous logon.
      request.Credentials = new NetworkCredential(conf["FTPUser"], conf["FTPPass"]);

      FtpWebResponse response = (FtpWebResponse)request.GetResponse();

      Stream responseStream = response.GetResponseStream();
      StreamReader reader = new StreamReader(responseStream);
      using (StreamWriter wr = new StreamWriter(Archivo))
      {
        while (!reader.EndOfStream)
          wr.WriteLine(reader.ReadLine());
        wr.Flush();
        wr.Close();
        wr.Dispose();
      }
      Log("Download Complete, status " + response.StatusDescription, w);

      response.Close();
    }
  }
}
