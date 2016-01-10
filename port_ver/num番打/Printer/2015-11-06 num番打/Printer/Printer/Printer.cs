using Microsoft.PointOfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epson
{


    class Printer : IDisposable
    {
        PosPrinter m_Printer;
        public Printer(string deviceName = "PosPrinter")
        {
            var posExplorer = new PosExplorer();
            var deviceInfo = posExplorer.GetDevice(DeviceType.PosPrinter, deviceName);
            m_Printer = (PosPrinter)posExplorer.CreateInstance(deviceInfo);
            m_Printer.Open();
            m_Printer.Claim(500);
            m_Printer.DeviceEnabled = true;
            m_Printer.RecLetterQuality = true;
            m_Printer.MapMode = MapMode.Metric;
            m_Printer.ClearOutput();
        }
        public void Close()
        {
            try
            {
                m_Printer.DeviceEnabled = false;
                m_Printer.Release();
            }
            catch (PosControlException)
            {
            }
            finally
            {
                m_Printer.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
        public void TransactionStart()
        {
            m_Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Transaction);
        }
        public void TransactionSubmit()
        {
            m_Printer.TransactionPrint(PrinterStation.Receipt, PrinterTransactionControl.Normal);
        }
        public void Print(string message)
        {
            m_Printer.PrintNormal(PrinterStation.Receipt, message);
        }
        public void PrintRow(string message)
        {
            Print(message + "\n");
        }
        public void CutPaper()
        {
            m_Printer.CutPaper(100);
        }
        public void Scroll(int logn)
        {
            Print("\u001b|" + logn +"uF");
        }
        public void ScrollCutPosition()
        {
            Print("\u001b|" + m_Printer.RecLinesToPaperCut + "lF");
        }

        static public int LenDBCS(string stTaregt)
        {
            return Encoding.GetEncoding("SJIS").GetBytes(stTaregt).Length;
        }

        static public string MakePrintString(int iLineChars, string strBuf, string strPrice)
        {
            int iSpaces = 0;
            String tab = "";
            try
            {
                iSpaces = iLineChars - (LenDBCS(strBuf) + LenDBCS(strPrice));
                for (int j = 0; j < iSpaces; j++)
                {
                    tab += " ";
                }
            }
            catch (Exception)
            {
            }
            return strBuf + tab + strPrice;
        }
        public String MakePrintString(String strBuf, String strPrice)
        {
            return MakePrintString(m_Printer.RecLineChars, strBuf, strPrice);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmatNumber">1～20の値</param>
        /// <param name="strFilePath">Bitmapのファイルパス</param>
        /// <returns></returns>
        public bool SetBitmap(int bitmatNumber, string strFilePath)
        {
            if (bitmatNumber > 0 && 21 > bitmatNumber)
            {

                bool bSetBitmapSuccess = false;
                for (int iRetryCount = 0; iRetryCount < 5; iRetryCount++)
                {
                    try
                    {
                        m_Printer.SetBitmap(bitmatNumber, PrinterStation.Receipt,
                            strFilePath, m_Printer.RecLineWidth / 2,
                            PosPrinter.PrinterBitmapCenter);
                        bSetBitmapSuccess = true;
                        break;
                    }
                    catch (PosControlException pce)
                    {
                        if (pce.ErrorCode == ErrorCode.Failure && pce.ErrorCodeExtended == 0 && pce.Message == "デバイスが初期化されていません。")
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                return bSetBitmapSuccess;
            }
            return false;
        }
        public bool PrintBitmap(string strFilePath)
        {
            bool bSetBitmapSuccess = false;
            for (int iRetryCount = 0; iRetryCount < 5; iRetryCount++)
            {
                m_Printer.PrintBitmap(PrinterStation.Receipt, strFilePath,
                    m_Printer.RecLineWidth,
                    PosPrinter.PrinterBitmapCenter);
                bSetBitmapSuccess = true;
                break;
            }
            return bSetBitmapSuccess;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmatNumber">1～20のインデックス値</param>
        public void PrintBitmap(int bitmatNumber)
        {
            m_Printer.PrintNormal(PrinterStation.Receipt, "\u001b|" + bitmatNumber.ToString() + "B");
        }

        public string RowRepeatStr(string parts, int lineChars)
        {
            string buff = "";
            int i = 0;

            List<char> charList = parts.ToCharArray().ToList();
            bool flag = true;

            while (flag)
            {
                charList.ForEach(delegate (char c)
                {
                    if (i < lineChars)
                    {
                        buff += c.ToString();
                        i++;
                    }
                    else
                    {
                        flag = false;
                    }
                });
            }
            return buff;
        }
        public string RowRepeatStr(string parts)
        {
            return RowRepeatStr(parts, m_Printer.RecLineChars);
        }
    }
}
