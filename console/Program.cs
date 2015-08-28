using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace interfaz.console
{
	internal class Program
	{
		public Program()
		{
		}

		public static void ConvertirAArchivo<T>(List<T> Objetos, TextWriter w)
		{
      if (Objetos.Count > 0)
      {
        string item = "";

        PropertyInfo[] properties = Objetos[0].GetType().GetProperties();
        foreach (T objeto in Objetos)
        {
          object obj = objeto;
          StringBuilder stringBuilder = new StringBuilder();
          PropertyInfo[] propertyInfoArray = properties;
          for (int i = 0; i < (int)propertyInfoArray.Length; i++)
          {
            PropertyInfo propertyInfo = propertyInfoArray[i];
            if (propertyInfo.Name != "flete")
            {
              stringBuilder.Append(item);
              if (propertyInfo.PropertyType == typeof(DateTime))
              {
                DateTime value = (DateTime)propertyInfo.GetValue(obj, new object[0]);
                stringBuilder.Append(value.ToString("yyyy-MM-dd"));
              }
              else if (!(propertyInfo.PropertyType == typeof(double)))
              {
                stringBuilder.Append(propertyInfo.GetValue(obj, new object[0]));
              }
              else
              {
                stringBuilder.Append(propertyInfo.GetValue(obj, new object[0]).ToString().Replace(',', '.'));
              }
              item = ConfigurationManager.AppSettings["separador"];
            }
          }
          item = "";
          w.WriteLine(stringBuilder.ToString());
        }
      }
		}

		public static void GenerarArchivo(string logMessage, TextWriter w)
		{
			w.Write("{0}", logMessage);
		}

		public static void Log(string logMessage, TextWriter w)
		{
			w.Write("\r\nAccion Ejecutada: ");
			w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
			w.WriteLine("  :{0}", logMessage);
			w.WriteLine("-------------------------------");
		}

		private static void Main(string[] args)
		{
			int count;
			StreamWriter streamWriter;
			DateTime now = DateTime.Now;
			string str = string.Concat(now.ToString("yyyyMMdd"), ".txt");
			StreamWriter streamWriter1 = File.AppendText(
        string.Concat(ConfigurationManager.AppSettings["Carpeta"],"log", str));
			try
			{
				try
				{
					Program.Log("Inicio", streamWriter1);
					Database database = new Database();
					try
					{
						Program.Log("Inicio de la consulta de pagos.", streamWriter1);
						List<Pagos> pagos = database.getPagos();
						count = pagos.Count;
						Program.Log(string.Concat("Se ejecuto la consulta de pagos, se obtuvieron ", count, " registros."), streamWriter1);
						//if (count != 0)
						{
							streamWriter = File.AppendText(string.Concat(ConfigurationManager.AppSettings["Carpeta"],"pagos", str));
							try
							{
								Program.ConvertirAArchivo<Pagos>(pagos, streamWriter);
								Program.Log(string.Concat("Archivo creado: pagos", str), streamWriter1);
								streamWriter.Flush();
								streamWriter.Close();
								streamWriter.Dispose();
								Program.SendFTP(string.Concat( "pagos", str), streamWriter1);
							}
							finally
							{
								if (streamWriter != null)
								{
									((IDisposable)streamWriter).Dispose();
								}
							}
						}
					}
					finally
					{
						if (database != null)
						{
							((IDisposable)database).Dispose();
						}
					}
					database = new Database();
					try
					{
						Program.Log("Inicio de la consulta de Pedidos.", streamWriter1);
						List<Pedidos> pedidos = database.getPedidos();
						count = pedidos.Count;
						Program.Log(string.Concat("Se ejecuto la consulta de Pedidos, se obtuvieron ", count, " registros."), streamWriter1);
						//if (count != 0)
						{
							streamWriter = File.AppendText(string.Concat(ConfigurationManager.AppSettings["Carpeta"],"pedidos", str));
							try
							{
								Program.ConvertirAArchivo<Pedidos>(pedidos, streamWriter);
								Program.Log(string.Concat("Archivo creado: pedidos", str), streamWriter1);
								streamWriter.Flush();
								streamWriter.Close();
								streamWriter.Dispose();
							}
							finally
							{
								if (streamWriter != null)
								{
									((IDisposable)streamWriter).Dispose();
								}
							}
							Program.SendFTP(string.Concat( "pedidos", str), streamWriter1);
						}
					}
					finally
					{
						if (database != null)
						{
							((IDisposable)database).Dispose();
						}
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					Program.Log(string.Concat("Error: ", exception.Message), streamWriter1);
				}
			}
			finally
			{
				if (streamWriter1 != null)
				{
					((IDisposable)streamWriter1).Dispose();
				}
			}
		}

		public static void SendFTP(string pFile, StreamWriter w)
		{
			string item = ConfigurationManager.AppSettings["FTP"];
			string str = ConfigurationManager.AppSettings["FTPUser"];
			string item1 = ConfigurationManager.AppSettings["FTPPass"];
			try
			{
				Path.GetFileName(pFile);
				string str1 = item;
				FtpWebRequest networkCredential = (FtpWebRequest)WebRequest.Create(string.Concat(str1, pFile));
				networkCredential.Credentials = new NetworkCredential(str, item1);
				networkCredential.KeepAlive = true;
				networkCredential.UseBinary = true;
				networkCredential.Method = "STOR";
				FileStream fileStream = File.OpenRead(ConfigurationManager.AppSettings["Carpeta"]+ pFile);
				byte[] numArray = new byte[fileStream.Length];
				fileStream.Read(numArray, 0, (int)numArray.Length);
				fileStream.Close();
				Stream requestStream = networkCredential.GetRequestStream();
				requestStream.Write(numArray, 0, (int)numArray.Length);
				requestStream.Close();
				FtpWebResponse response = (FtpWebResponse)networkCredential.GetResponse();
				Program.Log(string.Concat("Archivo subido al servidor, estado: ", response.StatusDescription), w);
			}
			catch (Exception exception)
			{
				throw exception;
			}
		}
	}
}