using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalculateWorkTime
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelInformation.Text = "Günlük Çalışma saat aralığı format bilgileri (09:30-12:30 13:30-17:30) gibi farklı aralıklar arasında boşluk bırakın";
            /*
             8:20-12:45 13:30-17:17
             7:57-12:20 13:30-17:07
             8:18-5:12
             8:12-5:10
             8:04-5:00
             8:10-12:16 14:00-17:10
             8:00-12:00 13:00-17:15 
             8:30-16:30
             8:00-17:15
             */
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            var input = txtInputHours.Text;
            var lines = input.Split("\r\n");
            string errorText = "";
            var result = CalculateRangeFromInput(lines, ref errorText);
            lblResult.Text = result + $" Dakika \r\n {result / 60.0} Saat\r\n{errorText}";
        }
        private double CalculateRangeFromInput(string[] lines, ref string errorText)
        {
            double minute = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                var ranges = line.Trim().Split(' ');
                for (int j = 0; j < ranges.Length; j++)
                {
                    var range = ranges[j];
                    string error = "";
                    minute += CalculateMinuteFromTimeRange(range, i, j, ref error);
                    errorText += error;
                }
            }
            return minute;
        }
        private double CalculateMinuteFromTimeRange(string range, int i, int j, ref string hasError)
        {
            double minute = 0;
            try
            {
                //09:30-12:30 input format

                var dates = range.Split('-');
                if (dates.Length != 2)
                    throw new InvalidEnumArgumentException($"{i + 1} Satırda {j + 1} aralıkta hatalı aralık. \r\n");
                var startTimeArray = dates[0].Split(":");
                var endTimeArray = dates[1].Split(":");
                if (startTimeArray.Length != 2 || endTimeArray.Length != 2)
                    throw new InvalidEnumArgumentException($"{i + 1} Satırda {j + 1} aralıkta hatalı data. \r\n");
                var hour1 = int.Parse(startTimeArray[0]);
                var minute1 = int.Parse(startTimeArray[1]);
                var hour2 = int.Parse(endTimeArray[0]);
                var minute2 = int.Parse(endTimeArray[1]);
                var startTime = new DateTime(2000, 10, 1, hour1, minute1, 0);
                var endTime = new DateTime(2000, 10, 1, hour2, minute2, 0);
                if (startTime > endTime)
                    throw new InvalidEnumArgumentException($"{i + 1} Satırda {j + 1} aralıkta başlangıç tarihi bitiş tarihinden büyük. \r\n");
                TimeSpan span = endTime.Subtract(startTime);
                minute = span.TotalMinutes;

                return minute;
            }
            catch (InvalidEnumArgumentException e)
            {
                hasError += e.Message;
                return minute;
            }
            catch (Exception e)
            {

                hasError += $"{i + 1} Satırda {j + 1} aralıkta hatalı input. \r\n";
                return minute;
            }

        }
    }
}
