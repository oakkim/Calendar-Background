using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using BingHelper;
using CalendarBackground.Model;
using Nager.Date;

namespace CalendarBackground.View
{
    public partial class MainWindow : Window
    {
        private List<CBDay> _lsDays;
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private DateTime _todayImageDate = new DateTime();

        private void InitTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetTime();
            SetToday();
            //CheckBingImage();
        }

        private async Task CheckBingImageAsync()
        {
            var now = DateTime.Now;
            if(now.CompareTo(_todayImageDate) == 1)
            {
                _todayImageDate = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                await SetBingImageAsync();
                _todayImageDate = _todayImageDate.AddDays(1);
            }
        }

        private void SetCalendar()
        {
            _lsDays = new List<CBDay>();
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            lblYear.Content = DateTime.Now.Year;
            lblMonth.Content = DateTime.Now.ToString("MMMM", new CultureInfo("en-US")).ToUpper();

            var lastDay = date.AddMonths(1).AddDays(-1).Day;

            for (int i = 1; i <= lastDay; i++)
            {
                var day = new CBDay {Day = i};
                if (date.DayOfWeek == DayOfWeek.Sunday || DateSystem.IsPublicHoliday(date, CountryCode.US))
                {
                    day.Foreground = "Red";
                }
                date = date.AddDays(1);
                _lsDays.Add(day);
            }

            lvCalendar.ItemsSource = _lsDays;
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

        private static async Task SetBingImageAsync()
        {
            var bingImage = new BingImage();
            var bingUnit = await bingImage.GetTodayBingUnit(BingImageResolution.LARGE);
            if (bingUnit != null)
            {
                //imgBingScreen.Source = new BitmapImage(new Uri(bingUnit.Path));
            }
        }
    }
}
