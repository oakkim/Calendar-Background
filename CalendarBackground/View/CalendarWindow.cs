using BingHelper;
using CalendarBackground.Model;
using Nager.Date;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CalendarBackground
{
    public partial class MainWindow : Window
    {
        List<CBDay> lsDays;
        DispatcherTimer timer = new DispatcherTimer();
        DateTime TodayImageDate = new DateTime();

        private void InitTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetTime();
            SetToday();
            //CheckBingImage();
        }

        private async Task CheckBingImageAsync()
        {
            DateTime now = DateTime.Now;
            if(now.CompareTo(TodayImageDate) == 1)
            {
                TodayImageDate = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                await SetBingImageAsync();
                TodayImageDate = TodayImageDate.AddDays(1);
            }
        }

        private void SetCalendar()
        {
            lsDays = new List<CBDay>();
            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lblYear.Content = DateTime.Now.Year;
            lblMonth.Content = DateTime.Now.ToString("MMMM", new CultureInfo("en-US")).ToUpper();

            int lastDay = date.AddMonths(1).AddDays(-1).Day;

            for (int i = 1; i <= lastDay; i++)
            {
                CBDay day = new CBDay();
                day.Day = i;
                if (date.DayOfWeek == DayOfWeek.Sunday || DateSystem.IsPublicHoliday(date, CountryCode.US))
                {
                    day.Foreground = "Red";
                }
                date = date.AddDays(1);
                lsDays.Add(day);
            }

            lvCalendar.ItemsSource = lsDays;
            SetToday();
        }

        private void SetTime()
        {
            var dt = DateTime.Now;
            tbTime.Text = dt.ToString("hh:mm:ss");
            tbAmPm.Text = tbAmPm.Text = dt.ToString("tt");
        }

        private void SetToday()
        {
            if (!DateTime.Now.ToString("MMMM", new CultureInfo("en-US")).ToUpper().Equals(lblMonth.Content))
            {
                SetCalendar();
                //SetMealList();
            }

            lvCalendar.SelectedIndex = DateTime.Now.Day - 1;
            SetTodayMeal();
        }

        public async Task SetBingImageAsync()
        {
            BingImage bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            if (bingUnit != null)
            {
                //imgBingScreen.Source = new BitmapImage(new Uri(bingUnit.Path));
            }
        }
    }
}
