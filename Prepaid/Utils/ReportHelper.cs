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

        public static void ExportUserEnergies(IEnumerable<UserEnergy> userEnergies, string[] titles, string fileName)
        {
            workbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = workbook.CreateSheet("能耗缴费历史账单");
            //SetPrintSetting(sheet); // 设置打印格式
            SetHeaderTitle(sheet, titles); // 设置标题栏
            int rowIndex = 0;
            for (int i = 0; i < userEnergies.Count(); i++)
            {
                UserEnergy userEnergy = userEnergies.ElementAt(i);
                int deviceCount = userEnergy.DeviceEnergies.Count();
                IRow row = CreateCommonRow(sheet, ++rowIndex);
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

            WriteToFile(fileName);
        }

        public static void ExportPrepaidEnergies(IEnumerable<PrepaidEnergy> prepaidEnergies, string[] titles, string fileName)
        {
            workbook = new HSSFWorkbook();
            InitializeWorkbook();
            ISheet sheet = workbook.CreateSheet("能耗缴费实时账单");
            //SetPrintSetting(sheet); // 设置打印格式
            SetHeaderTitle(sheet, titles); // 设置标题栏
            int rowIndex = 0;
            for (int i = 0; i < prepaidEnergies.Count(); i++)
            {
                PrepaidEnergy prepaidEnergy = prepaidEnergies.ElementAt(i);
                int deviceCount = prepaidEnergy.InstantDeviceEnergies.Count();
                IRow row = CreateCommonRow(sheet, ++rowIndex);
                // 业主UUID
                CreateCommonCell(sheet, row, 0, prepaidEnergy.UserID);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 0, 0));
                // 业主姓名
                CreateCommonCell(sheet, row, 1, prepaidEnergy.RealName);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 1, 1));
                // 建筑名称
                CreateCommonCell(sheet, row, 2, prepaidEnergy.BuildingName);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 2, 2));
                // 房间编号
                CreateCommonCell(sheet, row, 3, prepaidEnergy.RoomNo);
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 3, 3));
                // 当前总能耗
                CreateCommonCell(sheet, row, 9, prepaidEnergy.CurrentSumValue.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 9, 9));
                // 当前结算总价
                CreateCommonCell(sheet, row, 10, prepaidEnergy.CurrentSumMoney.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 10, 10));
                // 账户余额
                CreateCommonCell(sheet, row, 11, prepaidEnergy.AccountBalance.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 11, 11));
                // 结算后账户余额
                CreateCommonCell(sheet, row, 12, prepaidEnergy.CurrentAccountBalance.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 12, 12));
                // 账户报警金额
                CreateCommonCell(sheet, row, 13, prepaidEnergy.AccountWarnLimit.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 13, 13));
                // 账户可透支金额
                CreateCommonCell(sheet, row, 14, prepaidEnergy.MaxArrears.ToString());
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + deviceCount - 1, 14, 14));
                for (int j = 0; j < deviceCount; j++)
                {
                    InstantDeviceEnergy deviceEnergy = prepaidEnergy.InstantDeviceEnergies.ElementAt(j);
                    CreateCommonCell(sheet, row, 4, deviceEnergy.DeviceName);
                    CreateCommonCell(sheet, row, 5, deviceEnergy.PreValue.ToString());
                    CreateCommonCell(sheet, row, 6, deviceEnergy.CurrentValue.ToString());
                    CreateCommonCell(sheet, row, 7, deviceEnergy.IntervalValue.ToString());
                    CreateCommonCell(sheet, row, 8, deviceEnergy.IntervalValue.ToString());
                    if (j != deviceCount - 1)
                        row = CreateCommonRow(sheet, ++rowIndex);
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