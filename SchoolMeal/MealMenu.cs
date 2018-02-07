using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace SchoolMeal
{
    /// <summary>
    /// 급식메뉴에 대한 속성들을 제공합니다.
    /// </summary>
    [Serializable]
    public class MealMenu
    {

        /// <summary>
        /// 해당 날짜에 급식의 존재 여부를 제공합니다.
        /// </summary>
        public bool IsExistMenu
        {
            get
            {
                return this.Breakfast == null && this.Lunch == null && this.Dinner == null ? false : true;
            }
        }

        /// <summary>
        /// 해당 급식메뉴의 날짜를 제공합니다.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// 아침 메뉴를 제공합니다. 아침메뉴가 존재하지 않으면 null을 반환합니다.
        /// </summary>
        public List<string> Breakfast { get; }

        /// <summary>
        /// 점심 메뉴를 제공합니다. 점심메뉴가 존재하지 않으면 null을 반환합니다.
        /// </summary>
        public List<string> Lunch { get; }

        /// <summary>
        /// 저녁메뉴를 제공합니다. 저녁메뉴가 존재하지 않으면 null을 반환합니다.
        /// </summary>
        public List<string> Dinner { get; }

        /// <summary>
        /// 식사메뉴를 지정하여 <see cref="MealMenu"/>클래스의 새 인스턴스를 초기화합니다. 식사메뉴가 존재하지 않는다면 null을 지정합니다.
        /// </summary>
        /// <param name="date">급식메뉴의 날짜</param>
        /// <param name="breakfast">아침식사 메뉴</param>
        /// <param name="lunch">점심식사 메뉴</param>
        /// <param name="dinner">저녁식사 메뉴</param>
        public MealMenu(DateTime date, List<string> breakfast = null, List<string> lunch = null, List<string> dinner = null)
        {
            this.Date = date;
            this.Breakfast = breakfast;
            this.Lunch = lunch;
            this.Dinner = dinner;
        }

        /// <summary>
        /// 이 인스턴스의 값을 급식 시간표로 변환합니다.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{this.Date.Year}-{this.Date.Month}-{this.Date.Day}");
            if (this.Breakfast != null)
            {
                sb.AppendLine("- 조식 -");
                foreach (var menu in this.Breakfast)
                {
                    sb.AppendLine(menu);
                }
            }
            if (this.Lunch != null)
            {
                sb.AppendLine("- 중식 -");
                foreach (var menu in this.Lunch)
                {
                    sb.AppendLine(menu);
                }
            }
            if (this.Dinner != null)
            {
                sb.AppendLine("- 석식 -");
                foreach (var menu in this.Dinner)
                {
                    sb.AppendLine(menu);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 주어진 급식 시간대의 급식 시간표로 변환합니다.
        /// </summary>
        /// <param name="mealTime"></param>
        public string ToString(MealTime mealTime)
        {
            StringBuilder sb = new StringBuilder();
            List<string> menus = null;
            switch (mealTime)
            {
                case MealTime.BREAKFAST:
                    menus = this.Breakfast;
                    break;
                case MealTime.LUNCH:
                    menus = this.Lunch;
                    break;
                case MealTime.DINNER:
                    menus = this.Dinner;
                    break;
            }
            if (menus != null)
            {
                foreach (var menu in menus)
                {
                    sb.AppendLine(menu);
                }
            }
            return sb.ToString();
        }
    }

    public class MealTimeAttribute : Attribute
    {
        public static int[] mealTime = new int[] { 8, 13, 20 };

        public string Name { get; private set; }
        public int EndTime { get; private set; }

        public MealTimeAttribute(string Name, int EndTime)
        {
            this.Name = Name;
            this.EndTime = EndTime;
        }
    }

    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }

    /// <summary>
    /// 급식시간대를 열거합니다.
    /// </summary>
    public enum MealTime
    {
        /// <summary>
        /// 조식
        /// </summary>
        [MealTime("조식", 8)]
        BREAKFAST,
        /// <summary>
        /// 중식
        /// </summary>
        [MealTime("중식", 13)]
        LUNCH,
        /// <summary>
        /// 석식
        /// </summary>
        [MealTime("석식", 20)]
        DINNER
    }
}
