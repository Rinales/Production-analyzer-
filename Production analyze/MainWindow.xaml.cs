//using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Windows;
using DataTable = System.Data.DataTable;
using OxyPlot;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Windows.Documents;

namespace Production_analyze
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //Microsoft.Office.Interop.Excel.Application objExcel;
        SqlConnection database;
        String databaseCon = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\VisualStudioTest\\Production analyze\\Production analyze\\Database1.mdf\";Integrated Security=True";
        Chart chart;


        public MainWindow()
        {
            InitializeComponent();

            startDatePicker.SelectedDate = new DateTime(2023, 3, 1);
            endDatePicker.SelectedDate = new DateTime(2023, 3, 31);
            database = new SqlConnection(@databaseCon);
            chart = new Chart(LineChart, BarChart);

            chart.setUpChart(null);
            //chart.setUpPieChart(GetUniquesFromTable().Item1, null);
        }


        

        /* private void Load_Date_Click(object sender, RoutedEventArgs e)
           {

               OpenFileDialog openfileDialog1 = new OpenFileDialog();
               openfileDialog1.ShowDialog();
               String fileName = openfileDialog1.FileName;

               var ExcelApp = new Microsoft.Office.Interop.Excel.Application();
               ExcelApp.Visible = false;

               Workbook wb = ExcelApp.Workbooks.Open(fileName);
               Worksheet excelSheet = wb.ActiveSheet;

               Microsoft.Office.Interop.Excel.Range last = excelSheet.Cells.SpecialCells(Microsoft.Office.Interop.Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
               Microsoft.Office.Interop.Excel.Range range = excelSheet.get_Range("A1", last);

               int maxRows = last.Row;
               int maxColumns = last.Column;


               Thread thread = new Thread(() => ReadExcel(excelSheet, 2, maxRows, maxColumns));
               //Thread thread2 = new Thread(() => ReadExcel(excelSheet, (maxRows / 2) + 1, maxRows, maxColumns));

               thread.Start();
               //thread2.Start();
           }
        */
        private void Load_Date_Click(object sender, RoutedEventArgs e)
        {
            String fileName2 = "C:\\Přehled ztrát dostupnosti-1.xlsx";
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            var package = new ExcelPackage(new FileInfo(fileName2));
            ExcelWorksheet excelSheet = package.Workbook.Worksheets[0];
            int maxRows = excelSheet.Dimension.Rows;
            int maxColumns = excelSheet.Dimension.Columns;
            Thread thread = new Thread(() => ReadExcel(excelSheet, 2, maxRows, maxColumns));
            thread.Start();
        }



        /*   private void ReadExcel(Worksheet excelSheet, int startRow, int maxRows, int MaxColumns)
           {
               int hotovo = 0;
               DateTime start = DateTime.Now;
               Console.WriteLine(start + "  " + startRow);

               DataTable dataTable = new DataTable();
               dataTable.Columns.Add("Stroj", typeof(string));
               dataTable.Columns.Add("Zacatek", typeof(DateTime));
               dataTable.Columns.Add("Konec", typeof(DateTime));
               dataTable.Columns.Add("Duvod", typeof(string));
               dataTable.Columns.Add("Pricina", typeof(string));

               String stroj;
               DateTime zacatek;
               DateTime konec = DateTime.Parse("2000-01-01 00:00:00");
               String duvod;
               String pricina;

               Parallel.For(startRow, maxRows, i =>
               {
                   hotovo++;
                   stroj = excelSheet.Cells[i, 5].Value.ToString() ?? "";
                   zacatek = DateTime.Parse(excelSheet.Cells[i, 1].Value.ToString());

                   try { konec = DateTime.Parse(excelSheet.Cells[i, 2].Value.ToString()); }
                   catch { }

                   duvod = excelSheet.Cells[i, 7].Value.ToString() ?? "";
                   pricina = "";
                   if (duvod != "Není objednávka") { pricina = excelSheet.Cells[i, 6].Value.ToString(); }

                   lock (dataTable)
                   {
                       dataTable.Rows.Add(stroj, zacatek, konec, duvod, pricina);
                   }
                   Dispatcher.Invoke(() =>
                       {
                           PBar2.Maximum = maxRows;
                           PBar2.Value = hotovo;
                       });


               });
               lock (database)
               {
                   database.Open();
                   string query = "INSERT INTO Zaznamy (Stroj, Zacatek, Konec, Duvod, Pricina) SELECT Stroj, Zacatek, Konec, Duvod, Pricina FROM @dataTable";
                   Console.WriteLine(DateTime.Now - start);
                   start = DateTime.Now;
                   using (SqlBulkCopy bulkCopy = new SqlBulkCopy(database))
                   {
                       bulkCopy.DestinationTableName = "Zaznamy";
                       bulkCopy.ColumnMappings.Add("Stroj", "Stroj");
                       bulkCopy.ColumnMappings.Add("Zacatek", "Zacatek");
                       bulkCopy.ColumnMappings.Add("Konec", "Konec");
                       bulkCopy.ColumnMappings.Add("Duvod", "Duvod");
                       bulkCopy.ColumnMappings.Add("Pricina", "Pricina");

                       bulkCopy.BulkCopyTimeout = 60;

                       bulkCopy.WriteToServer(dataTable);
                   }
                   Console.WriteLine(DateTime.Now - start + "konec" + startRow);
                   Console.WriteLine("hotovo " + DateTime.Now);
                   database.Close();
               }
           }*/


        private void ReadExcel(ExcelWorksheet excelSheet, int startRow, int endRow, int MaxColumns)
        {
            int hotovo = 0;
            DateTime start = DateTime.Now;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Stroj", typeof(string));
            dataTable.Columns.Add("Zacatek", typeof(DateTime));
            dataTable.Columns.Add("Konec", typeof(DateTime));
            dataTable.Columns.Add("Duvod", typeof(string));
            dataTable.Columns.Add("Pricina", typeof(string));

            String stroj;
            DateTime zacatek;
            DateTime konec;
            String duvod;
            String pricina;



            for (int i = startRow; i <= endRow; i++)
            {
                hotovo++;


                object strojvalue = excelSheet.Cells[i, 5].Value;
                if (strojvalue != null)
                {
                    stroj = (String)strojvalue;
                }
                else { stroj = ""; }

                object zacatekvalue = excelSheet.Cells[i, 1].Value;
                if (zacatekvalue != null)
                {
                    zacatek = (DateTime)zacatekvalue;
                }
                else { zacatek = DateTime.Parse("2000-01-01 00:00:00"); }

                object konecvalue = excelSheet.Cells[i, 2].Value;
                if (konecvalue != null)
                {
                    konec = (DateTime)konecvalue;
                }
                else { konec = DateTime.Today; }

                object duvodvalue = excelSheet.Cells[i, 6].Value;
                if (duvodvalue != null)
                {
                    duvod = (String)duvodvalue;
                }
                else { duvod = ""; }

                object pricinavalue = excelSheet.Cells[i, 7].Value;
                if (pricinavalue != null)
                {
                    pricina = (String)pricinavalue;
                }
                else { pricina = ""; }
                lock (dataTable)
                {
                    dataTable.Rows.Add(stroj, zacatek, konec, duvod, pricina);
                }
                /*
                                    hotovo++;
                                stroj = excelSheet.Cells[i, 5].Value.ToString() ?? "";
                                zacatek = DateTime.Parse(excelSheet.Cells[i, 1].Value.ToString());

                                try { konec = DateTime.Parse(excelSheet.Cells[i, 2].Value.ToString()); }
                                catch { }

                                duvod = excelSheet.Cells[i, 7].Value.ToString() ?? "";
                                pricina = "";
                                if (duvod != "Není objednávka") { pricina = excelSheet.Cells[i, 6].Value.ToString(); }

                                lock (dataTable)
                                {
                                    dataTable.Rows.Add(stroj, zacatek, konec, duvod, pricina);
                                }*/
                Dispatcher.Invoke(() =>
                {
                    PBar2.Maximum = endRow;
                    PBar2.Value = hotovo;
                });
            }
            lock (database)
            {
                DeleteDataFromTable("Zaznamy");
                database.Open();
                string query = "INSERT INTO Zaznamy (Stroj, Zacatek, Konec, Duvod, Pricina) SELECT Stroj, Zacatek, Konec, Duvod, Pricina FROM @dataTable";
                Console.WriteLine(DateTime.Now - start);
                start = DateTime.Now;
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(database))
                {
                    bulkCopy.DestinationTableName = "Zaznamy";
                    bulkCopy.ColumnMappings.Add("Stroj", "Stroj");
                    bulkCopy.ColumnMappings.Add("Zacatek", "Zacatek");
                    bulkCopy.ColumnMappings.Add("Konec", "Konec");
                    bulkCopy.ColumnMappings.Add("Duvod", "Duvod");
                    bulkCopy.ColumnMappings.Add("Pricina", "Pricina");

                    bulkCopy.BulkCopyTimeout = 60;

                    bulkCopy.WriteToServer(dataTable);
                }
                database.Close();
            }
        }


        public void InsertToTable(String stroj, DateTime zacatek, DateTime konec, String duvod, String pricina)
        {
            //SqlCommand insert = new SqlCommand("insert into Zaznamy (Stroj, Zacatek, Konec, Duvod, Pricina) values ('" + stroj + "','" + zacatek + "','" + konec + "','" + duvod + "','" + pricina + "') ", database);
            SqlCommand insert = new SqlCommand("INSERT INTO Zaznamy (Stroj, Zacatek, Konec, Duvod, Pricina) VALUES (@Stroj, @Zacatek, @Konec, @Duvod, @Pricina)", database);
            if (stroj != null)
            {
                insert.Parameters.AddWithValue("@Stroj", stroj);
            }
            else insert.Parameters.AddWithValue("@Stroj", "");

            if (zacatek != null)
            {
                insert.Parameters.AddWithValue("@Zacatek", zacatek);
            }
            else insert.Parameters.AddWithValue("@Zacatek", DateTime.Parse("2000-01-01 00:00:00"));

            if (konec != null)
            {
                insert.Parameters.AddWithValue("@Konec", konec);
            }
            else insert.Parameters.AddWithValue("@Konec", null);
            // else insert.Parameters.AddWithValue("@Konec", DateTime.Parse("2000-01-01 00:00:00"));


            if (duvod != null)
            {
                insert.Parameters.AddWithValue("@Duvod", duvod);
            }
            else insert.Parameters.AddWithValue("@Duvod", "");


            if (pricina != null)
            {
                insert.Parameters.AddWithValue("@Pricina", pricina);
            }
            else insert.Parameters.AddWithValue("@Pricina", "");

            database.Open();
            insert.ExecuteNonQuery();
            database.Close();
        }

        private void DeleteDataFromTable(String name)
        {
            string tableName = name;
            using (SqlConnection connection = new SqlConnection(databaseCon))
            {
                connection.Open();
                string deleteQuery = $"DELETE FROM {tableName}";
                string resetQuery = $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)";

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine($"Deleted {rowsAffected} rows from the table.");
                }

                using (SqlCommand command = new SqlCommand(resetQuery, connection))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                }


            }

        }


        public void getFromTable(DateTime startDate, DateTime endDate)
        {
            DeleteDataFromTable("ZaznamyLoaded");
            if (startDate != DateTime.Parse("2000-01-01 00:00:00"))
            {
                using (SqlConnection connection = new SqlConnection(databaseCon))
                {
                    database.Open();
                    connection.Open();
                    //pro záznamy kde je začátek mezi startDate a endDate a Konec je menší než endDate
                    string readCommand = "select * from Zaznamy WHERE Zacatek >= @startDate And Zacatek <= @endDate AND Konec <=@endDate";
                    SqlCommand read = new SqlCommand(readCommand, connection);
                    read.Parameters.AddWithValue("@startDate", startDate);
                    read.Parameters.AddWithValue("@endDate", endDate);
                    SqlDataReader reader = read.ExecuteReader();
                    while (reader.Read())
                    {
                        String Stroj = "", Duvod = "", Pricina = "";
                        DateTime Zacatek = DateTime.Parse("2000-01-01 00:00:00"), Konec = endDate;
                        if (reader.GetValue(1) != null) { Stroj = reader.GetString(1); }
                        if (reader.GetValue(2) != null) { Zacatek = reader.GetDateTime(2); }
                        if (reader.GetValue(3) != null) { Konec = reader.GetDateTime(3); }
                        if (reader.GetValue(4) != null) { Duvod = reader.GetString(4); }
                        if (reader.GetValue(5) != null) { Pricina = reader.GetString(5); }
                        SqlCommand insert = new SqlCommand("INSERT INTO ZaznamyLoaded (Stroj, Zacatek, Konec, Trvani, Duvod, Pricina) VALUES (@Stroj, @Zacatek, @Konec, @Trvani, @Duvod, @Pricina)", database);
                        insert.Parameters.AddWithValue("@Stroj", Stroj);
                        insert.Parameters.AddWithValue("@Zacatek", Zacatek);
                        insert.Parameters.AddWithValue("@Konec", Konec);
                        TimeSpan trvaní = Konec.Subtract(Zacatek);
                        insert.Parameters.AddWithValue("@Trvani", trvaní.TotalMinutes);
                        insert.Parameters.AddWithValue("@Duvod", Duvod);
                        insert.Parameters.AddWithValue("@Pricina", Pricina);
                        insert.ExecuteNonQuery();
                        if (trvaní.TotalMinutes < 0) { Console.WriteLine("1"); }
                    }
                    reader.Close();

                    //pro záznamy kde je začátek před startDate a konec je mezi startDate a endDate
                    string readCommand2 = "select * from Zaznamy WHERE Zacatek < @startDate And konec >= @startDate AND konec <= @endDate";
                    
                    SqlCommand read2 = new SqlCommand(readCommand2, connection);
                    read2.Parameters.AddWithValue("@startDate", startDate);
                    read2.Parameters.AddWithValue("@endDate", endDate);
                    SqlDataReader reader2 = read2.ExecuteReader();
                    while (reader2.Read())
                    {
                        String Stroj = "", Duvod = "", Pricina = "";
                        DateTime Zacatek = DateTime.Parse("2000-01-01 00:00:00"), Konec = DateTime.Parse("2000-01-01 00:00:00");
                        if (reader2.GetValue(1) != null) { Stroj = reader2.GetString(1); }
                        if (reader2.GetValue(2) != null) { Zacatek = reader2.GetDateTime(2); }
                        if (reader2.GetValue(3) != null) { Konec = reader2.GetDateTime(3); }
                        if (reader2.GetValue(4) != null) { Duvod = reader2.GetString(4); }
                        if (reader2.GetValue(5) != null) { Pricina = reader2.GetString(5); }
                        SqlCommand insert = new SqlCommand("INSERT INTO ZaznamyLoaded (Stroj, Zacatek, Konec, Trvani, Duvod, Pricina) VALUES (@Stroj, @Zacatek, @Konec, @Trvani, @Duvod, @Pricina)", database);
                        insert.Parameters.AddWithValue("@Stroj", Stroj);
                        insert.Parameters.AddWithValue("@Zacatek", startDate);
                        insert.Parameters.AddWithValue("@Konec", Konec);
                        TimeSpan trvaní = Konec.Subtract(startDate);
                        insert.Parameters.AddWithValue("@Trvani", trvaní.TotalMinutes);
                        insert.Parameters.AddWithValue("@Duvod", Duvod);
                        insert.Parameters.AddWithValue("@Pricina", Pricina);
                        insert.ExecuteNonQuery();
                        if (trvaní.TotalMinutes < 0) { Console.WriteLine("2"); }
                        
                    }
                    reader2.Close();

                    //pro záznamy kde je začátek před startDate a konec je za endDate
                    string readCommand3 = "select * from Zaznamy WHERE Zacatek < @startDate And konec > @endDate";
                    SqlCommand read3 = new SqlCommand(readCommand3, connection);
                    read3.Parameters.AddWithValue("@startDate", startDate);
                    read3.Parameters.AddWithValue("@endDate", endDate);
                    SqlDataReader reader3 = read3.ExecuteReader();
                    while (reader3.Read())
                    {
                        String Stroj = "", Duvod = "", Pricina = "";
                        DateTime? Zacatek = DateTime.Parse("2000-01-01 00:00:00"), Konec = DateTime.Parse("2000-01-01 00:00:00");
                        if (reader3.GetValue(1) != null) { Stroj = reader3.GetString(1); }
                        if (reader3.GetValue(2) != null) { Zacatek = reader3.GetDateTime(2); }
                        if (reader3.GetValue(3) != null) { Konec = reader3.GetDateTime(3); }
                        if (reader3.GetValue(4) != null) { Duvod = reader3.GetString(4); }
                        if (reader3.GetValue(5) != null) { Pricina = reader3.GetString(5); }
                        SqlCommand insert = new SqlCommand("INSERT INTO ZaznamyLoaded (Stroj, Zacatek, Konec, Trvani, Duvod, Pricina) VALUES (@Stroj, @Zacatek, @Konec, @Trvani, @Duvod, @Pricina)", database);
                        insert.Parameters.AddWithValue("@Stroj", Stroj);
                        insert.Parameters.AddWithValue("@Zacatek", startDate);
                        insert.Parameters.AddWithValue("@Konec", endDate);
                        TimeSpan trvaní = endDate.Subtract(startDate);
                        insert.Parameters.AddWithValue("@Trvani", trvaní.TotalMinutes);
                        insert.Parameters.AddWithValue("@Duvod", Duvod);
                        insert.Parameters.AddWithValue("@Pricina", Pricina);
                        insert.ExecuteNonQuery();
                        if (trvaní.TotalMinutes < 0) { Console.WriteLine("3"); }

                    }
                    reader3.Close();

                    //pro záznamy kde je začátek mezi startDate a endDate a Konec je větší než endDate
                    string readCommand4 = "select * from Zaznamy WHERE Zacatek >= @startDate And Zacatek <= @endDate AND Konec >@endDate";
                    SqlCommand read4 = new SqlCommand(readCommand4, connection);
                    read4.Parameters.AddWithValue("@startDate", startDate);
                    read4.Parameters.AddWithValue("@endDate", endDate);
                    SqlDataReader reader4 = read4.ExecuteReader();
                    while (reader4.Read())
                    {
                        String Stroj = "", Duvod = "", Pricina = "";
                        DateTime Zacatek = DateTime.Parse("2000-01-01 00:00:00"), Konec = DateTime.Parse("2000-01-01 00:00:00");
                        if (reader4.GetValue(1) != null) { Stroj = reader4.GetString(1); }
                        if (reader4.GetValue(2) != null) { Zacatek = reader4.GetDateTime(2); }
                        if (reader4.GetValue(3) != null) { Konec = reader4.GetDateTime(3); }
                        if (reader4.GetValue(4) != null) { Duvod = reader4.GetString(4); }
                        if (reader4.GetValue(5) != null) { Pricina = reader4.GetString(5); }
                        SqlCommand insert = new SqlCommand("INSERT INTO ZaznamyLoaded (Stroj, Zacatek, Konec, Trvani, Duvod, Pricina) VALUES (@Stroj, @Zacatek, @Konec, @Trvani, @Duvod, @Pricina)", database);
                        insert.Parameters.AddWithValue("@Stroj", Stroj);
                        insert.Parameters.AddWithValue("@Zacatek", Zacatek);
                        insert.Parameters.AddWithValue("@Konec", endDate);
                        TimeSpan trvaní = endDate.Subtract(Zacatek);
                        insert.Parameters.AddWithValue("@Trvani", trvaní.TotalMinutes);
                        insert.Parameters.AddWithValue("@Duvod", Duvod);
                        insert.Parameters.AddWithValue("@Pricina", Pricina);
                        insert.ExecuteNonQuery();
                        if (trvaní.TotalMinutes < 0) { Console.WriteLine("4"); }
                    }
                    reader4.Close();
                }
                database.Close();
            }
            var result = GetUniquesFromTable();
            foreach (string machine in result.Item1)
            {
                Console.WriteLine(machine);
                ListOfMachines.Items.Add(machine);
            }
            foreach (string reason in result.Item2)
            {
                Console.WriteLine(reason);
                ListOfReasons.Items.Add(reason);
            }
            foreach (string cause in result.Item3)
            {
                Console.WriteLine(cause);
                ListOfCauses.Items.Add(cause);
            }
            ListOfCauses.Items.Add("Vše");
            ListOfCauses.SelectedItem = "Vše";
            ListOfReasons.Items.Add("Vše");
            ListOfReasons.SelectedItem = "Vše";
            ListOfMachines.Items.Add("Vše");
            ListOfMachines.SelectedItem = "Vše";

            setUpBarGraph("Vše");
            setUpLineGraph("Vše");
        }

        private void setUpBarGraph(String stroj)
        {
            List<string> allCauses = new List<string>();
            allCauses.Add("Vše");
            HashSet<string> uniqueCauses = new HashSet<string>(allCauses); // odstanění duplicit
            var durationData = getDurationFromZaznamyLoaded(stroj, GetUniquesFromTable().Item2, uniqueCauses);
            chart.setUpBarChart();
            chart.barChartDataLoad(durationData.Item1, durationData.Item2);
        }

        private void setUpLineGraph(String stroj)
        {
            List<string> allCauses = new List<string>();
            allCauses.Add("Vše");
            HashSet<string> uniqueCauses = new HashSet<string>(allCauses); // odstanění duplicit
            var durationData = getDurationFromZaznamyLoaded(stroj, GetUniquesFromTable().Item2, uniqueCauses);
            chart.setUpBarChart();
            chart.barChartDataLoad(durationData.Item1, durationData.Item2);
        }

        private (HashSet<string>, HashSet<string>, HashSet<string>) GetUniquesFromTable()
        {
            HashSet<string> uniqueMachines;
            HashSet<string> uniqueReasons;
            HashSet<string> uniqueCauses;

            using (SqlConnection connection = new SqlConnection(databaseCon))
            {
                connection.Open();
                string selectUnique = "SELECT Stroj, Duvod, Pricina FROM ZaznamyLoaded";
                SqlCommand command = new SqlCommand(selectUnique, connection);

                List<string> allMachines= new List<string>();
                List<string> allReasons = new List<string>();
                List<string> allCauses = new List<string>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allMachines.Add(reader["Stroj"].ToString());
                        allReasons.Add(reader["Duvod"].ToString());
                        allCauses.Add(reader["Pricina"].ToString());
                    }
                    allMachines.Sort();
                    allReasons.Sort();
                    allCauses.Sort();
                }
                uniqueMachines = new HashSet<string>(allMachines); // odstanění duplicit
                uniqueReasons = new HashSet<string>(allReasons); // odstanění duplicit
                uniqueCauses = new HashSet<string>(allCauses); // odstanění duplicit
                CausesListBox.ItemsSource = uniqueCauses;
            }
            return (uniqueMachines, uniqueReasons, uniqueCauses);
        }

        private void SetUpGraph()/*(HashSet<string> machines, HashSet<string> reasons, HashSet<string> causes)*/
        {
            String machine = ListOfMachines.SelectedItem.ToString();
            String reason = ListOfReasons.SelectedItem.ToString();
            String cause = ListOfCauses.SelectedItem.ToString();


            
            using (SqlConnection connection = new SqlConnection(databaseCon))
            {
                String getDuration = "SELECT Trvani FROM ZaznamyLoaded WHERE (@machine <> 'Vše' AND Stroj LIKE @machine) OR (@machine Like 'Vše')  AND (@reason <> 'Vše' AND Duvod LIKE @reason) OR (@reason Like 'Vše') AND (@cause <> 'Vše' AND Pricina LIKE @cause) OR (@cause Like 'Vše')";
                List<DataPoint> dataPoinsList= new List<DataPoint>();
                connection.Open();
                SqlCommand command = new SqlCommand(getDuration, connection);

                
                command.Parameters.AddWithValue("@machine", machine);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@cause", cause);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    int duration = int.Parse(reader["Trvani"].ToString());
                    Console.WriteLine(duration);

                   // dataPoinsList.Add(new DataPoint(, duration));


                }


            }
        }


        public (List<string>, List<double>) getDurationFromZaznamyLoaded(String Machine, HashSet<string> Reasons, HashSet<string> Causes)
        {
            List<string> reasonsList = new List<string>(Reasons);
            Console.WriteLine("count list" + reasonsList.Count);
            List<string> causesList = new List<string>(Causes);

            List <String> chartReasons= new List<String>();
            List <double> chartValues = new List<double>();


            int dataCount = 0;
            if(reasonsList.Count > 1) { dataCount = reasonsList.Count; }
            else if (causesList.Count > 1) { dataCount = causesList.Count; }

            using (SqlConnection connection = new SqlConnection(databaseCon))
            {
                for (int i =0; i< dataCount; i++)
                {
                    Console.WriteLine("count list" + i);
                    String getDuration = "SELECT Trvani, Id FROM ZaznamyLoaded WHERE ((@machine <> 'Vše' AND Stroj LIKE @machine) OR (@machine Like 'Vše'))  AND ((@reason <> 'Vše' AND Duvod LIKE @reason) OR (@reason Like 'Vše')) AND ((@cause <> 'Vše' AND Pricina LIKE @cause) OR (@cause Like 'Vše'))";
                    
                    SqlCommand command = new SqlCommand(getDuration, connection);

                    command.Parameters.AddWithValue("@machine", Machine);
                    if (reasonsList.Count > 1) { command.Parameters.AddWithValue("@reason", reasonsList[i]);  } else command.Parameters.AddWithValue("@reason", reasonsList[0]);
                    if (causesList.Count > 1) { command.Parameters.AddWithValue("@cause", causesList[i]); } else command.Parameters.AddWithValue("@cause", causesList[0]);
                    connection.Close();
                    connection.Open();


                    SqlDataReader reader = command.ExecuteReader();
                    int duration = 0;

                    while (reader.Read())
                    {
                        duration=  duration + int.Parse(reader["Trvani"].ToString());
                    }
                    if (reasonsList.Count > 1) { 
                        Console.WriteLine(reasonsList[i] + "  " + duration);

                        if (reasonsList[i]!= "Není objednávka") {
                            chartReasons.Add(reasonsList[i]);
                            chartValues.Add(duration);
                        }
                    }
                    else if (causesList.Count > 1) { 
                        Console.WriteLine(causesList[i] + "  " + duration); }
                }
            }
            return (chartReasons, chartValues);
        }


        private void Load_Click(object sender, RoutedEventArgs e){
            if(startDatePicker.SelectedDate!= null && endDatePicker.SelectedDate!=null) {
                DateTime startDate = (DateTime) startDatePicker.SelectedDate;
                startDate = startDate.AddHours(6);
                DateTime endDate = (DateTime) endDatePicker.SelectedDate; //?? DateTime.Parse("2000-01-02 00:00:00");
                endDate = endDate.AddHours(6);

                getFromTable(startDate, endDate);
            }
        }


        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.Name == "ListOfMachines" && comboBox.SelectedItem != null)
            {
                setUpBarGraph(comboBox.SelectedItem.ToString());
            }
            else if (comboBox.Name == "ListOfReasons" && comboBox.SelectedItem != null)
            {
                
            }
            else if (comboBox.Name == "ListOfCauses" && comboBox.SelectedItem != null)
            {
                
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var result = GetUniquesFromTable();
        }

        private void CausesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CausesListBox.Text = "5";

        }
    }
}
