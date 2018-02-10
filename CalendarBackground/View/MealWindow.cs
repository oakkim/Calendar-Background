using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using SchoolMeal;
using SchoolMeal.Exception;

namespace CalendarBackground.View
{
    public partial class MainWindow : Window
    {
        List<MealMenu> _lsMenu = new List<MealMenu>();
        private bool _isFilled = false;

        private int[] _mealTime = new int[] { 8, 13, 20 };

        public void SetMealList()
        {
            var meal = new Meal(Regions.Daegu, SchoolType.High, "D100000282");
            try
            {
                _lsMenu = meal.GetMealMenu();
                _isFilled = true;
                SetTodayMeal();
            }
            catch (FaildToParseException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void SetTodayMeal()
        {
            if (_isFilled)
            {
                lblMeal.Content = _lsMenu[DateTime.Now.Day - 1].ToString();
            }
        }
    }
}
