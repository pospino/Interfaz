using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentData;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;

namespace interfaz.console.despachos
{
 public class DBLib
  {
    static NameValueCollection conf = ConfigurationManager.AppSettings;

    public static int getIdOrder(string idCart)
    {
      string sql = "SELECT id_order FROM ps_orders WHERE id_cart = @cart";
      int id_order = Database.Context.Sql(sql, new object[0])
        .Parameter("cart",idCart)
        .QuerySingle<int>();
      return id_order;
    }
    public static void setStatus(string id_order, Estados id_status)
    {
      Uri address = new Uri("https://test.diabetrics.tienda/ChangeStatus.php");

      HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      // Create the data we want to send  
      string appId = conf["appId"];
      
      StringBuilder data = new StringBuilder();
      data.Append("appid=" + HttpUtility.UrlEncode(appId));
      data.Append("id_order=" + HttpUtility.UrlEncode(id_order));
      data.Append("id_order_state=" + HttpUtility.UrlEncode(((int)id_status).ToString()));

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
        Console.WriteLine(reader.ReadToEnd());
      }
      using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
      {
        // Get the response stream  
        StreamReader reader = new StreamReader(response.GetResponseStream());

        // Console application output  
        Console.WriteLine(reader.ReadToEnd());
      }

    }
  }
}
