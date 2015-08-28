using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace interfaz.console.despachos
{
  class Program
  {
    static NameValueCollection conf = ConfigurationManager.AppSettings;
    public static void setStatus(Despacho ent,StreamWriter w)
    {
      Uri address = new Uri("https://www.diabetrics.tienda/ChangeStatus.php");

      HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      // Create the data we want to send  
      string appId = conf["appId"];

      StringBuilder data = new StringBuilder();
      data.Append("appId=" + HttpUtility.UrlEncode(appId));
      data.Append("&order=" + HttpUtility.UrlEncode(ent.id_cart));
      data.Append("&state=" + HttpUtility.UrlEncode(((int)ent.Estado).ToString()));
      if (!string.IsNullOrEmpty(ent.Guia))
        data.Append("&shipping=" + HttpUtility.UrlEncode(ent.Guia));
      if (!string.IsNullOrEmpty(ent.Factura_SAP))
        data.Append("&invoice=" + HttpUtility.UrlEncode(ent.Factura_SAP));
      



      // Create a byte array of the data we want to send  
      byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

      // Set the content length in the request headers  
      request.ContentLength = byteData.Length;

      // Write data  
      using (Stream postStream = request.GetRequestStream())
      {
        postStream.Write(byteData, 0, byteData.Length);
      }

      // Get response  
      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {
        // Get the response stream  
        StreamReader reader = new StreamReader(response.GetResponseStream());

        // Console application output  
        Log(reader.ReadToEnd(), w);
      }


    }
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
      using (System.IO.StreamWriter w = File.AppendText(conf["Carpeta"]+ logfile))
      {
        #region despacho
        Log("Inicio lectura de archivo de despachos.", w);
        Database database = new Database();
        string despacho_file = "despachos" + Fechastr;
        Program.Log("Buscando Archivo " + despacho_file, w);
        DownloadFile(despacho_file, w);
        DeleteFile(despacho_file, w);
        Log("Leyendo archivo local", w);
        try {
          using (StreamReader reader = new StreamReader(conf["Carpeta"]+despacho_file))
          {
            try {
              Log("Archivo leido", w);
              while (!reader.EndOfStream)
              {
                string linea = reader.ReadLine();
                realizarAcciones(linea, w);
              }
            } catch (Exception e)
            {
              Log("Error: " + e.Message, w);
            }
          }
        }catch(Exception e)
        {
          Log("Error: " + e.Message, w);
        }
        #endregion
        #region despacho
        Log("Inicio lectura de archivo de despachos.", w);
        string error_file = "despachos" + Fechastr;
        Program.Log("Buscando Archivo " + despacho_file, w);
        DownloadFile(despacho_file, w);
        DeleteFile(despacho_file, w);
        Log("Leyendo archivo local", w);
        try
        {
          using (StreamReader reader = new StreamReader(conf["Carpeta"] + despacho_file))
          {
            try
            {
              Log("Archivo leido", w);
              while (!reader.EndOfStream)
              {
                string linea = reader.ReadLine();
                realizarAcciones(linea, w);
              }
            }
            catch (Exception e)
            {
              Log("Error: " + e.Message, w);
            }
          }
        }
        catch (Exception e)
        {
          Log("Error: " + e.Message, w);
        }
        #endregion
        w.Flush();
        w.Close();
        w.Dispose();
      }
    }
    static void realizarAcciones(string linea, StreamWriter w)
    {
      PropertyInfo[] props = typeof(Despacho).GetProperties();
      string[] datos = linea.Split(conf["separador"][0]);
      int cont = 0;
      Despacho ent = new Despacho();
      foreach (PropertyInfo p in props)
      {
        if(p.Name != "Estado")
        p.SetValue(ent,datos[cont],null);
        cont++;
      }
      ent.Estado = Estados.Preparacion;
      if (!string.IsNullOrEmpty(ent.Fecha_Recibido))
      {
        ent.Estado = Estados.Entregado;
      }
      else if (!string.IsNullOrEmpty(ent.Fecha_Envio))
      {
        ent.Estado = Estados.Enviado;
      }
      setStatus(ent,w);
    }
    static void DownloadFile(string Archivo, StreamWriter w)
    {
     
      FtpWebRequest request = (FtpWebRequest)WebRequest.Create(conf["FTP"]+Archivo);
      request.Method = WebRequestMethods.Ftp.DownloadFile;
      FtpWebResponse response=null;
      // This example assumes the FTP site uses anonymous logon.
      request.Credentials = new NetworkCredential(conf["FTPUser"], conf["FTPPass"]);
      try
      {
         response = (FtpWebResponse)request.GetResponse();

        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        using (StreamWriter wr = new StreamWriter(conf["Carpeta"] + Archivo))
        {
          while (!reader.EndOfStream)
            wr.WriteLine(reader.ReadLine());

          wr.Flush();
          wr.Close();
          wr.Dispose();
        }
        Log("Download Complete, status " + response.StatusDescription, w);
      }
      catch (Exception e)
      {
        Log("Error:"+e.Message, w);
      }
      finally
      {
        if (response != null)
          response.Close();
      }
    }
    static void DeleteFile(string Archivo, StreamWriter w)
    {
      try {
        NameValueCollection conf = ConfigurationManager.AppSettings;

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(conf["FTP"] + Archivo);
        request.Method = WebRequestMethods.Ftp.DeleteFile;

        // This example assumes the FTP site uses anonymous logon.
        request.Credentials = new NetworkCredential(conf["FTPUser"], conf["FTPPass"]);

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        Log("Delete Complete, status " + response.StatusDescription, w);

        response.Close();
      }catch(Exception ex)
      {
        Log("Error: " + ex.Message, w);
      }
    }
  }
}
