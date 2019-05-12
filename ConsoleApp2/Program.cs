﻿using Microsoft.Office.Interop.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using word = Microsoft.Office.Interop.Word;
using excel = Microsoft.Office.Interop.Excel;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            if (args.Length!=3)
            {
                Console.Out.WriteLine("Usage: ProgramName  ExcelFileWithStudents ExcelFileWithTime DocFile");

                return;
            }*/

           
            File.Copy(args[1], Path.Combine(args[1], "..", "NewFile.doc"),true);

            excel.Application excelApp = new excel.Application(); //Студенты
            word.Application wordApp = new word.Application();

           

            excel.Workbook workbook = excelApp.Workbooks.Open(args[0],ReadOnly:true);
            word.Document doc = wordApp.Documents.Open(Path.Combine(args[1],"..","NewFile.doc"));

            excel.Worksheet worksheet = workbook.Sheets[1];
            excel.Range excelRange = worksheet.UsedRange;


            Dictionary<String, HashSet<String>> contracts = new Dictionary<string, HashSet<string>>(); //словарь ключ-пара <город,договоры>
            Dictionary<String, List<Student>> dict = new Dictionary<string, List<Student>>(); //словарь ключ-пара <город,список студентов>
            Dictionary<String, String> address = new Dictionary<string, string>(); //адреса назначения 

            int index = 1;
            
            while(excelRange.Cells[index,1].Value2!=null || excelRange.Cells[index+1, 1].Value2 != null)
            {
                if (excelRange.Cells[index, 1].Value2 != null && excelRange.Cells[index, 1].Value2[0]!='2')
                {
                    Student student = new Student();
                    student.Name = excelRange.Cells[index, 1].Value2.ToString();
                    student.Group = excelRange.Cells[index, 2].Value2.ToString();
                    student.Faculty = excelRange.Cells[index, 3].Value2.ToString();
                    student.Course = excelRange.Cells[index, 4].Value2.ToString();    
                    student.City = excelRange.Cells[index, 5].Value2.ToString();
                    student.Contract = excelRange.Cells[index, 6].Value2.ToString();
                    student.Address = excelRange.Cells[index, 7].Value2.ToString();

                    String city = excelRange.Cells[index, 5].Value2.ToString();
                    if (dict.ContainsKey(city))
                    {
                        dict[city].Add(student);
                        contracts[city].Add(student.Contract);
                    }
                    else
                    {
                        dict.Add(city, new List<Student>() { student });
                        contracts.Add(city,new HashSet<string>() { student.Contract});
                    }
                }

                index++;
            }

            
            foreach(KeyValuePair<String, List<Student>> pair in dict)
            {
                Console.Out.WriteLine("====="+pair.Key+"=====");
                
                foreach(Student str in pair.Value)
                {
                    Console.Out.WriteLine(str.Name+"  "+str.Group);
                }

                Console.Out.Write("\n");
            }


            

            doc.Save();


            workbook.Close();
            excelApp.Application.Quit();

            doc.Close();
            wordApp.Application.Quit();


            Console.In.Read();
            


        }

        public static void ReplaceWord(string textToReplace,string text,word.Document wordDoc) // заменяет textToReplace на text в файле wordDoc
        {
            var range = wordDoc.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: textToReplace,ReplaceWith:text);
        }

        
    }
}
