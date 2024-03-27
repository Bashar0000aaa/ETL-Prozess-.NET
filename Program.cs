/************************************* 
Fertigstellung:   <26.12.2022> 
Bearbeiter(in) 1:  <Madarati, Mhd Bashar, 1386844> 
***************************************/
using System.IO; 
using System;
using System.Net;
using System.Globalization;
class Monat {
public int index { get; set;} 
public List<Zeile> rows = new List<Zeile>(); 

}

class Zeile {
   public DateTime Zeitpunkt { get; set;} 
   public Double GCM { get; set;}
   public Double StromK { get; set;}
   public Double GasK{ get; set;}

}
namespace bwi40322
{
    class ETL
    {
        public static CultureInfo EnglishCulture { get; private set; }

        static void Main(string[] args)
        {
            
            /* -------------------------------------------------- 
               Stage 1: Daten vom Webserver holen 
            -----------------------------------------------------*/
            // Es wird ein WebClient Objekt fuer den Internetzugriff erzeugt
            WebClient client = new WebClient();

            // Die url wir als String gespeichert
            string url = "https://cbrell.de/bwi403/demo/ZaehlerstandExport.csv";

            //Consoleausgebe um den Nutzer auf dem laufenen zu halten
            Console.WriteLine("Die Daten werden vom URL geladen");

           //Die Webseite wird geoefnet und mit Reader werden die Daten eingelesen
            Stream stream = client.OpenRead(url);
            StreamReader reader = new StreamReader(stream);

            // Die Daten werden in String Variable gespeichert 
            string info = reader.ReadToEnd();

            // Alle VErbindungen werden gebrochen bzw. geschlossen 
            stream.Close();
            reader.Close();

            //Consoleausgebe um den Nutzer auf dem laufenen zu halten
            Console.WriteLine("Verbindung zum URL wurde beendet\n und die daten werden in einer CSV.Datei gespeichert");

            // Die String, wo sich die Daten befinden, wird in eine CSV.Datei geschreiben 
            File.WriteAllText("ein.csv", info); 

            //Pruefen, ob dei Datei vorhanden ist und dementsprechend eine Konsoleausgabe 
            if (File.Exists("ein.csv"))
            {
                Console.WriteLine("Die Datei ein.csv wurde erstellt.\n");
            }
            else
            {
                Console.WriteLine("Die Datei ein.csv ist nicht vorhanden.\n");
            }

            /* -------------------------------------------------- 
            Stage 2: Daten von Lokal laden und aufbereiten 
         -----------------------------------------------------*/











      
            // Aus der lokalen Datei werden  Daten wieder eingeladen 
            //Ein Dateistrom wird erzeugt, um aus der zuvor erstellten Datei einlesen zu koennen
            StreamReader reader2 = new StreamReader("ein.csv");
            //Eine neue Datei wird erstellt, in die die zu extrahierenden Daten geschrieben werden koennen


            List<Zeile> rows = new List<Zeile>(); 

            Console.WriteLine("Die Daten aus ein.csv werden in aus.csv geschrieben");
            
            // leere bzw null werte muessen gecatchr werden also man braucht try catch anweisungen
            // wir haben den Dateistrom erzeugt um lesen zu koenen und jetzt speichern wir das eingelesene im string
            while (!reader2.EndOfStream)
            {
                try
                {
                    string ExDaten = reader2.ReadLine(); // die Zeilen werden Einzel gelesen
                    string[] values = ExDaten.Split(';', ' ');  // Die Spalten werden mit ; getrennt  
 
                     Zeile obj = new Zeile {
                        Zeitpunkt =  Convert.ToDateTime((values[0])),
                        GCM = Convert.ToDouble(values[3]),
                        StromK = Convert.ToDouble(values[4]),
                        GasK = Convert.ToDouble(values[5])
                    }; 
                  /*   Console.WriteLine(obj.Zeitpunkt + ";" + obj.GCM + ";" + obj.StromK + ";" + obj.GasK + ";"); */
                    rows.Add(obj);   

                /* if (!zeitpunkt.Contains(zp)) { zeitpunkt.Add(zp); } */

                }
                catch (Exception q) { continue; }
            }

            reader2.Close();
            stream.Close();

            string title = "Zeitpunkt;Gas cbm kumuliert;Strom kwh kumuliert;Gas kwh kumuliert;";
            File.AppendAllText("ausgabe.csv", title + Environment.NewLine);

            foreach (var row in rows) { 
                 string zeile = row.Zeitpunkt.ToString("dd.MM.yyyy") + ";" + row.GCM + ";" + row.StromK + ";" + row.GasK + ";";
                 File.AppendAllText("ausgabe.csv", zeile + Environment.NewLine);
            } 

            double gesamtVerbrauch = rows[rows.Count - 1].GasK - rows[1].GasK;
            double anzahlTage = (rows[rows.Count - 1].Zeitpunkt).Subtract(rows[1].Zeitpunkt).TotalDays;
            double temp =  rows[1].StromK * 100 / (rows[rows.Count - 1].StromK);
             Console.WriteLine(rows[1].StromK * 100 / (rows[rows.Count - 1].StromK));
             Console.WriteLine(temp * 100);


            EnglishCulture = new CultureInfo("en-EN");


        List<Monat> months = new List<Monat>(); 
        // 
        foreach (var row in rows) { 
        int currentMonth = row.Zeitpunkt.Date.Month;

        Boolean found = false;
         foreach (var month in months) { 
                if(month.index == currentMonth) {
                    found = true;
                }
         } 
         if(!found) {
              Monat month = new Monat();  
              month.index = currentMonth;
              month.rows.Add(row);
              months.Add(month);
         }
         else {
            foreach (var month in months) { 
                if(month.index == currentMonth) {
                   month.rows.Add(row);
                }
         } 
         }
 
        }

            string data = "[";
            string labels = "[";

        foreach (var month in months) { 
             double gesamtverbrauch2 = month.rows[month.rows.Count - 1].StromK - month.rows[0].StromK;  
             data+= gesamtverbrauch2.ToString("F2",EnglishCulture)+ ",";
             labels+= FormatMonthIndex(month.index) + ","; 
             Console.WriteLine(data+"dsfsd");                         
        }
         
        data += "]";
        labels += "]";
       string html = "<html><head>";
            html+= "<link rel='stylesheet' href='stylesheet.css'>";
            html+= "</head> <body>"; 
            html+= "<div><canvas id='myChart'></canvas></div> </div></body>";
            html+= "  "; 
            html += "<script src='https://cdn.jsdelivr.net/npm/chart.js'></script>";
            html += "<script>";
            html += " const ctx = document.getElementById('myChart');";
            html += "new Chart(ctx, {";
            html += " type: 'line',";
            html += " data: {";
            html += "labels: " + labels + ",";
            html += "  datasets: [{";
            html += " label: 'Stromverbrauch',";
            html += " data:" +  data + ",";
            html += " borderWidth: 1";
            html += "   }]";
            html += "  },";
            html += "   options: {";
            html += "  scales: {";
            html += "y: {";
            html += "beginAtZero: true";
            html += " }}} }); </script>";  

            Console.WriteLine("Berechnungsergebnis: " + 3.3 + 2);
            html+= "</html>";    
            File.WriteAllText("index.html", html); 
             using (FileStream fs = new FileStream("stylesheet.css", FileMode.Create)) 
            { 
             using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8)) 
            { 
              w.WriteLine("#column-example-11 {height: 200px;max-width: 300px;margin: 0 auto;}"); 
              w.WriteLine(".table{padding : 5px; width: 1000px;height: 500px} "); 
            } 
            }

            
        } 
 public static string FormatMonthIndex(int monthIndex)
{
  string[] months = {"'Januar'", "'Februar'", "'März'", "'April'", "'Mai'", "'Juni'", "'Juli'", "'August'", "'September'", "'Oktober'", "'November'", "'Dezember'"};
  if (monthIndex < 1 || monthIndex > 12)
  {
    return "Ungültiger Monatsindex";
  }
  return months[monthIndex - 1];
}
    }
}





