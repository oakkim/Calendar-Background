using SchoolMeal;
using SchoolMeal.Exception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;


namespace CalendarBackground
{
    public partial class MainWindow : Window
    {
        List<MealMenu> lsMenu = new List<MealMenu>();
        private bool _isFilled = false;

        private int[] mealTime = new int[] { 8, 13, 20 };

        public void SetMealList()
        {
            Meal meal = new Meal(Regions.Daegu, SchoolType.High, "D100000282");
            try
            {
                lsMenu = meal.GetMealMenu();
                _isFilled = true;
                SetTodayMeal();
            }
            catch (FaildToParseException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void SetTodayMeal()
        {
            if (_isFilled)
            {
                lblMeal.Content = lsMenu[DateTime.Now.Day - 1].ToString();
            }
        }
    }
}
