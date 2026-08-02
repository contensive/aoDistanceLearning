using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contensive.BaseClasses;

namespace Contensive.distanceLearningImportTool.Model.solutionModels {
    //
    public class csvFile {
        //
        public static List<csvFileRow> GetCsvFileObjectList(CPBaseClass CP, string csv_file_path, ref List<architectureModels.errorClass> errorList) {
            var csvFileRowList = new List<csvFileRow>();
            try {
                //
                var Quizzes = new List<Quizzes>();
                int rowNumber = 1;
                int columnCount = 0;
                //
                using (var reader = new StreamReader(csv_file_path)) {
                    string[] header = parseCsvLine(reader.ReadLine()); // skip header

                    if (header.Length == 10) {
                        //
                        while (!reader.EndOfStream) {
                            rowNumber += 1;
                            string[] fields = parseCsvLine(reader.ReadLine());
                            columnCount = fields.Length;
                            //
                            // Validate Number of columns
                            //
                            if (columnCount >= 4) {

                                switch (fields.Length) {
                                    case 4: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], "", "", "", "", "", ""));
                                            break;
                                        }
                                    case 5: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], "", "", "", "", ""));
                                            break;
                                        }
                                    case 6: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], fields[5], "", "", "", ""));
                                            break;
                                        }
                                    case 7: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], fields[5], fields[6], "", "", ""));
                                            break;
                                        }
                                    case 8: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], fields[5], fields[6], fields[7], "", ""));
                                            break;
                                        }
                                    case 9: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], fields[5], fields[6], fields[7], fields[8], ""));
                                            break;
                                        }
                                    case 10: {
                                            csvFileRowList.Add(csvFileRow.GetRow(CP, fields[0], fields[1], fields[2], columnCount - 3, rowNumber, fields[3], fields[4], fields[5], fields[6], fields[7], fields[8], fields[9]));
                                            break;
                                        }
                                }
                                //
                                // Order de list by Subject, rowNumber
                                //
                                //
                            } else {
                                // Number of columns are <> 10
                                CP.Utils.AppendLog($"error in number of colums in row # {rowNumber}");
                            }
                            //
                        }
                        //
                        var SortedList = csvFileRowList.OrderBy(x => x.Subject).ThenBy(x => x.rowNumber).ToList();
                        csvFileRowList = SortedList;
                        //
                    } else {
                        //
                        var oneError = new architectureModels.errorClass();
                        oneError.number = 100;
                        oneError.userMsg = "Number of columns is different to 10.";
                        errorList.Add(oneError);
                        //
                    }
                }
                //
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in csvFile.ProcessFile");
                var oneError = new architectureModels.errorClass();
                oneError.number = 100;
                oneError.userMsg = "Unexpected error in csvFile.ProcessFile.";
                errorList.Add(oneError);
            }
            return csvFileRowList;
        }
        //
        /// <summary>
        /// Parse a CSV line handling quoted fields with commas and escaped quotes.
        /// </summary>
        private static string[] parseCsvLine(string line) {
            var fields = new List<string>();
            if (string.IsNullOrEmpty(line)) {
                return fields.ToArray();
            }
            int i = 0;
            while (i <= line.Length) {
                if (i == line.Length) {
                    fields.Add("");
                    break;
                }
                if (line[i] == '"') {
                    // quoted field
                    int j = i + 1;
                    var field = new System.Text.StringBuilder();
                    while (j < line.Length) {
                        if (line[j] == '"') {
                            if (j + 1 < line.Length && line[j + 1] == '"') {
                                field.Append('"');
                                j += 2;
                            } else {
                                j++;
                                break;
                            }
                        } else {
                            field.Append(line[j]);
                            j++;
                        }
                    }
                    fields.Add(field.ToString());
                    // skip comma after closing quote
                    if (j < line.Length && line[j] == ',') {
                        j++;
                    }
                    i = j;
                } else {
                    // unquoted field
                    int commaIndex = line.IndexOf(',', i);
                    if (commaIndex == -1) {
                        fields.Add(line.Substring(i));
                        break;
                    } else {
                        fields.Add(line.Substring(i, commaIndex - i));
                        i = commaIndex + 1;
                    }
                }
            }
            return fields.ToArray();
        }
    }
    //
}
