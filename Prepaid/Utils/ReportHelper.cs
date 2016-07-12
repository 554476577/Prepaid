using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Prepaid.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Prepaid.Utils
{
    public class ReportHelper
    {
        static HSSFWorkbook hssfworkbook;

        public static string ExportUserEnergies(IEnumerable<UserEnergy> userEnergies)
        {
            string[] titles = { "用户UUID", "业主姓名", "结算时间", "设备名称", "设备累计读数", "设备结算能耗", 
                                  "设备结算价格", "总读数", "结算总能耗","结算总价格" };
            hssfworkbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("能耗缴费历史账单");
            //SetPrintSetting(sheet);

            IRow row = sheet.CreateRow(0);
            row.HeightInPoints = 30;
            for (int i = 0; i < titles.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(titles[i]);
                sheet.AutoSizeColumn(i);

                ICellStyle style = hssfworkbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                IFont font = hssfworkbook.CreateFont();
                font.IsBold = true;
                style.SetFont(font);
                cell.CellStyle = style;
            }

            int rowIndex = 0;
            for (int i = 0; i < userEnergies.Count(); i++)
            {
                UserEnergy userEnergy = userEnergies.ElementAt(i);
                int deviceCount = userEnergy.DeviceEnergies.Count();
                row = CreateCommonRow(sheet, ++rowIndex);
                // 业主UUID
                CreateCommonCell(sheet, row, 0, userEnergy.UserID);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 0, 0));
                // 业主姓名
                CreateCommonCell(sheet, row, 1, userEnergy.RealName);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 1, 1));
                // 结账时间
                CreateCommonCell(sheet, row, 2, userEnergy.DateTime.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 2, 2));
                // 总读数
                CreateCommonCell(sheet, row, 7, userEnergy.SumTotolValue.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 7, 7));
                // 结算总能耗
                CreateCommonCell(sheet, row, 8, userEnergy.SumValue.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 8, 8));
                // 结算总价格
                CreateCommonCell(sheet, row, 9, userEnergy.SumMoney.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 9, 9));
                for (int j = 0; j < deviceCount; j++)
                {
                    DeviceEnergy deviceEnergy = userEnergy.DeviceEnergies.ElementAt(j);
                    CreateCommonCell(sheet, row, 3, deviceEnergy.DeviceName);
                    CreateCommonCell(sheet, row, 4, deviceEnergy.TotolValue.ToString());
                    CreateCommonCell(sheet, row, 5, deviceEnergy.Value.ToString());
                    CreateCommonCell(sheet, row, 6, deviceEnergy.Money.ToString());
                    if (j != deviceCount - 1)
                        row = CreateCommonRow(sheet, ++rowIndex);
                }
            }

            return WriteToFile("业主能耗缴费历史账单.xls");
        }

        private static IRow CreateCommonRow(ISheet sheet, int rowIndex)
        {
            IRow row = sheet.CreateRow(rowIndex);
            row.HeightInPoints = 20;
            return row;
        }

        private static void CreateCommonCell(ISheet sheet, IRow row, int index, string value)
        {
            sheet.AutoSizeColumn(index);
            ICell cell = row.CreateCell(index);
            cell.SetCellValue(value);
            ICellStyle style = hssfworkbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle = style;
        }

        private static void InitializeWorkbook()
        {
            //Create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "广东百德朗科技有限公司";
            hssfworkbook.DocumentSummaryInformation = dsi;

            //Create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "预付费系统报表导出";
            hssfworkbook.SummaryInformation = si;
        }

        private static void SetPrintSetting(ISheet sheet)
        {
            sheet.SetMargin(MarginType.RightMargin, (double)0.5);
            sheet.SetMargin(MarginType.TopMargin, (double)0.5);
            sheet.SetMargin(MarginType.LeftMargin, (double)0.5);
            sheet.SetMargin(MarginType.BottomMargin, (double)0.5);

            sheet.PrintSetup.Copies = 1;
            sheet.PrintSetup.NoColor = true;
            sheet.PrintSetup.Landscape = true;
            sheet.PrintSetup.PaperSize = (short)PaperSize.A4;

            sheet.FitToPage = true;
            sheet.PrintSetup.FitHeight = 2;
            sheet.PrintSetup.FitWidth = 3;
            sheet.IsPrintGridlines = true;
        }

        private static string WriteToFile(string fileName)
        {
            //Write the stream data of workbook to the root directory
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, fileName);
            FileStream file = new FileStream(path, FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();

            return fileName;
        }
    }
}