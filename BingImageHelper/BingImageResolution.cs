using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingHelper
{
    public enum BingImageResolution
    {
        /// <summary>
        /// 240x240
        /// </summary>
        SQUARE_240,

        /// <summary>
        /// 240x400
        /// </summary>
        PHONE_TINY,

        /// <summary>
        /// 400x240
        /// </summary>
        TINY,

        /// <summary>
        /// 958x512
        /// </summary>
        DEFAULT,

        /// <summary>
        /// 480x800
        /// </summary>
        PHONE_WVGA,

        /// <summary>
        /// 720x1280
        /// </summary>
        PHONE_720p,

        /// <summary>
        /// 768x1280
        /// </summary>
        PHONE_WXGA,

        /// <summary>
        /// 1080x1920
        /// </summary>
        PHONE_1080p,

        /// <summary>
        /// 1024x768
        /// </summary>
        SMALL,

        /// <summary>
        /// 1280x720
        /// </summary>
        NORMAL,

        /// <summary>
        /// 1366x768
        /// </summary>
        MEDIUM,

        /// <summary>
        /// 1920x1200
        /// </summary>
        LARGE,
    };
}
