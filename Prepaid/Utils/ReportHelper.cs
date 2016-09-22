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
        static HSSFWorkbook workbook;

        public static void ExportRoomBills(IEnumerable<RoomBill> bills, string[] titles, string fileName)
        {
            workbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = workbook.CreateSheet("能耗缴费历史账单");
            //SetPrintSetting(sheet); // 设置打印格式
            SetHeaderTitle(sheet, titles); // 设置标题栏
            int rowIndex = 0;
            for (int i = 0; i < bills.Count(); i++)
            {
                RoomBill bill = bills.ElementAt(i);
                int deviceCount = bill.DeviceBills.Count();
                IRow row = CreateCommonRow(sheet, ++rowIndex);
                // 房间编号
                CreateCommonCell(sheet, row, 0, bill.RoomNo);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 0, 0));
                // 结算批号
                CreateCommonCell(sheet, row, 1, bill.LotNo);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 1, 1));
                // 业主姓名
                CreateCommonCell(sheet, row, 2, bill.RealName);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 2, 2));
                // 结账时间
                CreateCommonCell(sheet, row, 3, bill.DateTime.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 3, 3));
                // 总费用
                CreateCommonCell(sheet, row, 10, bill.SumMoney.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 10, 10));
                // 账户余额
                CreateCommonCell(sheet, row, 11, bill.BilledAccountBalance);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 11, 11));
                for (int j = 0; j < deviceCount; j++)
                {
                    DeviceBill deviceBill = bill.DeviceBills.ElementAt(j);
                    CreateCommonCell(sheet, row, 4, deviceBill.DeviceNo);
                    CreateCommonCell(sheet, row, 5, deviceBill.DeviceName);
                    CreateCommonCell(sheet, row, 6, deviceBill.PreValue.ToString());
                    CreateCommonCell(sheet, row, 7, deviceBill.CurValue.ToString());
                    CreateCommonCell(sheet, row, 8, deviceBill.Price);
                    CreateCommonCell(sheet, row, 9, deviceBill.Money);
                    if (j != deviceCount - 1)
                        row = CreateCommonRow(sheet, ++rowIndex);
                }
            }

            WriteToFile(fileName);
        }

        public static void ExportPrepaidBills(IEnumerable<PrepaidBill> prepaidBills, string[] titles, string fileName)
        {
            workbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = workbook.CreateSheet("能耗预付费实时账单");
            //SetPrintSetting(sheet); // 设置打印格式
            SetHeaderTitle(sheet, titles); // 设置标题栏
            int rowIndex = 0;
            for (int i = 0; i < prepaidBills.Count(); i++)
            {
                PrepaidBill bill = prepaidBills.ElementAt(i);
                int deviceCount = bill.PrepaidDeviceBills.Count();
                IRow row = CreateCommonRow(sheet, ++rowIndex);
                // 房间编号
                CreateCommonCell(sheet, row, 0, bill.RoomNo);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 0, 0));
                // 建筑编号
                CreateCommonCell(sheet, row, 1, bill.BuildingNo);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 1, 1));
                // 业主姓名
                CreateCommonCell(sheet, row, 2, bill.RealName);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 2, 2));
                // 总价格
                CreateCommonCell(sheet, row, 9, bill.SumMoney);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 9, 9));
                // 账户余额
                CreateCommonCell(sheet, row, 10, bill.AccountBalance);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 10, 10));
                // 结算余额
                CreateCommonCell(sheet, row, 11, bill.BilledBalance);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 11, 11));
                for (int j = 0; j < deviceCount; j++)
                {
                    PrepaidDeviceBill deviceBill = bill.PrepaidDeviceBills.ElementAt(j);
                    CreateCommonCell(sheet, row, 3, deviceBill.DeviceNo);
                    CreateCommonCell(sheet, row, 4, deviceBill.DeviceName);
                    CreateCommonCell(sheet, row, 5, deviceBill.PreValue.ToString());
                    CreateCommonCell(sheet, row, 6, deviceBill.CurValue.ToString());
                    CreateCommonCell(sheet, row, 7, deviceBill.Price);
                    CreateCommonCell(sheet, row, 8, deviceBill.Money);
                    if (j != deviceCount - 1)
                        row = CreateCommonRow(sheet, ++rowIndex);
                }
            }

            WriteToFile(fileName);
        }

        public static void Export<T>(Dictionary<string, string> pairs, IEnumerable<T> sources, string fileName)
        {
            string sheetName = fileName.Substring(0, fileName.LastIndexOf('.'));
            string[] titles = pairs.Keys.ToArray();
            string[] properties = pairs.Values.ToArray();
            workbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            //SetPrintSetting(sheet); // 设置打印格式
            SetHeaderTitle(sheet, titles); // 设置标题栏
            for (int i = 0; i < sources.Count(); i++)
            {
                T item = sources.ElementAt(i);
                IRow row = CreateCommonRow(sheet, i + 1);
                for (int j = 0; j < properties.Length; j++)
                {
                    var propertyInfo = item.GetType().GetProperty(properties[j]);
                    object obj = propertyInfo.GetValue(item);
                    string value = string.Empty;
                    if (obj != null)
                        value = obj.ToString();
                    CreateCommonCell(sheet, row, j, value);
                }
            }

            WriteToFile(fileName);
        }

        private static void SetHeaderTitle(ISheet sheet, string[] titles)
        {
            IRow row = sheet.CreateRow(0);
            row.HeightInPoints = 30;
            for (int i = 0; i < titles.Length; i++)
            {
                ICell cell = row.CreateCell(i);
                cell.SetCellValue(titles[i]);
                sheet.AutoSizeColumn(i);

                ICellStyle style = workbook.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                IFont font = workbook.CreateFont();
                font.IsBold = true;
                style.SetFont(font);
                cell.CellStyle = style;
            }
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
            ICellStyle style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle = style;
        }

        private static void InitializeWorkbook()
        {
            //Create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "广东百德朗科技有限公司";
            workbook.DocumentSummaryInformation = dsi;

            //Create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "预付费系统报表导出";
            workbook.SummaryInformation = si;
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

        private static void WriteToFile(string fileName)
        {
            //Write the stream data of workbook to the root directory
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, fileName);
            FileStream file = new FileStream(path, FileMode.Create);
            workbook.Write(file);
            file.Close();
        }
    }
}