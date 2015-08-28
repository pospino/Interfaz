using System;

using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace interfaz.console
{
	public class Pagos
	{
        public DateTime fdoc
        {
            get;
            set;
        }
        public string claseDoc
        {
            get
            {
                return "EP";
            }
        }
        public string sociedad
        {
            get
            {
                return "CO06";
            }
        }
        public DateTime fcon
        {
            get;
            set;
        }
        public int periodo
        {
            get { return fcon.Month; }
            
        }
        public string moneda
        {
            get
            {
                return "COP";
            }
        }

        public string oc
        {
            get;
            set;
        }

        public string claveContabilizacion1
        {
            get
            {
                return "40";
            }
        }
        public string cuenta
        {
            get
            {
                return "1105050101";
            }
        }
        public double neto
        {
            get;
            set;
        }
        public DateTime fvalor
        {
            get;
            set;
        }

        public string claveContabilizacion2
        {
            get
            {
                return "11";
            }
        }
        public string cliente
        {
            get
            {
                return ConfigurationManager.AppSettings["clientepago"];
            }
        }
        public string claseCuenta
        {
            get
            {
                return "D";
            }
        }
        public string partidasNormales
        {
            get
            {
                return "X";
            }
        }
        public string nombre
        {
            get;
            set;
        }
        public string cedula
        {
            get;
            set;
        }
        public string xref2
        {
            get
            {
                return "";
            }
        }
        public string Año
		{
			get
			{
				return this.fcon.ToString("yyyy");
			}
		}




		


		

		









		


		

		public Pagos()
		{
		}
	}
}