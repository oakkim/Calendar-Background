using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingHelper
{
    public class BingUnit
    {
        private string _startdate = string.Empty;
        public string StartDate
        {
            get
            {
                return _startdate;
            }
            set
            {
                _startdate = value;
            }
        }

        private string _enddate = string.Empty;
        public string EndDate
        {
            get
            {
                return _enddate;
            }
            set
            {
                _enddate = value;
            }
        }

        private string _path = string.Empty;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        private string _copyright = string.Empty;
        public string Copyright
        {
            get
            {
                return _copyright;
            }
            set
            {
                _copyright = value;

                if (!string.IsNullOrEmpty(_copyright))
                {
                    string str = _copyright;
                    //괄호가 description중간에 있는 경우도 있다.
                    int start = str.LastIndexOf("©");
                    if (start > 0) //0의 위치는 명백히 오류이기 때문에 '0'이상으로.
                    {
                        string desc = str.Substring(0, start - 1).Trim();
                        Discription = desc;

                        int end = str.LastIndexOf(")");

                        if (end > start)
                        {
                            end = end - start;
                            string creater = str.Substring(start, end).Trim();
                            CreatedBy = creater;
                        }
                    }
                    else
                    {
                        Discription = _copyright;
                    }
                }
            }
        }

        private string _createdBy = string.Empty;
        public string CreatedBy
        {
            get
            {
                return _createdBy;
            }
            set
            {
                _createdBy = value;
            }

        }

        private string _discription = string.Empty;
        public string Discription
        {
            get
            {
                return _discription;
            }
            set
            {
                _discription = value;
            }

        }

        private string _mapLink = string.Empty;
        public string MapLink
        {
            get
            {
                return _mapLink;
            }
            set
            {
                _mapLink = value;

                try
                {
                    if (!string.IsNullOrEmpty(_mapLink))
                    {
                        string src = _mapLink;

                        string[] separateContext = src.Split('&');

                        foreach (string sep in separateContext)
                        {
                            string temp = string.Empty;
                            int start = -1;

                            start = sep.LastIndexOf('=');
                            temp = sep.Substring(start + 1, sep.Length - (start + 1));

                            if (sep.ToLower().Contains("cp="))
                            {
                                string[] lanlon = temp.Split('~');

                                Latitude = double.Parse(lanlon[0].Trim(), CultureInfo.InvariantCulture);
                                Longitude = double.Parse(lanlon[1].Trim(), CultureInfo.InvariantCulture);
                            }
                            else if (sep.ToLower().Contains("lvl="))
                            {
                                Zoomlvl = double.Parse(temp.Trim(), CultureInfo.InvariantCulture);
                            }
                            else if (sep.ToLower().Contains("q="))
                            {
                                SerchTerm = temp.Trim().Replace("%20", " ").Replace("%2C", ",");
                            }
                            else
                            {
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private string _serchTerm = string.Empty;
        public string SerchTerm
        {
            get
            {
                return _serchTerm;
            }
            set
            {
                _serchTerm = value;
            }
        }

        private double _latitude = 0.0;
        public double Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                _latitude = value;
            }
        }

        private double _longitude = 0.0;
        public double Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                _longitude = value;
            }
        }

        private double _zoomlvl = 10.0;
        public double Zoomlvl
        {
            get
            {
                return _zoomlvl;
            }
            set
            {
                _zoomlvl = value;
            }
        }
    }
}
